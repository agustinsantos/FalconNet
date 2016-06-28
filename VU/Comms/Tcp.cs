using FalconNet.Common.Encoding;
using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;

namespace FalconNet.VU.Comms
{
    [StructLayout(LayoutKind.Sequential)]
    public struct tcpHeader
    {
        /* HEADER magic value  for TCP COM messages */
        //#define     HEADER_BASE 0xFEEDFACE
        public const ushort HEADER_BASE = 0xFACE;
        public const int Size = sizeof(ushort) * 3;

        public ushort header_base;
        public ushort size;
        public ushort inv_size;
        /*int  header_size; */

        public static tcpHeader Decode(byte[] buffer, ref int pos)
        {
            tcpHeader header = new tcpHeader();
            header.header_base = UInt16EncodingLE.Decode(buffer, pos); pos += sizeof(ushort);
            header.size = UInt16EncodingLE.Decode(buffer, pos); pos += sizeof(ushort);
            header.inv_size = UInt16EncodingLE.Decode(buffer, pos); pos += sizeof(ushort);
            return header;
        }
    }


    public enum HandleTypes : short
    {
        LISTENER = 0,
        GROUP = 1,
        CONNECTION = 2,
    }

    public class ComTCP : ComIP
    {
        public IPEndPoint Addr;

        public short timeoutsecs;
        public HandleTypes handletype;
        public COMAPIStatus state;
        public short ListenPort;
        public int messagesize;
        public int headersize;
        public int recv_buffer_start;
        public int bytes_needed_for_header;
        public int bytes_needed_for_message;
        public int bytes_recvd_for_message;
        public tcpHeader Header;
        public CommApi.ConnectCallback connect_callback_func;
        public CommApi.AcceptCallback accept_callback_func;
    }
    public static partial class CommApi
    {
        #region TCP 

        static Thread AcceptThread;
        static Thread ConnectThread;

        static int AcceptConnectioncount = 0;
        static int AcceptCount = 0;

        /* List head for connection list */
        private static CAPIList GlobalListHead = null; // any ocurrance of this outside tcp is a bug
        private static CAPIList GlobalGroupListHead = null;
        private static int ComAPILastError = 0;

        /* some thread control flags  for the accpetconnection() thread*/
        const int THREAD_STOP = 0;
        const int THREAD_ACTIVE = 1;
        const int THREAD_TERMINATED = 2;
        const int SLEEP_IN_ACCEPT = 1000;

        /* TCP  specific open .. */
        /* begin a TCP connection as a listener to wait for connections */
        public static ComTCP ComTCPOpenListen(int buffersize, string gamename, int tcpPort, AcceptCallback acb)
        {
            ComTCP c;
            int err = 0;
            // int trueValue=1;
            // int falseValue=0;
            Socket listen_sock = null;
            CAPIList listitem;

            /* InitWS2 checks that WSASstartup is done only once and increments reference count*/
#if TODO
            byte[] wsaData;
            if (InitWS2(ref wsaData) == 0)
            {
                return null;
            }
#endif

            enter_cs();
            /* Is this port already being listened to?*/
            listitem = CAPIListFindTCPListenPort(GlobalListHead, (short)tcpPort);

            if (listitem != null)
            {
                c = (ComTCP)listitem.com;
                REQUEST_LOCK(c.lock_);
                c.referencecount++;
                RELEASE_LOCK(c.lock_);

                leave_cs();
                return c;
            }

            /* CREATE NEW LISTENER */
            /* add new socket connection to list */
            /* although this is only a listener socket */
            GlobalListHead = CAPIListAppend(GlobalListHead);

            if (GlobalListHead == null)
            {
                leave_cs();
                return null;
            }

            /* allocate a new ComHandle struct */
            c = new ComTCP();
            GlobalListHead.com = c;
            //GlobalListHead.ListenPort = tcpPort;
            //memset(c, 0, sizeof(ComTCP));

            /* initialize header data */
            c.accept_callback_func = acb;
            c.apiheader.protocol = ComAPIEnumProtocols.CAPI_TCP_PROTOCOL;
            c.apiheader.send_func = ComTCPSend;
            c.apiheader.sendX_func = ComTCPSendX;
            c.apiheader.recv_func = ComTCPGetMessage;
            c.apiheader.send_buf_func = ComTCPSendBufferGet;
            c.apiheader.recv_buf_func = ComTCPRecvBufferGet;
            c.apiheader.addr_func = ComIPHostIDGet;
            c.apiheader.close_func = ComTCPClose;
            c.apiheader.query_func = ComTCPQuery;
            c.apiheader.get_timestamp_func = ComTCPGetTimeStamp;
            c.handletype = HandleTypes.LISTENER;
            c.referencecount = 1;
            c.buffer_size = tcpHeader.Size + buffersize;
            c.ListenPort = (short)tcpPort;
            c.state = COMAPIStatus.COMAPI_STATE_CONNECTED;

            /* create socket */
            try
            {

                listen_sock = c.recv_sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            }
            catch (SocketException ex)
            {
                ComAPILastError = err = ex.ErrorCode;// CAPI_WSAGetLastError();
            }

            /* Set Server Address... */
            //memset((char*)&c.Addr, 0, sizeof(c.Addr));

            c.Addr = new IPEndPoint(IPAddress.Any, tcpPort);

            /* Bind to local address -- don't really need this but Hey ... */
            try
            {
                listen_sock.Bind(c.Addr);
            }
            catch (SocketException ex)
            {
                err = ex.ErrorCode;
                // int error = ex.ErrorCode; //CAPI_WSAGetLastError();
                leave_cs();
                return null;
            }

            /* listen() backlog options */
            const int MAXBACKLOG = 5;
            /* now listen on this socket */
            try
            {
                listen_sock.Listen(MAXBACKLOG);
            }
            catch (SocketException ex)
            {
                err = ex.ErrorCode;
                // int error = ex.ErrorCode; // CAPI_WSAGetLastError();
                leave_cs();
                return null;
            }

            /* Create a mutex for this connection */
            CREATE_LOCK(c.lock_, "Listen socket");

            /* Mark this thread ACCEPT() active in the data struct */
            c.ThreadActive = THREAD_ACTIVE;

            /* Start the thread which waits for socket connectiosn with accept() */
            c.ThreadHandle = new Thread(AcceptConnection);

            if (c.ThreadHandle == null)
            {
                // DWORD error = GetLastError();
            }

            //SetThreadPriority(c.ThreadHandle, THREAD_PRIORITY_IDLE);
            leave_cs();
            return c;
        }


        /* begin a TCP connection to a targeted listening TCP listener */
        public static ComAPIHandle ComTCPOpenConnect(int buffersize, string gamename, int tcpPort, uint IPaddress, ConnectCallback ccb, int timeoutsecs)
        {
            ComTCP c;
            int err;
            // int trueValue=1;
            // int falseValue=0;
            CAPIList listitem;


            /* InitWS2 checks that WSASstartup is done only once and increments reference count*/
#if TODO
            byte[] wsaData;
            if (InitWS2(&wsaData) == 0)
            {
                return null;
            }
#endif

            enter_cs();
            /* GFG */
            listitem = CAPIListFindTCPIPaddress(GlobalListHead, new IPEndPoint(IPaddress, tcpPort));

            if (listitem != null)
            {
                leave_cs();
                return null;
            }

            GlobalListHead = CAPIListAppend(GlobalListHead);

            if (GlobalListHead == null)
            {
                leave_cs();
                return null;
            }

            /* allocate a new ComHandle struct */
            c = new ComTCP();
            GlobalListHead.com = c;
            //memset(c, 0, sizeof(ComTCP));

            /* InitWS2 checks that WSASstartup is done only once and increments reference count*/
#if TODO
            if (InitWS2(&c.wsaData) == 0)
            {
                GlobalListHead = CAPIListRemove(GlobalListHead, (ComAPIHandle)c);
                leave_cs();
                //free(c);

                return null;
            }
#endif

            leave_cs();

            /* initialize header data */
            c.connect_callback_func = ccb;
            c.apiheader.protocol = ComAPIEnumProtocols.CAPI_TCP_PROTOCOL;
            c.apiheader.send_func = ComTCPSend;
            c.apiheader.sendX_func = ComTCPSendX;
            c.apiheader.recv_func = ComTCPGetMessage;
            c.apiheader.send_buf_func = ComTCPSendBufferGet;
            c.apiheader.recv_buf_func = ComTCPRecvBufferGet;
            c.apiheader.query_func = ComTCPQuery;
            c.apiheader.addr_func = ComIPHostIDGet;
            c.apiheader.close_func = ComTCPClose;
            c.apiheader.get_timestamp_func = ComTCPGetTimeStamp;

            c.buffer_size = buffersize + tcpHeader.Size;
            c.send_buffer = new byte[c.buffer_size];
            c.send_buffer_start = 0;
            c.recv_buffer = new byte[c.buffer_size];
            c.recv_buffer_start = 0;
            c.timeoutsecs = (short)timeoutsecs;
            c.handletype = HandleTypes.CONNECTION;
            c.referencecount = 1;

            /* create socket */
            try
            {
                c.recv_sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            }
            catch (SocketException ex)
            {
                err = ex.ErrorCode;
                ComTCPFreeData(c);
                return null;
            }

            /* Server Address... */
            //memset((char*)&c.Addr, 0, sizeof(c.Addr));
            c.Addr = new IPEndPoint(new IPAddress(IPAddress.HostToNetworkOrder(IPaddress)), IPAddress.HostToNetworkOrder(tcpPort));
            //c.Addr.sin_family = AF_INET;
            //c.Addr.sin_port = IPAddress.HostToNetworkOrder(tcpPort);
            //c.Addr.sin_addr.s_addr = IPAddress.HostToNetworkOrder(IPaddress);

            /* create a mutex */
            CREATE_LOCK(c.lock_, "connect socket");
            REQUEST_LOCK(c.lock_);
            c.state = COMAPIStatus.COMAPI_STATE_CONNECTION_PENDING;
            RELEASE_LOCK(c.lock_);

            /* Create thread which attempt to make connection */
            c.ThreadHandle = new Thread(RequestConnection);

            if (c.ThreadHandle == null)
            {
                // DWORD error = GetLastError();
                ComTCPFreeData(c);
                return null;
            }

            return (ComAPIHandle)c;
        }


        /* Get handle for an anticipated acception from target IP */
        /* Fails if no Listening Handle has not been previously Opened on the port */
        public static ComAPIHandle ComTCPOpenAccept(uint IPaddress, int tcpPort, int timeoutsecs)
        {
            ComTCP c, listener;
            CAPIList listitem;

            /* InitWS2 checks that WSASstartup is done only once and increments reference count*/
#if TODO
            byte[] wsaData;
            if (InitWS2(&wsaData) == 0)
            {
                return null;
            }
#endif

            if (IPaddress == 0)
            {
                return null;
            }

            enter_cs();

            if ((listitem = CAPIListFindTCPListenPort(GlobalListHead, (short)tcpPort)) == null)
            {
                leave_cs();
                return null;
            }

            listener = (ComTCP)listitem.com;

            /* Is this handle already in our list */
            listitem = CAPIListFindTCPIPaddress(GlobalListHead, new IPEndPoint(IPaddress, tcpPort));

            if (listitem != null)
            {
                leave_cs();

                return listitem.com;
            }

            /* add connection to our local list */
            GlobalListHead = CAPIListAppend(GlobalListHead);

            if (GlobalListHead == null)
            {
                leave_cs();
                return null;
            }

            /* allocate a new ComHandle struct */
            c = new ComTCP();
            GlobalListHead.com = c;
            //memset(c, 0, sizeof(ComTCP));
            /* copy the Listen socket's ComHandle data into the Accepted socket's ComHandle*/
            //memcpy(c, listener, sizeof(ComTCP));
            listener = c;
            c.recv_sock = c.send_sock = null;
            c.state = COMAPIStatus.COMAPI_STATE_CONNECTION_PENDING;
            ((ComAPIHandle)c).protocol = ComAPIEnumProtocols.CAPI_TCP_PROTOCOL;
            c.lock_ = null;

            /* Create the target address... */
            //memset((char*)&c.Addr, 0, sizeof(c.Addr));
            c.Addr = new IPEndPoint(IPAddress.HostToNetworkOrder(IPaddress), IPAddress.HostToNetworkOrder(tcpPort));
            //c.Addr.sin_family = AF_INET;
            //c.Addr.sin_port = IPAddress.HostToNetworkOrder(tcpPort);
            //c.Addr.sin_addr.s_addr = IPAddress.HostToNetworkOrder(IPaddress);

            c.send_buffer = new byte[c.buffer_size];
            c.send_buffer_start = 0;
            c.recv_buffer = new byte[c.buffer_size];
            c.recv_buffer_start = 0;
            c.ThreadActive = THREAD_STOP;
            c.handletype = HandleTypes.CONNECTION;

            c.bytes_needed_for_message = (int)(DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond);  /* use this variable to hold start time */
            c.timeoutsecs = (short)timeoutsecs;

            leave_cs();

            return (ComAPIHandle)c;
        }

        /* get a handle to use with ComAPISend for sending data to all TCP connections */
        /* will send to all open connections, either accept() or connect() type */
        /* will ignore listen sockets */
        public static ComAPIHandle ComTCPGetGroupHandle(int buffersize)
        { throw new NotImplementedException(); }

        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////

        static void AcceptConnection(object cvoid)
        {
            ComTCP ctcpListen = (ComTCP)cvoid;   /* This is the handle to the listening socket */
            SocketError err = 0;
            IPEndPoint comCliAddr = null;
            bool trueValue = true;
            Socket connecting_sock;
            ComTCP ctcpAccept;

            AcceptConnectioncount++;


            /* A call to ComTCPClose will change .ThreadActive so we can gracefully exit this thread */
            while (true)
            {
                REQUEST_LOCK(ctcpListen.lock_);

                if (ctcpListen.ThreadActive != THREAD_ACTIVE)
                {
                    RELEASE_LOCK(ctcpListen.lock_);
                    break;
                }

                RELEASE_LOCK(ctcpListen.lock_);

                /*    Sleep(SLEEP_IN_ACCEPT);    */
                err = 0;

                /* wait for a connection */
                try
                {
                    connecting_sock = ctcpListen.recv_sock.Accept();
                    comCliAddr = (IPEndPoint)connecting_sock.RemoteEndPoint;
                    AcceptCount++;
                    REQUEST_LOCK(ctcpListen.lock_);

                    /* if reasonable ERROR loop back and wait for another connection */
                }
                catch (SocketException ex)
                {
                    err = ex.SocketErrorCode;// CAPI_WSAGetLastError();

                    if (err == SocketError.WouldBlock)
                    {
                        CAPIList listitem;

                        enter_cs();

                        listitem = CAPIListFindTCPAcceptPendingExpired(GlobalListHead);

                        leave_cs();

                        if (listitem != null)
                        {
                            listitem.com.close_func(listitem.com);
                            ((ComTCP)(listitem.com)).accept_callback_func(listitem.com, (int)ComAPIEnumErrorCodes.COMAPI_CONNECTION_TIMEOUT);
                        }

                        RELEASE_LOCK(ctcpListen.lock_);

                        continue;
                    }
                    else if (ctcpListen.ThreadActive != THREAD_ACTIVE && err == SocketError.NotSocket)
                    {
                        break;
                    }
                    else
                    {
                        RELEASE_LOCK(ctcpListen.lock_);
                        break;
                    }
                }
                //else
                {
                    CAPIList listitem;
                    byte[] save_recv_buffer;
                    byte[] save_send_buffer;

                    save_recv_buffer = null;
                    save_send_buffer = null;

                    enter_cs();
                    listitem = CAPIListFindTCPIPaddress(GlobalListHead, new IPEndPoint(comCliAddr.Address, ctcpListen.Addr.Port));

                    if (listitem != null)
                    {
                        ctcpAccept = (ComTCP)listitem.com;
                        save_recv_buffer = ctcpAccept.recv_buffer;
                        save_send_buffer = ctcpAccept.send_buffer;
                    }
                    else
                    {
                        /*  Create a new entry in list of connections */

                        GlobalListHead = CAPIListAppend(GlobalListHead);

                        if (GlobalListHead == null)
                        {
                            leave_cs();
                            return;
                        }

                        /* allocate a new ComHandle struct */
                        ctcpAccept = new ComTCP();
                        GlobalListHead.com = ctcpAccept;
                        //memset(ctcpAccept, 0, sizeof(ComTCP));
                    }

                    leave_cs();

                    /* copy the Listen socket's ComHandle data into the Accepted socket's ComHandle*/
                    //memcpy(ctcpAccept, ctcpListen, sizeof(ComTCP));
                    ctcpListen = ctcpAccept;

                    /* BUT  no thread on accepted sockets  */
                    ctcpAccept.ThreadActive = THREAD_STOP;
                    ctcpAccept.handletype = HandleTypes.CONNECTION;

                    /* copy  who we are connected to */
                    ctcpAccept.Addr = comCliAddr;

                    /* set the socket numbers */
                    ctcpAccept.recv_sock = ctcpAccept.send_sock = connecting_sock;

                    /* allocate send and receive data buffers */
                    if (save_send_buffer == null)
                    {
                        ctcpAccept.send_buffer = new byte[ctcpListen.buffer_size];
                    }
                    else
                    {
                        ctcpAccept.send_buffer = save_send_buffer;
                    }

                    if (save_recv_buffer == null)
                    {
                        ctcpAccept.recv_buffer = new byte[ctcpListen.buffer_size];
                    }
                    else
                    {
                        ctcpAccept.recv_buffer = save_recv_buffer;
                    }

                    ctcpAccept.send_buffer_start = 0;
                    ctcpAccept.recv_buffer_start = 0;
                    ctcpAccept.referencecount = 1;

                    /* Create a mutex for this connection */
                    CREATE_LOCK(ctcpAccept.lock_, "accept socket");

                    /* Now set the new socket for Non-Blocking IO */
                    try
                    {
                        ctcpAccept.recv_sock.Blocking = trueValue;
                    }
                    catch (SocketException ex)
                    {
                        err = ex.SocketErrorCode;// CAPI_WSAGetLastError();
                    }

                    /* turn off the Nagle alog. which buffers small messages */
                    try
                    {
                        ctcpAccept.recv_sock.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, trueValue);
                    }
                    catch (SocketException ex)
                    {
                        err = ex.SocketErrorCode;// CAPI_WSAGetLastError();
                    }

                    /* initialize header data values */
                    initComTCP(ctcpAccept);

                    /* Increment reference count of openconnections */
                    /* we have to do this here because no explicit WS2Init() was called */
                    WS2Connections++;

                    /* Notify User thru his callback that we have connected on a socket */
                    ctcpAccept.state = COMAPIStatus.COMAPI_STATE_ACCEPTED;

                    ctcpAccept.accept_callback_func((ComAPIHandle)ctcpAccept, 0);

                    /* Any errors ? */
                    if (err != 0)
                    {
                        RELEASE_LOCK(ctcpListen.lock_);
                        break;
                    }

                    RELEASE_LOCK(ctcpListen.lock_);
                }
            }

            /* If we get here we need to exit to terminate the thread */

            REQUEST_LOCK(ctcpListen.lock_);

            if (ctcpListen.ThreadActive == THREAD_ACTIVE) /* we got here due to a break */
            {
                /* indicate that thread is inactive, about to exit */
                ctcpListen.ThreadActive = THREAD_STOP;
                RELEASE_LOCK(ctcpListen.lock_);

            }
            else
            {
                /* indicate that we are exiting to ComTCPClose which is waiting for exit of thread*/
                ctcpListen.ThreadActive = THREAD_TERMINATED;
                RELEASE_LOCK(ctcpListen.lock_);
            }

            return;
        }

        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////

        static void RequestConnection(object cvoid)
        {
            ComTCP c = (ComTCP)cvoid;
            int err;
            bool trueValue = true;
            long starttime, endtime;

            /* set the new socket for Non-Blocking IO */
            try
            {
                c.recv_sock.Blocking = trueValue;
            }
            catch (SocketException ex)
            {
                err = ex.ErrorCode;
            }

            starttime = DateTime.Now.Ticks;
            c.state = COMAPIStatus.COMAPI_STATE_CONNECTION_PENDING;

            while (true)
            {
                //??Sleep(0);

                /* Look for a connection */
                try
                {
                    c.recv_sock.Connect(c.Addr);
                    /* it means successful connection */
                    break;
                }
                catch (SocketException ex)
                {
                    /* check for timeout */
                    endtime = (DateTime.Now.Ticks - starttime) / TimeSpan.TicksPerSecond;

                    /* keep waiting */
                    if (endtime < c.timeoutsecs)
                    {
                        continue;
                    }
                    else
                    {
                        /* error or time out here , so inform user and then exit*/
                        c.state = COMAPIStatus.COMAPI_STATE_CONNECTED;
                        c.connect_callback_func((ComAPIHandle)c, -1 * ex.ErrorCode);

                        return;
                    }
                }
            }

            /* If we get here we have a good connection */
            /* Set send socket to the same */
            c.send_sock = c.recv_sock;

            try
            {
                //err = CAPI_setsockopt(c.recv_sock, IPPROTO_TCP, TCP_NODELAY, (char*)&trueValue, sizeof(trueValue));
                c.recv_sock.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, trueValue);
            }
            catch (SocketException ex)
            {
                err = ex.ErrorCode;
            }

            //if (err != 0)
            //{
            //    err = CAPI_WSAGetLastError();
            //}

            /* initialize some data */
            initComTCP(c);

            REQUEST_LOCK(c.lock_);

            c.state = COMAPIStatus.COMAPI_STATE_CONNECTED;

            RELEASE_LOCK(c.lock_);

            /* Notify User that we have connected on a socket */
            c.connect_callback_func((ComAPIHandle)c, 0);

            return;
        }

        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////

        static void ComTCPClose(ComAPIHandle c)
        {
            int sockerror;
            ComTCP ctcp = (ComTCP)c;

            if (c == null)
            {
                return;
            }

            enter_cs();

            if (CAPIListFindHandle(GlobalListHead, c) == null)
            {
                leave_cs();

                return; /* is it in  our list ? */
            }

            leave_cs();

            ctcp.referencecount--;

            if (ctcp.referencecount != 0)
            {
                return;
            }

            if (ctcp.lock_ != null)
            {
                REQUEST_LOCK(ctcp.lock_);
            }

            if (ctcp.recv_sock != null)
            {
                /* if this is a listening socket, we have an active listening thread*/
                if (ctcp.ThreadActive == THREAD_ACTIVE)
                {
                    ctcp.ThreadActive = THREAD_STOP;   /* tell thread to stop */

                    RELEASE_LOCK(ctcp.lock_);

                    /* close the socket */
                    try
                    {
                        ctcp.recv_sock.Close();
                    }
                    catch (SocketException ex)
                    {
                        sockerror = ex.ErrorCode;// CAPI_WSAGetLastError();
                    }

                    ctcp.recv_sock = null; /* clear it */

                    while (true)
                    {
                        REQUEST_LOCK(ctcp.lock_);

                        if (ctcp.ThreadActive == THREAD_TERMINATED)
                        {
                            break;
                        }

                        RELEASE_LOCK(ctcp.lock_);
                    }
                }
                else  /* not a listening socket , a regular socket */
                {
                    /* close the socket */
                    try
                    {
                        ctcp.recv_sock.Close();
                    }
                    catch (SocketException ex)
                    {
                        sockerror = ex.ErrorCode;//CAPI_WSAGetLastError();
                    }

                    ctcp.recv_sock = null; /* clear it */
                }
            }

            /* reset this to start of receive buffer so pointer to free() is correct */
            //ctcp.recv_buffer = ctcp.recv_buffer_start ;

            /* remove this ComHandle from the current list */
            enter_cs();

            GlobalListHead = CAPIListRemove(GlobalListHead, c);

            leave_cs();

            if (ctcp.lock_ != null)
            {
                RELEASE_LOCK(ctcp.lock_);

                DESTROY_LOCK(ctcp.lock_);
            }

            ctcp.lock_ = null;  /* clear it */

            if ((ctcp.handletype != HandleTypes.GROUP) && (ctcp.state != COMAPIStatus.COMAPI_STATE_CONNECTION_PENDING))
            {
                WS2Connections--;   /* decrement the INIT reference count */
            }

            /* free the data structs */
            //if (ctcp.recv_buffer)
            //{
            //    free(ctcp.recv_buffer);
            //}

            //if (ctcp.send_buffer)
            //{
            //    free(ctcp.send_buffer);
            //}

            //free(c);

            /* if No more connections then WSACleanup() */
#if TODO
            if (!WS2Connections)
            {
                if (sockerror = CAPI_WSACleanup())
                {
                    sockerror = CAPI_WSAGetLastError();
                }

#if LOAD_DLLS
                FreeLibrary(hWinSockDLL);
                hWinSockDLL = 0;
#endif

            }
#endif
        }

        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////

        static int ComTCPSendX(ComAPIHandle c, int msgsize, bool oob, int type, ComAPIHandle Xcom)
        {
            if (c == Xcom)
            {
                return 0;
            }
            else
            {
                return ComTCPSend(c, msgsize, oob, type);
            }
        }

        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////

        static int ComTCPSend(ComAPIHandle c, int msgsize, bool oob, int type)
        {
            if (c != null)
            {
                ComTCP ctcp = (ComTCP)c;
                SocketError senderror;
                int bytesSent = 0;

                enter_cs();

                if (CAPIListFindHandle(GlobalListHead, c) == null)
                {
                    leave_cs();

                    return -1 * (int)SocketError.NotSocket; /* is it in  our list ? */
                }

                leave_cs();

                if (ctcp.handletype == HandleTypes.LISTENER)
                {
                    return -1 * (int)SocketError.NotSocket;
                }

                if (ctcp.state == COMAPIStatus.COMAPI_STATE_CONNECTION_PENDING)
                {
                    return (int)ComAPIEnumErrorCodes.COMAPI_CONNECTION_PENDING;
                }

                if (msgsize > ctcp.buffer_size - (int)tcpHeader.Size)
                {
                    return (int)ComAPIEnumErrorCodes.COMAPI_MESSAGE_TOO_BIG;
                }

                /* add the header size to the requested messagesize */
                int send_buffer_len = msgsize + ctcp.headersize;
                /* create and store header in buffer */
                setHeader(ctcp.send_buffer, msgsize);

                try
                {
                    if (CApiBandwithControl.check_bandwidth(send_buffer_len, true, type))
                    {
                        bytesSent = ctcp.send_sock.Send(ctcp.send_buffer, ctcp.send_buffer_start, send_buffer_len, SocketFlags.None);
                        senderror = 0;
                    }
                    else
                    {
                        return (int)ComAPIEnumErrorCodes.COMAPI_WOULDBLOCK;
                    }
                }
                catch (SocketException ex)
                {

                    senderror = ex.SocketErrorCode;
                }
                if (senderror == SocketError.SocketError)
                {
                    //senderror = CAPI_WSAGetLastError();

                    if (senderror == SocketError.WouldBlock)
                    {
                        /* let's keep a count of wouldblocks - might be interesting later */

                        ctcp.sendwouldblockcount++;
                        return -1 * (int)SocketError.WouldBlock;
                    }

                    return -1 * (int)senderror;
                }
                CApiBandwithControl.use_bandwidth(bytesSent, 1, type);
#if checkbandwidth

                totalbwused += bytesSent;

                if (oob) oobbwused += bytesSent;

                if (now > laststatcound + 1000)
                {
                    MonoPrint("TCP Bytes pr sec %d, OOB %d",
                              (int)(totalbwused * 1000 / (now - laststatcound)),
                              (int)(oobbwused * 1000 / (now - laststatcound)));
                    laststatcound = now;
                    totalbwused = 0;
                    oobbwused = 0;
                    Posupdbwused = 0;
                }

#endif

                if (bytesSent != send_buffer_len)  /* Incomplete message was sent?? */
                {
                    ctcp.sendwouldblockcount++;
                    return -1 * (int)SocketError.WouldBlock;
                }
                else
                {
                    /* keep track of successful messages sent/received */
                    ctcp.sendmessagecount++;
                    return bytesSent - ctcp.headersize;
                }
            }
            else
            {
                return -1 * (int)SocketError.NotSocket;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////

        static int ComTCPSendBufferGet(ComAPIHandle c)
        {
            if (c != null)
            {
                enter_cs();

                if (CAPIListFindHandle(GlobalListHead, c) == null)
                {
                    leave_cs();

                    return -1; /* is it in  our list ? */
                }

                leave_cs();

                return ((ComTCP)c).send_buffer_start + tcpHeader.Size;
            }
            else
            {
                return -1;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////

        static int ComTCPRecvBufferGet(ComAPIHandle c)
        {
            if (c != null)
            {
                enter_cs();

                if (CAPIListFindHandle(GlobalListHead, c) == null)
                {
                    leave_cs();

                    return -1; /* is it in  our list ? */
                }

                leave_cs();

                if (((ComTCP)c).handletype == HandleTypes.GROUP)
                {
                    return -1;
                }

                return ((ComTCP)c).recv_buffer_start + tcpHeader.Size;
            }
            else
            {
                return -1;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////

        static uint ComTCPQuery(ComAPIHandle c, ComAPIQueryTypes querytype)
        {
            if (c != null)
            {
                if (((ComTCP)c).handletype == HandleTypes.GROUP)
                {
                    return 0;
                }

                switch (querytype)
                {
                    case ComAPIQueryTypes.COMAPI_MESSAGECOUNT:
                        return ((ComTCP)c).sendmessagecount + ((ComTCP)c).recvmessagecount;

                    case ComAPIQueryTypes.COMAPI_SEND_MESSAGECOUNT:
                        return ((ComTCP)c).sendmessagecount;

                    case ComAPIQueryTypes.COMAPI_RECV_MESSAGECOUNT:
                        return ((ComTCP)c).recvmessagecount;

                    case ComAPIQueryTypes.COMAPI_RECV_WOULDBLOCKCOUNT:
                        return ((ComTCP)c).recvwouldblockcount;

                    case ComAPIQueryTypes.COMAPI_SEND_WOULDBLOCKCOUNT:
                        return ((ComTCP)c).sendwouldblockcount;

                    case ComAPIQueryTypes.COMAPI_CONNECTION_ADDRESS:
                    case ComAPIQueryTypes.COMAPI_SENDER:
                        return ConvertAddrToUInt32(((ComTCP)c).Addr.Address);

                    case ComAPIQueryTypes.COMAPI_RECEIVE_SOCKET:
#if TODO
                        return ((ComTCP)c).recv_sock;
#endif
                        throw new NotImplementedException();

                    case ComAPIQueryTypes.COMAPI_SEND_SOCKET:
#if TODO
                        return ((ComTCP)c).send_sock;
#endif
                        throw new NotImplementedException();

                    case ComAPIQueryTypes.COMAPI_RELIABLE:
                        return 1;

                    case ComAPIQueryTypes.COMAPI_MAX_BUFFER_SIZE:
                        return 0;

                    case ComAPIQueryTypes.COMAPI_ACTUAL_BUFFER_SIZE:
                        return (uint)(((ComIP)c).buffer_size - tcpHeader.Size);

                    case ComAPIQueryTypes.COMAPI_PROTOCOL:
                        return (uint)c.protocol;

                    case ComAPIQueryTypes.COMAPI_STATE:
                        return (uint)((ComTCP)c).state;

                    case ComAPIQueryTypes.COMAPI_TCP_HEADER_OVERHEAD:
                        return tcpHeader.Size + 40; // Size of underlying header.

                    default:
                        return 0;
                }
            }

            return 0;
        }

        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////

        static int ComTCPRecv(ComAPIHandle c, int BytesToRecv)
        {
            if (c != null)
            {
                ComTCP ctcp = (ComTCP)c;
                int recverror;
                int bytesRecvd;
                // int flags = 0;

                /* precautionary test to protect buffers, since recv_buffer is updated in ComTCPGetNBytes() */
                if (ctcp.recv_buffer_start + BytesToRecv > ctcp.buffer_size)
                {
                    return (int)ComAPIEnumErrorCodes.COMAPI_OVERRUN_ERROR;
                }

                /* now perform the the receive */
                bytesRecvd = recverror = 0;
                try
                {
                    bytesRecvd = ctcp.recv_sock.Receive(ctcp.recv_buffer, ctcp.recv_buffer_start, BytesToRecv, SocketFlags.None);

                }
                catch (SocketException ex)
                {
                    recverror = ex.ErrorCode; // CAPI_WSAGetLastError();      /* error condition */
                }
                if (bytesRecvd == 0)
                {
                    recverror = (int)ComAPIEnumErrorCodes.COMAPI_CONNECTION_CLOSED;      /* graceful close is indicated */
                }


                if (recverror != 0)
                {
                    if (recverror == (int)SocketError.WouldBlock)     /* nothing ready */
                    {
                        recverror = 0;
                        ctcp.recvwouldblockcount++;
                    }
                    else if (recverror != (int)ComAPIEnumErrorCodes.COMAPI_CONNECTION_CLOSED)
                    {
                        recverror *= -1;                 /* Negate Winsock error code */
                    }

                    return recverror;
                }
                else
                {
                    return bytesRecvd;
                }
            }
            else
            {
                return 0;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////

        static int ComTCPGetMessage(ComAPIHandle c)
        {
            // int retval = 0;

            if (c != null)
            {
                ComTCP ctcp = (ComTCP)c;
                int bytesRecvd;

                if (ctcp.handletype == HandleTypes.GROUP)
                {
                    return -1 * (int)SocketError.NotSocket;
                }

                if (ctcp.state == COMAPIStatus.COMAPI_STATE_CONNECTION_PENDING)
                {
                    return (int)ComAPIEnumErrorCodes.COMAPI_CONNECTION_PENDING;
                }

                enter_cs();

                if (CAPIListFindHandle(GlobalListHead, c) == null)
                {
                    leave_cs();
                    return -1 * (int)SocketError.NotSocket; /* is it in  our list ? */
                }

                leave_cs();

                if (ctcp.handletype == HandleTypes.LISTENER)
                {
                    return -1 * (int)SocketError.NotSocket;
                }

                /* Make this thread safe */
                REQUEST_LOCK(ctcp.lock_);

                /* need to read a header for message size */
                if (ctcp.bytes_needed_for_header != 0)
                {
                    bytesRecvd = ComTCPGetNbytes(c, ctcp.bytes_needed_for_header);

                    if (bytesRecvd <= 0)     /* either no data (= 0) available or an error */
                    {
                        RELEASE_LOCK(ctcp.lock_);
                        return bytesRecvd;
                    }

                    ctcp.bytes_needed_for_header -= bytesRecvd;

                    if (ctcp.bytes_needed_for_header == 0)   /* we now have a complete header */
                    {
                        /*should have a header here for new message .. so extract size of message*/
                        ctcp.messagesize = isHeader(ctcp.recv_buffer, ctcp.recv_buffer_start);

                        if (ctcp.messagesize == 0)
                        {
                            RELEASE_LOCK(ctcp.lock_);
                            return (int)ComAPIEnumErrorCodes.COMAPI_BAD_HEADER;
                        }

                        /* now set these values for receiving the message data */
                        ctcp.bytes_needed_for_message = ctcp.messagesize;
                        ctcp.bytes_recvd_for_message = 0;
                    }
                    else  /* incomplete header .. try again later */
                    {
                        RELEASE_LOCK(ctcp.lock_);
                        return 0;
                    }
                }

                /* Do we need to receive  message data ? */
                if (ctcp.bytes_needed_for_message != 0)
                {
                    if (ctcp.bytes_needed_for_message > ctcp.buffer_size - (int)tcpHeader.Size)
                    {
                        RELEASE_LOCK(ctcp.lock_);
                        return (int)ComAPIEnumErrorCodes.COMAPI_MESSAGE_TOO_BIG;
                    }

                    bytesRecvd = ComTCPGetNbytes(c, ctcp.bytes_needed_for_message);

                    if (bytesRecvd <= 0)     /* either no data (= 0) available or an error */
                    {
                        RELEASE_LOCK(ctcp.lock_);
                        return bytesRecvd;
                    }

                    ctcp.bytes_needed_for_message -= bytesRecvd;
                    ctcp.bytes_recvd_for_message += bytesRecvd;

                    if (ctcp.bytes_needed_for_message == 0)   /* we now have a complete message */
                    {
                        /* a PANIC Check */
                        if (ctcp.messagesize != ctcp.bytes_recvd_for_message)
                        {
                            RELEASE_LOCK(ctcp.lock_);
                            return (int)ComAPIEnumErrorCodes.COMAPI_BAD_MESSAGE_SIZE;
                        }

                        /* Reset for next message */
                        ctcp.bytes_needed_for_header = ctcp.headersize;
                        ctcp.bytes_needed_for_message = 0;
                        ctcp.bytes_recvd_for_message = 0;
                        ctcp.recv_buffer_start = 0;
                        ctcp.recvmessagecount++;    /* global message counter */
                        RELEASE_LOCK(ctcp.lock_);

                        if (CAPI_TimeStamp != null)
                        {
                            ctcp.timestamp = CAPI_TimeStamp();
                        }

                        return ctcp.messagesize;
                    }
                    else     /* incomplete message */
                    {
                        RELEASE_LOCK(ctcp.lock_);
                        return 0;   // complete message not avaliable yet
                    }
                }

                RELEASE_LOCK(ctcp.lock_);

                return 0;   /* message not avaliable yet */
            }

            return 0;
        }

        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////

        static void initComTCP(ComTCP c)
        {
            c.headersize = tcpHeader.Size;
            c.bytes_needed_for_header = c.headersize;
            c.bytes_needed_for_message = 0;

            return;
        }

        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////

        static int isHeader(byte[] data, int pos)
        {
            int ret = 0;

            if (data != null)
            {
                tcpHeader header = tcpHeader.Decode(data, ref pos);
                ret = 1;

                if (header.header_base != tcpHeader.HEADER_BASE)
                {
                    ret = 0;
                }
                else
                {
                    ret = ((header.inv_size ^ header.size) == 0xFFFF) ? header.size : 0;
                }
            }

            return ret;
        }

        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////

        static void setHeader(byte[] data, int size)
        {
            tcpHeader header;
            if (data != null)
            {
                int pos = 0;
                header = tcpHeader.Decode(data, ref pos);
                header.header_base = tcpHeader.HEADER_BASE;
                header.size = (ushort)size;
                header.inv_size = (ushort)(~size);
            }
        }

        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////

        static int ComTCPGetNbytes(ComAPIHandle c, int BytesToGet)
        {
            ComTCP ctcp = (ComTCP)c;
            int bytesRecvd = 1;
            int BytesGotten = 0;

            if (c != null)
            {
                // while(BytesGotten < BytesToGet && bytesRecvd > 0)
                while (BytesToGet != 0 && bytesRecvd > 0)
                {
                    bytesRecvd = ComTCPRecv(c, BytesToGet);

                    /* If bytesRecvd == 0 means nothing to get .. so quit */
                    if (bytesRecvd > 0)
                    {
                        BytesGotten += bytesRecvd;
                        BytesToGet -= bytesRecvd;
                        ctcp.recv_buffer_start += bytesRecvd;  /* move the receive buffer start point along */
                    }
                    else
                    {
                        if (bytesRecvd < 0)
                        {
                            BytesGotten = bytesRecvd; /*negative error back  thru BytesGotten */
                        }
                    }
                }
            }

            return BytesGotten;
        }

        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////

        static void ComTCPFreeData(ComTCP c)
        {
            if (c != null)
            {
                leave_cs();

                GlobalListHead = CAPIListRemove(GlobalListHead, (ComAPIHandle)c);

                leave_cs();

                /* free the data structs */
                //if (c.recv_buffer)
                //{
                //    free(c.recv_buffer);
                //}

                //if (c.send_buffer)
                //{
                //    free(c.send_buffer);
                //}

                //free(c);
            }
        }

        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////

        static int CAPIListCount(CAPIList list)
        {
            CAPIList curr;
            int i;

            if (list == null)
            {
                return 0;
            }

            i = 0;
            curr = list;

            while (curr != null)
            {
                i++;
                curr = curr.next;
            }

            return i;
        }

        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////

        static CAPIList CAPIListAppend(CAPIList list)
        {
            CAPIList newnode;

            newnode = new CAPIList();
            //memset(newnode, 0, sizeof(CAPIList));
            newnode.com = null;
            newnode.next = list;

            MonoPrint("%08x = CAPI List Append %08x\n", newnode, list);
            return newnode;
        }

        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////

        static CAPIList CAPIListRemove(CAPIList list, ComAPIHandle com)
        {
            CAPIList start, prev, curr;

            start = list;

            if (list == null)
            {
                return null;
            }

            while (true)
            {
                prev = null;
                curr = list;

                while ((curr != null) && (curr.com != com))
                {
                    prev = curr;
                    curr = curr.next;
                }

                /* not found, return list unmodified */
                if (curr == null)
                {
#if DEBUG
                    // if (list)
                    // MonoPrint ("%08x = CAPI List Remove %08x \"%s\" %08x\n", list, start, com.name, ((ComIP)com).address.sin_addr.s_addr);
#endif

                    return list;
                }

                /* found at head */
                if (prev == null)
                {
                    list = list.next;
                }
                else
                {
                    prev.next = curr.next;
                }

                curr.next = null;

                //free(curr);
            }
        }

        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////

        static CAPIList CAPIListFindHandle(CAPIList list, ComAPIHandle com)
        {
            CAPIList curr;

            for (curr = list; curr != null; curr = curr.next)
            {
                if (curr.com == com)
                {
                    return curr;
                }
            }

            return null;
        }

        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////

        static CAPIList CAPIListFindTCPListenPort(CAPIList list, short port)
        {
            CAPIList curr;

            if (port == 0)
            {
                return null;
            }

            for (curr = list; curr != null; curr = curr.next)
            {
                if (curr.com.protocol == ComAPIEnumProtocols.CAPI_TCP_PROTOCOL)
                {
                    if ((((ComTCP)(curr.com)).ListenPort == port) && (((ComTCP)(curr.com)).handletype == HandleTypes.LISTENER))
                    {
                        return curr;
                    }
                }
            }

            return null;
        }

        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////

        static CAPIList CAPIListFindTCPIPaddress(CAPIList list, IPEndPoint IPaddress)
        {
            CAPIList curr;
            ComTCP c;

            if (IPaddress == null)
            {
                return null;
            }

            for (curr = list; curr != null; curr = curr.next)
            {
                if (curr.com.protocol == ComAPIEnumProtocols.CAPI_TCP_PROTOCOL)
                {
                    IPEndPoint cip;

                    c = (ComTCP)curr.com;
                    cip = c.Addr;

                    if (cip == IPaddress)
                    {
                        return curr;
                    }
                }
            }

            return null;
        }

        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////

        static CAPIList CAPIListFindTCPAcceptPendingExpired(CAPIList list)
        {
            CAPIList curr;
            ComTCP c;

            for (curr = list; curr != null; curr = curr.next)
            {
                if (curr.com.protocol == ComAPIEnumProtocols.CAPI_TCP_PROTOCOL)
                {
                    c = (ComTCP)curr.com;

                    if ((c.state == COMAPIStatus.COMAPI_STATE_CONNECTION_PENDING) && (c.timeoutsecs != 0))
                    {
                        if (c.timeoutsecs <= ((DateTime.Now.Ticks - c.bytes_needed_for_message) / 1000))
                        {
                            c.timeoutsecs = 0;

                            return curr;
                        }
                    }
                }
            }

            return null;
        }

        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////

        delegate void DestructorDelegate(ComAPIHandle handle);
        static void CAPIListDestroy(CAPIList list, DestructorDelegate destructor)
        {
            CAPIList prev, curr;

            if (list == null)
            {
                return;
            }

            prev = list;
            curr = list.next;

            while (curr != null)
            {
                if (destructor != null)
                {
                    destructor(prev.com);
                }

                prev.next = null;

                //free(prev);

                prev = curr;
                curr = curr.next;
            }

            if (destructor != null)
            {
                destructor(prev.com);
            }

            prev.next = null;

            //free(prev);
        }

        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////

        static CAPIList CAPIListAppendTail(CAPIList list)
        {
            CAPIList newnode;

            newnode = new CAPIList();
            //memset(newnode, 0, sizeof(CAPIList));

            /* list was null */
            if (list == null)
            {
                list = newnode;
            }
            else
            {
                list.next = newnode;
            }

            MonoPrint("%08x = CAPI List Append Tail %08x\n", newnode, list);
            return newnode;
        }

        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////

        static int ComGROUPRecvBufferGet(ComAPIHandle c)
        {
#if TODO
            c;
#endif

            return -1;
        }

        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////


        static int ComGROUPGet(ComAPIHandle c)
        {
#if TODO
            c;
#endif

            return 0;
        }

        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////

        // JPO -always called from within the CS
        static void ComGROUPClose(ComAPIHandle c)
        {
            CAPIList curr;

            if (c == null)
            {
                return;
            }

            if (CAPIListFindHandle(GlobalGroupListHead, c) == null)         /* in our list of groups ?*/
            {
                return;
            }

            CAPIListDestroy(((ComGROUP)c).GroupHead, null);               /* destroy this group list */

#if DEBUG
            MonoPrint("Group Close CH:\"%s\"\n", c.name);
#endif
            GlobalGroupListHead = CAPIListRemove(GlobalGroupListHead, c);    /* remove group from list of groups */

#if DEBUG
            MonoPrint("================================\n");
            MonoPrint("GlobalGroupListHead\n");

            curr = GlobalGroupListHead;

            while (curr != null)
            {
#if TODO
                if (!F4IsBadReadPtrC(curr.com.name, 1)) // JB 010724 CTD
                    MonoPrint("  \"%s\"\n", curr.com.name);
#endif
                curr = curr.next;
            }

            MonoPrint("================================\n");
#endif

            for (curr = GlobalGroupListHead; curr != null; curr = curr.next)
            {
                ComAPIDeleteFromGroup(curr.com, c);                             /* remove this group from others */
            }

            if (((ComGROUP)c).send_buffer != null)
            {
                //free(((ComGROUP)c).send_buffer);
            }

            //free(c);

            return;
        }

        private static void MonoPrint(string format, params object[] args)
        {
            log.DebugFormat(format, args);
        }

        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////

        static uint ComGROUPQuery(ComAPIHandle c, ComAPIQueryTypes querytype)
        {
            if (c != null)
            {
                switch (querytype)
                {
                    case ComAPIQueryTypes.COMAPI_PROTOCOL:
                        return (uint)c.protocol;

                    case ComAPIQueryTypes.COMAPI_ACTUAL_BUFFER_SIZE:
                        return (uint)(((ComGROUP)c).buffer_size - ((ComGROUP)c).max_header);

                    default:
                        return 0;
                }
            }

            return 0;
        }

        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////

        static ComAPIEnumErrorCodes ComGROUPAddress(ComAPIHandle c, out int ipaddr, bool reset)
        {
            ipaddr = -1;

            if (c != null)
            {
                ipaddr = ((ComGROUP)c).HostID;

                if (ipaddr >= 0)
                {
                    return 0;
                }
            }

            return ComAPIEnumErrorCodes.COMAPI_HOSTID_ERROR;
        }

        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////

        /* get the associated write buffer */

        static int ComGROUPSendBufferGet(ComAPIHandle c)
        {
            if (c != null)
            {
                if (c.protocol != ComAPIEnumProtocols.CAPI_GROUP_PROTOCOL)
                {
                    return -1;
                }

                return (((ComGROUP)c).send_buffer_start + ((ComGROUP)c).max_header);
            }
            else
            {
                return -1;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////

        static int ComGROUPSend(ComAPIHandle c, int msgsize, bool oob, int type)
        {
            ComAPIEnumErrorCodes senderror = 0;
            int bytesSent = 0;

            // JPO added a bunch of asserts after crashes occured in here.
            if (c != null)
            {
                ComGROUP group = (ComGROUP)c;
                CAPIList curr;
                int ret = 0;
                byte[] save_send_buffer;
                int save_send_buffer_start;
                enter_cs(); // JPO

                if (CAPIListFindHandle(GlobalGroupListHead, c) == null)
                {
                    leave_cs();
                    return (int)ComAPIEnumErrorCodes.COMAPI_NOTAGROUP; /* is it in  our list ? */
                }

                if (group.GroupHead == null)
                {
                    senderror = ComAPIEnumErrorCodes.COMAPI_EMPTYGROUP;
                }
                else
                {
                    senderror = 0;
                }

                /* proceed thru list and call send_function() for each connection */
                for (curr = group.GroupHead; curr != null; curr = curr.next)
                {

                    // @todo sfr: remove hack
#if TODO
                    if (F4IsBadReadPtrC(curr.com, sizeof(ComAPIHandle)))
                    {
                        // JB 010221 CTD
                        continue; // JB 010221 CTD
                    }
#endif

                    if (curr.com.protocol == ComAPIEnumProtocols.CAPI_TCP_PROTOCOL)
                    {
                        ComTCP this_ctcp;
                        this_ctcp = (ComTCP)curr.com;

                        //if(this_ctcp)
#if TODO
                        if (this_ctcp && !IsBadCodePtr((FARPROC)(*curr.com.send_func)))  // JB 010401 CTD
#else
                        if (this_ctcp != null)  // JB 010401 CTD
#endif
                        {
                            save_send_buffer = this_ctcp.send_buffer;
                            save_send_buffer_start = this_ctcp.send_buffer_start;
                            this_ctcp.send_buffer = group.send_buffer;
                            this_ctcp.send_buffer_start = group.send_buffer_start + group.TCP_buffer_shift;

                            if (curr.com.send_func != null)
                            {
                                ret = curr.com.send_func(curr.com, msgsize, oob, type);
                            }

                            this_ctcp.send_buffer = save_send_buffer;
                            this_ctcp.send_buffer_start = save_send_buffer_start;
                        }
                    }
                    else if (curr.com.protocol == ComAPIEnumProtocols.CAPI_UDP_PROTOCOL)
                    {
                        ComIP this_cudp;
                        this_cudp = (ComIP)curr.com;

                        //if(this_cudp) // JB 010222 CTD
#if TODO
                        if (
                            this_cudp && (this_cudp.send_buffer) &&
                            !F4IsBadReadPtrC(this_cudp.send_buffer, sizeof(char)) && // JB 010222 CTD
                            !F4IsBadCodePtrC((FARPROC)(*curr.com.send_func))
                        // JB 010401 CTD
                        )
#else
                        if (this_cudp != null && this_cudp.send_buffer != null)
#endif
                        {
                            save_send_buffer = this_cudp.send_buffer;
                            save_send_buffer_start = this_cudp.send_buffer_start;
                            this_cudp.send_buffer = group.send_buffer;
                            this_cudp.send_buffer_start = group.send_buffer_start + group.UDP_buffer_shift;

                            //memcpy(this_cudp.send_buffer, save_send_buffer, ComAPIHeader.Size);
                            Array.Copy(this_cudp.send_buffer, this_cudp.send_buffer_start, save_send_buffer, save_send_buffer_start, ComAPIHeader.Size);

                            if (curr.com.send_func != null)
                            {
                                ret = curr.com.send_func(curr.com, msgsize, oob, type);

                                if (ret > 0)
                                {
                                    // oob = true; // force other UDP packets to go OOB if the first one went
                                }
                            }

                            this_cudp.send_buffer = save_send_buffer;
                            this_cudp.send_buffer_start = save_send_buffer_start;
                        }
                    }
                    else if (curr.com.protocol == ComAPIEnumProtocols.CAPI_RUDP_PROTOCOL)
                    {
                        ComIP this_cudp;

                        this_cudp = (ComIP)curr.com;

                        //if(this_cudp)
#if TODO
                        if (this_cudp && !IsBadCodePtr((FARPROC)(*curr.com.send_func)))  // JB 010401 CTD
#else
                        if (this_cudp != null)
#endif
                        {
                            save_send_buffer = this_cudp.send_buffer;
                            save_send_buffer_start = this_cudp.send_buffer_start;
                            this_cudp.send_buffer = group.send_buffer;
                            this_cudp.send_buffer_start = group.send_buffer_start + group.RUDP_buffer_shift;

                            if (curr.com.send_func != null)
                            {
                                ret = curr.com.send_func(curr.com, msgsize, oob, type);
                            }

                            this_cudp.send_buffer = save_send_buffer;
                            this_cudp.send_buffer_start = save_send_buffer_start;
                        }
                    }
                    /*else if
                    (
                     (curr.com.protocol == ComAPIEnumProtocols.CAPI_DPLAY_TCP_PROTOCOL) ||
                     (curr.com.protocol == ComAPIEnumProtocols.CAPI_DPLAY_MODEM_PROTOCOL) ||
                     (curr.com.protocol == ComAPIEnumProtocols.CAPI_DPLAY_SERIAL_PROTOCOL)
                    )
                    {
                     ret = ComDPLAYSendFromGroup(curr.com,msgsize,group.send_buffer +group.max_header);
                    }*/
                    else if (curr.com.protocol == ComAPIEnumProtocols.CAPI_GROUP_PROTOCOL) /* another group */
                    {
                        ComGROUP this_group;
                        this_group = (ComGROUP)curr.com;

                        //if(this_group)
#if TODO
                        if (this_group && !F4IsBadCodePtrC((FARPROC)(*curr.com.send_func))) // JB 010401 CTD
#else
                        if (this_group != null) // JB 010401 CTD
#endif
                        {
                            save_send_buffer = this_group.send_buffer;
                            save_send_buffer_start = this_group.send_buffer_start;
                            this_group.send_buffer = group.send_buffer;
                            this_group.send_buffer_start = group.send_buffer_start;

                            if (curr.com.send_func != null)
                            {
                                ret = curr.com.send_func(curr.com, msgsize, false, type);
                            }

                            this_group.send_buffer = save_send_buffer;
                            this_group.send_buffer_start = save_send_buffer_start;
                        }
                    }
                    else
                    {
                        //me123 hack hack
                        if (bytesSent == 0) ret = 1;
                    }

                    if (ret >= 0)
                    {
                        bytesSent += ret;
                    }
                    else
                    {
                        senderror = (ComAPIEnumErrorCodes)ret;
                    }
                }
            }
            else
            {
                senderror = ComAPIEnumErrorCodes.COMAPI_EMPTYGROUP;
            }

            leave_cs();

            if (senderror != 0)
            {
                return (int)senderror;
            }
            else
            {
                return bytesSent;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////

        static int ComGROUPSendX(ComAPIHandle c, int msgsize, bool oob, int type, ComAPIHandle Xcom)
        {
            ComAPIEnumErrorCodes senderror = 0;
            int bytesSent = 0;

            if (c != null)
            {
                ComGROUP group = (ComGROUP)c;
                CAPIList curr;
                int ret = 0;
                byte[] save_send_buffer;
                int save_send_buffer_start;
                enter_cs();

                if (CAPIListFindHandle(GlobalGroupListHead, c) == null)
                {
                    leave_cs();
                    return (int)ComAPIEnumErrorCodes.COMAPI_NOTAGROUP; /* is it in  our list ? */
                }

                if (group.GroupHead == null)
                {
                    senderror = ComAPIEnumErrorCodes.COMAPI_EMPTYGROUP;
                }
                else
                {
                    senderror = 0;
                }

                /* proceed thru list and call send_function() for each connection */
                for (curr = group.GroupHead; curr != null; curr = curr.next)
                {
                    if (curr.com == Xcom)
                    {
                        continue;
                    }

                    if (curr.com.protocol == ComAPIEnumProtocols.CAPI_TCP_PROTOCOL)
                    {
                        ComTCP this_ctcp;

                        this_ctcp = (ComTCP)curr.com;

                        if (this_ctcp != null)
                        {
                            save_send_buffer = this_ctcp.send_buffer;
                            save_send_buffer_start = this_ctcp.send_buffer_start;
                            this_ctcp.send_buffer = group.send_buffer;
                            this_ctcp.send_buffer_start = group.send_buffer_start + group.TCP_buffer_shift;

                            if (curr.com.send_func != null)
                            {
                                ret = curr.com.send_func(curr.com, msgsize, false, type);
                            }

                            this_ctcp.send_buffer = save_send_buffer;
                            this_ctcp.send_buffer_start = save_send_buffer_start;

                        }
                    }
                    else if (curr.com.protocol == ComAPIEnumProtocols.CAPI_UDP_PROTOCOL)
                    {
                        ComIP this_cudp;

                        this_cudp = curr.com as ComIP;

                        if (this_cudp != null)
                        {
                            save_send_buffer = this_cudp.send_buffer;
                            save_send_buffer_start = this_cudp.send_buffer_start;
                            this_cudp.send_buffer = group.send_buffer;
                            this_cudp.send_buffer_start = group.send_buffer_start + group.UDP_buffer_shift;
                            //memcpy(this_cudp.send_buffer, save_send_buffer, ComAPIHeader.Size);
                            Array.Copy(this_cudp.send_buffer, this_cudp.send_buffer_start, save_send_buffer, save_send_buffer_start, ComAPIHeader.Size);
                            if (curr.com.send_func != null)
                            {
                                ret = curr.com.send_func(curr.com, msgsize, false, type);
                            }

                            this_cudp.send_buffer = save_send_buffer;
                            this_cudp.send_buffer_start = save_send_buffer_start;
                        }
                    }
                    else if (curr.com.protocol == ComAPIEnumProtocols.CAPI_RUDP_PROTOCOL)
                    {
                        ComIP this_cudp;

                        this_cudp = (ComIP)curr.com;

                        if (this_cudp != null)
                        {
                            save_send_buffer = this_cudp.send_buffer;
                            save_send_buffer_start = this_cudp.send_buffer_start;
                            this_cudp.send_buffer = group.send_buffer;
                            this_cudp.send_buffer_start = group.send_buffer_start + group.RUDP_buffer_shift;

                            if (curr.com.send_func != null)
                            {
                                ret = curr.com.send_func(curr.com, msgsize, false, type);
                            }

                            this_cudp.send_buffer = save_send_buffer;
                            this_cudp.send_buffer_start = save_send_buffer_start;
                        }
                    }
                    /*else if
                    (
                     curr.com.protocol == ComAPIEnumProtocols.CAPI_DPLAY_TCP_PROTOCOL ||
                     curr.com.protocol == ComAPIEnumProtocols.CAPI_DPLAY_MODEM_PROTOCOL ||
                     curr.com.protocol == ComAPIEnumProtocols.CAPI_DPLAY_SERIAL_PROTOCOL
                    )
                    {
                     ret = ComDPLAYSendFromGroup(curr.com,msgsize,group.send_buffer +group.max_header);
                    }*/
                    else if (curr.com.protocol == ComAPIEnumProtocols.CAPI_GROUP_PROTOCOL) /* another group */
                    {
                        ComGROUP this_group;
                        this_group = (ComGROUP)curr.com;

                        if (this_group != null)
                        {
                            save_send_buffer = this_group.send_buffer;
                            save_send_buffer_start = this_group.send_buffer_start;
                            this_group.send_buffer = group.send_buffer;
                            this_group.send_buffer_start = group.send_buffer_start;

                            if (curr.com.send_func != null)
                            {
                                ret = curr.com.send_func(curr.com, msgsize, false, type);
                            }

                            this_group.send_buffer = save_send_buffer;
                            this_group.send_buffer_start = save_send_buffer_start;
                        }
                    }

                    if (ret >= 0)
                    {
                        bytesSent += ret;
                    }
                    else
                    {
                        senderror = (ComAPIEnumErrorCodes)ret;
                    }
                }
            }
            else
            {
                senderror = ComAPIEnumErrorCodes.COMAPI_EMPTYGROUP;
            }

            leave_cs();

            if (senderror != 0)
            {
                return (int)senderror;
            }
            else
            {
                return bytesSent;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////

        public static ComAPIHandle ComAPICreateGroup(string name, int messagesize, params ComAPIHandle[] vptr)
        {
            ComGROUP group;
            int buffersize;
            int count = 0;
            int HostID = 0;
            CAPIList curr = null;

            enter_cs();

            buffersize = messagesize;

            foreach (ComAPIHandle CH in vptr)
            {
                count++;
                switch (CH.protocol)
                {
                    case ComAPIEnumProtocols.CAPI_UDP_PROTOCOL:
                        {
                            MonoPrint("ComAPICreateGroup UDP\n");

                            if (buffersize == 0)
                            {
                                buffersize = ((ComIP)CH).buffer_size;
                            }
                            else
                            {
                                buffersize = Math.Min(buffersize, (int)CH.query_func(CH, ComAPIQueryTypes.COMAPI_ACTUAL_BUFFER_SIZE));
                            }

                            if (HostID == 0)
                            {
                                CH.addr_func(CH, out HostID, false);
                            }

                            break;
                        }

                    case ComAPIEnumProtocols.CAPI_TCP_PROTOCOL:
                        {
                            MonoPrint("ComAPICreateGroup RUDP\n");

                            if (buffersize == 0)
                            {
                                buffersize = ((ComTCP)CH).buffer_size;
                            }
                            else
                            {
                                buffersize = Math.Min(buffersize, (int)CH.query_func(CH, ComAPIQueryTypes.COMAPI_ACTUAL_BUFFER_SIZE));
                            }

                            if (HostID == 0)
                            {
                                CH.addr_func(CH, out HostID, false);
                            }

                            break;
                        }

                    /* a trick -- use COMIP here/below since top of ComDPLAY struct is same as ComIP */
                    case ComAPIEnumProtocols.CAPI_DPLAY_SERIAL_PROTOCOL:
                    case ComAPIEnumProtocols.CAPI_DPLAY_MODEM_PROTOCOL:
                        {
                            MonoPrint("ComAPICreateGroup Serial\n");

                            if (buffersize == 0)
                            {
                                buffersize = ((ComIP)CH).buffer_size;
                            }
                            else
                            {
                                buffersize = Math.Min(buffersize, (int)CH.query_func(CH, ComAPIQueryTypes.COMAPI_ACTUAL_BUFFER_SIZE));
                            }

                            if (HostID == 0)
                            {
                                CH.addr_func(CH, out HostID, false);
                            }

                            break;
                        }

                    default:
                        {
                            break;
                        }
                }
            }

            if (count == 0)
            {
                buffersize = messagesize;
            }

            if (buffersize == 0)
            {
                leave_cs();

                return null;
            }

            MonoPrint("ComAPICreateGroup CAPIListAppend GlobalGroupListHead\n");
            GlobalGroupListHead = CAPIListAppend(GlobalGroupListHead);

            if (GlobalGroupListHead == null)
            {
                leave_cs();

                return null;
            }

            /* allocate a new ComHandle struct */
            GlobalGroupListHead.com = new ComGROUP();
            //memset(GlobalGroupListHead.com, 0, sizeof(ComGROUP));
            GlobalGroupListHead.com.name = name;
            group = (ComGROUP)(GlobalGroupListHead.com);
            group.HostID = HostID;

#if DEBUG
            MonoPrint("================================\n");
            MonoPrint("GlobalGroupListHead\n");
            curr = GlobalGroupListHead;

            while (curr != null)
            {
#if TODO
                if (!F4IsBadReadPtrC(curr.com.name, 1)) // JB 010724 CTD
                    MonoPrint("  \"%s\"\n", curr.com.name);
#endif
                curr = curr.next;
            }

            MonoPrint("================================\n");

            MonoPrint("ComAPICreateGroup Created ComGroup \"%s\"\n", GlobalGroupListHead.com.name);
            MonoPrint("ComAPICreateGroup Appended GGLH%08x CH:\"%s\" IP%08x\n", GlobalGroupListHead, GlobalGroupListHead.com.name, HostID);
#endif

            /* initialize header data */

            group.GroupHead = null;

            group.apiheader.protocol = ComAPIEnumProtocols.CAPI_GROUP_PROTOCOL;

            group.apiheader.recv_func = ComGROUPGet;
            group.apiheader.recv_buf_func = ComGROUPRecvBufferGet;
            group.apiheader.addr_func = ComGROUPAddress;
            group.apiheader.query_func = ComGROUPQuery;
            group.apiheader.close_func = ComGROUPClose;
            group.apiheader.send_buf_func = ComGROUPSendBufferGet;
            group.apiheader.send_func = ComGROUPSend;
            group.apiheader.sendX_func = ComGROUPSendX;
            group.apiheader.get_timestamp_func = ComTCPGetTimeStamp;

            group.max_header = Math.Max(tcpHeader.Size, ComAPIHeader.Size);
            group.buffer_size = buffersize + group.max_header;
            group.send_buffer = new byte[group.buffer_size];
            group.send_buffer_start = 0;
            group.TCP_buffer_shift = (sbyte)(group.max_header - tcpHeader.Size);
            group.UDP_buffer_shift = (sbyte)(group.max_header - ComAPIHeader.Size);
            group.RUDP_buffer_shift = (sbyte)group.max_header;
            group.DPLAY_buffer_shift = 0;

            leave_cs();

            return (ComAPIHandle)group;
        }

        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////

        public static IPAddress ComAPIGetNetHostByHandle(ComAPIHandle c)
        {
            ComTCP ctcp;
            ComIP cudp;
            IPAddress ip = null;

            if (c.protocol == ComAPIEnumProtocols.CAPI_TCP_PROTOCOL)
            {
                ctcp = (ComTCP)c;
                ip = ComAPIGetNetHostBySocket(ctcp.recv_sock);
            }
            else if (c.protocol == ComAPIEnumProtocols.CAPI_TCP_PROTOCOL)
            {
                cudp = (ComIP)c;
                ip = ComAPIGetNetHostBySocket(cudp.recv_sock);
            }

            return ip;
        }

        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////

        static uint ComTCPGetTimeStamp(ComAPIHandle c)
        {
            if (c != null)
            {
                ComTCP ctcp = (ComTCP)c;

                return ctcp.timestamp;
            }
            else
            {
                return 0;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////
        #endregion

    }
}
