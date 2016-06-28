using FalconNet.Common.Encoding;
using FalconNet.Common.Threading;
using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FalconNet.VU.Comms
{
    /* COMAPI Error codes  to return to application */
    public enum ComAPIEnumErrorCodes
    {
        COMAPI_OK = 0,
        COMAPI_BAD_HEADER = -1,  /* COMAPI message header not correct */
        COMAPI_OUT_OF_SYNC = -2,  /* data not quite as we expected */
        COMAPI_OVERRUN_ERROR = -3,  /* trying to read too much */
        COMAPI_BAD_MESSAGE_SIZE = -4,  /* internal syncing error */
        COMAPI_CONNECTION_CLOSED = -5,  /* remote connection closed gracefully */
        COMAPI_MESSAGE_TOO_BIG = -6,
        COMAPI_CONNECTION_PENDING = -7,
        COMAPI_WOULDBLOCK = -8,
        COMAPI_EMPTYGROUP = -9,
        COMAPI_PLAYER_LEFT = -10,
        COMAPI_NOTAGROUP = -11,
        COMAPI_CONNECTION_TIMEOUT = -12,
        COMAPI_HOSTID_ERROR = -13,
        COMAPI_WINSOCKDLL_ERROR = -14,     /* WS2_32.DLL */
        COMAPI_DPLAYDLL_ERROR = -15,     /* DPLAYX.DLL */
        COMAPI_OLE32DLL_ERROR = -16,     /* OLE32.DLL   for DirectPLay */
        COMAPI_TENDLL_ERROR = -17,
        WSAENOTSOCK = -10038,
    }

    public enum ComAPIEnumProtocols
    {
        CAPI_UNKNOWN_PROTOCOL = 0,
        CAPI_UDP_PROTOCOL = 1,
        CAPI_IP_MULTICAST_PROTOCOL = 2,
        CAPI_SERIAL_PROTOCOL = 3,
        CAPI_TEN_PROTOCOL = 4,
        CAPI_TCP_PROTOCOL = 5,
        CAPI_DPLAY_MODEM_PROTOCOL = 6,
        CAPI_DPLAY_SERIAL_PROTOCOL = 7,
        CAPI_DPLAY_TCP_PROTOCOL = 8,
        CAPI_DPLAY_IPX_PROTOCOL = 9,
        CAPI_RUDP_PROTOCOL = 10,
        CAPI_GROUP_PROTOCOL = 11
    }

    public enum ComAPIQueryTypes
    {
        COMAPI_MESSAGECOUNT = 1,
        COMAPI_RECV_WOULDBLOCKCOUNT = 2,
        COMAPI_SEND_WOULDBLOCKCOUNT = 3,
        COMAPI_CONNECTION_ADDRESS = 4,
        COMAPI_RECEIVE_SOCKET = 5,
        COMAPI_SEND_SOCKET = 6,
        COMAPI_RELIABLE = 7,
        COMAPI_RECV_MESSAGECOUNT = 8,
        COMAPI_SEND_MESSAGECOUNT = 9,
        COMAPI_UDP_CACHE_SIZE = 10,
        COMAPI_RUDP_CACHE_SIZE = 12,
        COMAPI_MAX_BUFFER_SIZE = 13,   /* 0 = no MAXIMUM =  stream */
        COMAPI_ACTUAL_BUFFER_SIZE = 14,
        COMAPI_PROTOCOL = 15,
        COMAPI_STATE = 16,
        COMAPI_DPLAY_PLAYERID = 17,
        COMAPI_DPLAY_REMOTEPLAYERID = 18,
        COMAPI_SENDER = 19,
        COMAPI_TCP_HEADER_OVERHEAD = 20,
        COMAPI_UDP_HEADER_OVERHEAD = 21,
        COMAPI_RUDP_HEADER_OVERHEAD = 22,
        COMAPI_PING_TIME = 23,
        COMAPI_BYTES_PENDING = 24,
        COMAPI_SENDER_PORT = 25,//< sfr: converts port information
        COMAPI_ID = 26, //< sfr: id of sender
    }

    public enum COMAPIStatus : short
    {
        COMAPI_STATE_CONNECTION_PENDING = 0,
        COMAPI_STATE_CONNECTED = 1,
        COMAPI_STATE_ACCEPTED = 2
    }

    public static partial class CommApi
    {
        public delegate void AcceptCallback(ComAPIHandle c, int retcode);
        public delegate void ConnectCallback(ComAPIHandle c, int retcode);
        public delegate void RegisterInfoCallback(ComAPIHandle c, int send, int msgsize);

        // my address information
        public static int myReliableRecvPort = 0;
        public static int myRecvPort = 0;


        public static int ComAPIInitComms()
        {
            int ret = 1;

            if (WS2Connections == 0)
            {
#if TODO
              WSADATA wsaData;
              ret = InitWS2(&wsaData);
#endif
                WS2Connections--;

                /* if No more connections then WSACleanup() */
                if (WS2Connections == 0)
                {
#if TODO
                   CAPI_WSACleanup();
#endif
                }
            }

            return ret;
        }

        public static void ComAPISetName(ComAPIHandle c, string name)
        {
            c.name = name;
        }

        //sfr: all these functions receive and return in host order
        /** sets comm ports (best effort and reliable) */
        public static void ComAPISetLocalPorts(int b, int r)
        {
            myRecvPort = IPAddress.HostToNetworkOrder(b);
            myReliableRecvPort = IPAddress.HostToNetworkOrder(r);
        }


        public static IPAddress ComAPIGetPeerIP(ComIP c)
        {
            // we send to this address, so this is his IP
            //return IPAddress.NetworkToHostOrder(((ComIP*)c).sendAddress.sin_addr.S_un.S_addr);
            return ((IPEndPoint)c.send_sock.RemoteEndPoint).Address;
        }

        public static int ComAPIGetRecvPort(ComIP c)
        {
            // this is the same for all coms of same type
            //return IPAddress.NetworkToHostOrder(c.recAddress.sin_port);
            return ((IPEndPoint)c.send_sock.LocalEndPoint).Port;
        }

        public static int ComAPIGetPeerRecvPort(ComIP c)
        {
            // we send to this address, so its his receive port
            //return CAPI_ntohs(((ComIP*)c).sendAddress.sin_port);
            return ((IPEndPoint)c.send_sock.RemoteEndPoint).Port;
        }

        public static ComAPIEnumProtocols ComAPIGetProtocol(ComAPIHandle c)
        {
            return c.protocol;
        }

        public static int ComAPIGetPeerId(ComIP c)
        {
            return IPAddress.NetworkToHostOrder(c.id);
        }

        // my information, the globals are defined in capi.c
        // always use the functions
        public static int ComAPIGetMyRecvPort()
        {
            return IPAddress.NetworkToHostOrder(myRecvPort);
        }

        public static int ComAPIGetMyReliableRecvPort()
        {
            return IPAddress.NetworkToHostOrder(myReliableRecvPort);
        }

        public static void ComAPISetMyRecvPort(ushort port)
        {
            myRecvPort = IPAddress.HostToNetworkOrder(port);
        }

        public static void ComAPISetMyReliableRecvPort(ushort port)
        {
            myReliableRecvPort = IPAddress.HostToNetworkOrder(port);
        }

        // checks if an IP comes from a private network
        // ip is in host order
        public static int ComAPIPrivateIP(uint ip)
        {
            // ip is composed
            // XXX.YYY.ZZZ.WWW
            uint x, y, z, w; //use long to avoid warning
            x = (ip & 0xFF000000) >> 24;
            y = (ip & 0x00FF0000) >> 16;
            z = (ip & 0x0000FF00) >> 8;
            w = (ip & 0x000000FF);

            if (
                ((x == 127) && (y == 0) && (z == 0) && (w == 1)) || // localhost
                (x == 10) || // class A reserved
                ((x == 172) && ((y >= 16) && (y < 31))) ||  // class B reserver
                ((x == 192) && (y == 168) && ((z >= 0) && (z < 255))) // class C reserved
            )
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
        // - sfr end

        public static ComAPIHandle ComIPMulticastOpen(int buffersize, string gamename, int mc_scope)
        { throw new NotImplementedException(); }


        //ComAPIHandle ComDPLAYOpen(int protocol,int mode, char *address, int buffersize, void *guid, void (*ConnectCallback)(ComAPIHandle c, int ret), int timeoutsecs);

        //void ComAPIDPLAYSendMode(ComAPIHandle c, int sendmode);   /* default is GUARANTEED */


        /* end comms session */
        public static void ComAPIClose(ComAPIHandle c)
        { throw new NotImplementedException(); }


        // send and receive data from comms
        public static int ComAPISendOOB(ComAPIHandle c, int msgsize, int type)
        { throw new NotImplementedException(); }

        public static int ComAPISend(ComAPIHandle c, int msgsize, int type)
        { throw new NotImplementedException(); }

        public static int ComAPISendDummy(ComAPIHandle c, uint ip, ushort port)
        { throw new NotImplementedException(); }


        public static int ComAPIGet(ComAPIHandle c)
        { throw new NotImplementedException(); }


        public static void ComAPIRegisterInfoCallback(RegisterInfoCallback func)
        { throw new NotImplementedException(); }


        //////////////////
        // BW FUNCTIONS //
        //////////////////
        // sfr: changed bw functions
        /** starts bw control */
        public static void ComAPIBWStart()
        { throw new NotImplementedException(); }

        /** gets local bandwidth, bytes per second */
        public static int ComAPIBWGet()
        { throw new NotImplementedException(); }

        /** called when a player joins, adjusting bw */
        public static void ComAPIBWPlayerJoined()
        { throw new NotImplementedException(); }

        /** called when a player leaves, adjusting bw */
        public static void ComAPIBWPlayerLeft()
        { throw new NotImplementedException(); }

        /** enters a given state: CAPI_LOBBY_ST, CAPI_CAS_ST, CAC_ST e DF_ST */
        public static void ComAPIBWEnterState(int state)
        { throw new NotImplementedException(); }

        /** gets BW situation for a connection: 0 ok, 1 yellow, 2 or more critical */
        public static int ComAPIBWGetStatus(int isReliable)
        { throw new NotImplementedException(); }



        /* set the group to send and recieve data from */
        public static void ComAPIGroupSet(ComAPIHandle c, int group)
        { throw new NotImplementedException(); }


        /* get the local hosts unique id */
        public static int ComAPIHostIDLen(ComAPIHandle c)
        { throw new NotImplementedException(); }

        public static int ComAPIHostIDGet(ComAPIHandle c, byte[] buf, int reset)
        { throw new NotImplementedException(); }


        /* get the associated buffers */
        public static byte[] ComAPISendBufferGet(ComAPIHandle c)
        { throw new NotImplementedException(); }

        public static byte[] ComAPIRecvBufferGet(ComAPIHandle c)
        { throw new NotImplementedException(); }


        /* query connection information - not all options are supported on all protocols */
        /* refer to query types #defined above */
        public static uint ComAPIQuery(ComAPIHandle c, ComAPIQueryTypes querytype)
        { throw new NotImplementedException(); }


        /* Group functions*/
        //public static ComAPIHandle ComAPICreateGroup(string name, int messagesize, params ComAPIHandle[] vptr)

        /* Must call with 0 terminating parameter ie: ComCreateGroup(1024,0)
           If called with comhandles , these will be used to determine Group Message size
           ie:   ComCreateGroup(1024,,ch1,ch2,0)  */


        /* member may be a group handle or a connection handle */
        public static ComAPIEnumErrorCodes ComAPIAddToGroup(ComAPIHandle grouphandle, ComAPIHandle memberhandle)
        {
            ComGROUP group = (ComGROUP)grouphandle;

            if (grouphandle == null)
            {
                return ComAPIEnumErrorCodes.COMAPI_EMPTYGROUP;
            }

            if (grouphandle.protocol != ComAPIEnumProtocols.CAPI_GROUP_PROTOCOL)
            {
                return ComAPIEnumErrorCodes.COMAPI_NOTAGROUP;
            }

            enter_cs();

            group.GroupHead = CAPIListRemove(group.GroupHead, memberhandle);
            group.GroupHead = CAPIListAppend(group.GroupHead);

            if (group.GroupHead == null)
            {
                leave_cs();
                return ComAPIEnumErrorCodes.COMAPI_EMPTYGROUP;
            }

            group.GroupHead.com = memberhandle;

            /* Reduce group send_buffer size if adding a member with smaller buffer */
            {
                int bufSize = group.buffer_size;
                int qSize = (int)memberhandle.query_func(memberhandle, ComAPIQueryTypes.COMAPI_ACTUAL_BUFFER_SIZE) + group.max_header;
                group.buffer_size = Math.Min(bufSize, qSize);
            }

            /* If Group HostID is null, take HostID from the first added member*/
            if (group.HostID == 0)
            {
                memberhandle.addr_func(memberhandle, out group.HostID, false);
            }

            leave_cs();

            return ComAPIEnumErrorCodes.COMAPI_OK;
        }

        public static ComAPIEnumErrorCodes ComAPIDeleteFromGroup(ComAPIHandle grouphandle, ComAPIHandle memberhandle)
        {
            ComGROUP group = (ComGROUP)grouphandle;
            CAPIList curr = null;

            if (grouphandle == null)
            {
                return ComAPIEnumErrorCodes.COMAPI_OK;
            }

            if (memberhandle == null)
            {
                return ComAPIEnumErrorCodes.COMAPI_OK;
            }

            if (grouphandle.protocol != ComAPIEnumProtocols.CAPI_GROUP_PROTOCOL)
            {
                return ComAPIEnumErrorCodes.COMAPI_NOTAGROUP;
            }

            enter_cs();

#if DEBUG
            MonoPrint("ComAPIDeleteFromGroup GH:\"%s\" MH:\"%s\"\n", grouphandle.name, memberhandle.name);
#endif
            group.GroupHead = CAPIListRemove(group.GroupHead, memberhandle);

#if DEBUG
            MonoPrint("================================\n");
            MonoPrint("GROUP \"%s\"\n", grouphandle.name);
            curr = group.GroupHead;

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

            leave_cs();

            return ComAPIEnumErrorCodes.COMAPI_OK;
        }

        public static ComAPIHandle CAPIIsInGroup(ComAPIHandle grouphandle, uint ipAddress)
        {
            ComGROUP group = (ComGROUP)grouphandle;
            CAPIList curr;

            if (grouphandle == null)
            {
                return null;
            }

            if (CAPIListFindHandle(GlobalGroupListHead, grouphandle) == null)
            {
                return null; /* is it in  our list ? */
            }

            if (group.GroupHead == null)
            {
                return null;
            }

            /* proceed thru list and call send_function() for each connection */
            for (curr = group.GroupHead; curr != null; curr = curr.next)
            {
                if (curr.com.protocol == ComAPIEnumProtocols.CAPI_TCP_PROTOCOL)
                {
                    ComTCP c;
                    uint cip;

                    c = (ComTCP)curr.com;
                    cip = ConvertAddrToUInt32(c.Addr.Address);

                    if (cip == ipAddress)
                    {
                        return curr.com;
                    }
                }
                else if (curr.com.protocol == ComAPIEnumProtocols.CAPI_UDP_PROTOCOL)
                {
                    ComIP c;
                    uint cip;

                    c = (ComIP)curr.com;
                    cip = ConvertAddrToUInt32(c.sendAddress);

                    if (cip == ipAddress)
                    {
                        return curr.com;
                    }
                }
                else if
                (
                    (curr.com.protocol == ComAPIEnumProtocols.CAPI_DPLAY_TCP_PROTOCOL) ||
                    (curr.com.protocol == ComAPIEnumProtocols.CAPI_DPLAY_MODEM_PROTOCOL) ||
                    (curr.com.protocol == ComAPIEnumProtocols.CAPI_DPLAY_SERIAL_PROTOCOL)
                )
                {
                    uint cip;
                    cip = ComAPIQuery(curr.com, ComAPIQueryTypes.COMAPI_CONNECTION_ADDRESS);

                    if (cip == ipAddress)
                    {
                        return curr.com;
                    }
                }
                else if (curr.com.protocol == ComAPIEnumProtocols.CAPI_GROUP_PROTOCOL)  /* another group */
                {
                    ComAPIHandle c;
                    c = CAPIIsInGroup(curr.com, ipAddress);

                    if (c != null)
                    {
                        return c;
                    }
                }
            }

            return null;
        }
        /* Send to a group but exclude Xhandle */
        //int ComAPISendX(ComAPIHandle group, int msgsize, ComAPIHandle Xhandle );

        /* Close all Open IP handles */
        public static void ComAPICloseOpenHandles()
        {
            CAPIList curr, list;
            int i;

            list = GlobalGroupListHead;

            if (list != null)
            {
                for (i = 0, curr = list; curr != null; i++, curr = curr.next)
                {
                    curr.com.close_func(curr.com);
                }
            }

            list = GlobalListHead;

            if (list != null)
            {
                for (i = 0, curr = list; curr != null; i++, curr = curr.next)
                {
                    curr.com.close_func(curr.com);
                }
            }
        }

        /* Convert host long IP address to string */
        public static string ComAPIinet_htoa(uint ip)
        { throw new NotImplementedException(); }


        /* Convert net long IP address to string */
        public static string ComAPIinet_ntoa(uint ip)
        { throw new NotImplementedException(); }

        /* Convert dotted string ipa ddress to host long */
        public static uint ComAPIinet_haddr(string IPaddress)
        { throw new NotImplementedException(); }


        /* reports last error for ComOPen calls */
        public static uint ComAPIGetLastError()
        { throw new NotImplementedException(); }


        public static IPAddress ComAPIGetNetHostBySocket(Socket socket)
        {
            if (socket == null)
            {
                return null;
            }
            return ((IPEndPoint)socket.LocalEndPoint).Address;
        }

        /* TIMESTAMP functions */
        public delegate uint TimeStampDelegate();

        /* OPTIONAL call to set the timestamp function */
        static TimeStampDelegate CAPI_TimeStamp = null;
        public static void ComAPISetTimeStampFunction(TimeStampDelegate TimeStamp)
        {
            CAPI_TimeStamp = TimeStamp;
            if (CAPI_TimeStamp != null)
                CAPI_TimeStamp();
        }


        /* get the timestamp associated with the most recent ComAPIGet()
           returns 0 if no timestamp function has been defined */
        public static uint ComAPIGetTimeStamp(ComAPIHandle c)
        { throw new NotImplementedException(); }


        /****  Protoype for TimeStamp callback
            uint ComAPIGetTimeStamp(ComAPIHandle c)
        ***/


        /* sets the rececive thread priority for UDP .. receive thread .. if exists*/
        public static void ComAPISetReceiveThreadPriority(ComAPIHandle c, int priority)
        { throw new NotImplementedException(); }



        /** sfr: translates address to ip. Answer in machine order */
        public static long ComAPIGetIP(byte[] address)
        { throw new NotImplementedException(); }

        static int internalId = -1;
        static int force_ip_address = 0;
        static ComAPIEnumErrorCodes ComIPHostIDGet(ComAPIHandle c, out int id, bool reset)
        {

            // reset the ID, choose a new one
            if (reset)
            {
                internalId = -1;
            }

            // -1 is never a valid ID
            while (internalId == -1)
            {
                internalId = (int)(DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond);
            }

            if (force_ip_address != 0)
            {
                internalId = force_ip_address;
            }

            // writes Id to buffer, host order
            //memcpy(buf, &internalId, sizeof(int));
            id = internalId;
            return 0;

#if false
    // our hostname
    char name[1024];
    // our hostent struct, with our IP
    struct hostent *hentry;
    int i;

    // we need a valid handle
    if (c == NULL)
    {
        return COMAPI_HOSTID_ERROR;
    }


    // get our hostname
    if (CAPI_gethostname(name, 1024) == SOCKET_ERROR)
    {
        return COMAPI_HOSTID_ERROR;
    }

    // from hostname, get IP
    if ((hentry = CAPI_gethostbyname(name)) == NULL)
    {
        return COMAPI_HOSTID_ERROR;
    }


    if (force_ip_address)
    {
        buf[3] = (char)(force_ip_address & 0xff);
        buf[2] = (char)((force_ip_address >> 8)  & 0xff);
        buf[1] = (char)((force_ip_address >> 16) & 0xff);
        buf[0] = (char)((force_ip_address >> 24) & 0xff);
        return 0;
    }

    // sfr: build IP from hentry struct.
    // shouldnt we use inet_ntoa?
    // wow this is dark magic... dont wanna get in here
    // hentry->h_addr_list[i] is an array of char
    // run all available IPs for host
    for (i = 0; hentry->h_addr_list[i] != NULL; i++)
    {
        // we return either first IP or hostIdx IP
        // first IP
        if (i == 0)
        {
            *((int*)buf) = *((int*)(hentry->h_addr_list[i]));
        }

        // hostIdx IP
        if (i == ComIPGetHostIDIndex)
        {
            *((int*)buf) = *((int*)(hentry->h_addr_list[i]));
            return 0;
        }
    }

    return 0;
#endif
        }

        static bool init_cs = false;
        static CRITICAL_SECTION cs;

        static void enter_cs()
        {
            if (!init_cs)
            {
                CriticalSectionConversion.InitializeCriticalSection(ref cs);
                init_cs = true;
            }

            CriticalSectionConversion.EnterCriticalSection(ref cs);
        }

        static void leave_cs()
        {
            if (init_cs)
            {
                CriticalSectionConversion.LeaveCriticalSection(ref cs);
            }
        }

        /* Mutex macros */
        static void SAY_ON(Mutex a) { }

        static void SAY_OFF(Mutex a) { }

        static void CREATE_LOCK(Mutex a, string name)
        {
            a = new Mutex(false, name);
            if (a == null)
                Debugger.Break();
        }

        static void REQUEST_LOCK(Mutex a)
        {
            bool w = a.WaitOne()/*WaitForSingleObject(a, INFINITE)*/;
            SAY_ON(a);
            if (!w) //== WAIT_FAILED)
                Debugger.Break();
        }

        static void RELEASE_LOCK(Mutex a)
        {
            SAY_OFF(a);
            try
            {
                a.ReleaseMutex();
            }
            catch (Exception ex)
            {
                Debugger.Break();
            }
        }
        static void DESTROY_LOCK(Mutex a)
        {
            try
            {
                a.Close();
            }
            catch (Exception ex)
            {
                Debugger.Break();
            }
        }

        static uint ConvertAddrToUInt32(IPAddress addr)
        {
            byte[] buf = addr.GetAddressBytes();
            Array.Reverse(buf); // Flip big-endian (network order) to little-endian
            return BitConverter.ToUInt32(buf, 0);
        }

        private static int WS2Connections = 0;

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }

    public class ComAPIHandle
    {
        public string name;
        public ComAPIEnumProtocols protocol;

        // send and receive functions for this comm
        public delegate int SendDelegate(ComAPIHandle c, int msgsize, bool oob, int type);
        public delegate int SendDummyDelegate(ComAPIHandle c, uint msgsize, ushort port);
        public delegate int SendXDelegate(ComAPIHandle c, int msgsize, bool oob, int type, ComAPIHandle Xcom);
        public delegate int RecvDelegate(ComAPIHandle c);

        public SendDelegate send_func;
        public SendDummyDelegate send_dummy_func;
        public SendXDelegate sendX_func;
        public RecvDelegate recv_func;

        // buffer functions
        public delegate int SendBufDelegate(ComAPIHandle c);
        public delegate int RecvBufDelegate(ComAPIHandle c);

        public SendBufDelegate send_buf_func;
        public RecvBufDelegate recv_buf_func;

        // address function
        public delegate ComAPIEnumErrorCodes AddrDelegate(ComAPIHandle c, out int id, bool reset);
        public AddrDelegate addr_func;

        // close function
        public delegate void CloseDelegate(ComAPIHandle c);
        public CloseDelegate close_func;

        // query function
        public delegate uint QueryDelegate(ComAPIHandle c, ComAPIQueryTypes querytype);
        public QueryDelegate query_func;

        // timestamp function
        public delegate uint GetTimestampDelegate(ComAPIHandle c);
        public GetTimestampDelegate get_timestamp_func;
    }

    public class Reliable_Packet
    {
        public uint last_sent_at; /* time this packet was last sent */
        public ushort sequence_number;
        public ushort message_number;
        public ushort size;
        public byte send_count;
        public byte oob;
        public byte message_slot;
        public byte message_parts;
        public byte dispatched; /* has the application seen us yet? */
        public byte acknowledged; /* have we sent the ack out of sequence */

        public byte[] data;

        public Reliable_Packet next;
    }

    public class Reliable_Data
    {
        public ushort sequence_number; /* sequence number for sending */
        public ushort oob_sequence_number; /* sequence number for sending */

        public short message_number; /* message number for sending */
        public Reliable_Packet sending; /* list of sent packets */
        public Reliable_Packet last_sent; /* last of the sending packets */

        public int reset_send; /* What is the reset stage we are in */

        public int last_sequence; /* other's last seen sequence number */
        public int last_received; /* my last received sequential sequence number for ack.*/
        public int last_sent_received; /* the last last_received that I acknowledged */
        public int send_ack; /* we need to send an ack packet */

        public int last_dispatched; /* the last packet that I dispatched */
        public Reliable_Packet receiving; /* list of received packets */

        public int sent_received; /* what the last received I sent */
        public uint last_send_time; /* last time we checked for ack */

        public Reliable_Packet oob_sending; /* list of sent packets */
        public Reliable_Packet oob_last_sent; /* last of the sending packets */

        public int last_oob_sequence; /* other's last seen sequence number */
        public int last_oob_received; /* my last received sequential sequence number for ack.*/
        public int last_oob_sent_received; /* the last last_received that I acknowledged */
        public int send_oob_ack; /* we need to send an ack packet */

        public int last_oob_dispatched; /* the last packet that I dispatched */
        public Reliable_Packet oob_receiving; /* list of received packets */

        public int sent_oob_received; /* what the last received I sent */
        uint last_oob_send_time; /* last time we checked for ack */

        public long last_ping_send_time; /* when was the last time I sent a ping */
        public long last_ping_recv_time; /* the time we received on a ping */
        public byte[] real_send_buffer; /* buffer actually sent down the wire (after encoding) */
    }

    public class ComIP : ComAPIHandle
    {
        public ComAPIHandle apiheader;

        public int buffer_size;
        public int max_buffer_size;
        public int ideal_packet_size;
        public byte[] send_buffer;
        public int send_buffer_start;
        public byte[] recv_buffer;
        public byte[] wsaData;
        public byte[] compression_buffer;

        //sfr:
        public IPAddress sendAddress; // we send data to this address
        public IPAddress recAddress;  // and receive from this. This is same among all handles of same protocol

        public Socket send_sock;
        public Socket recv_sock;
        public uint recvmessagecount;
        public uint sendmessagecount;
        public uint recvwouldblockcount;
        public uint sendwouldblockcount;

#if TODO
        public HANDLE lock_;
        public HANDLE ThreadHandle;
#else
        public Mutex lock_;
        public Thread ThreadHandle;
#endif
        public short ThreadActive;
        public short current_get_index;
        public short current_store_index;
        public short last_gotten_index;
        public short wrapped;
        public byte[] message_cache;
        public short[] bytes_recvd;
        public ComIP parent;
        public int referencecount;
        public int BroadcastModeOn;
        public int NeedBroadcastMode;
        //sfr: identifiers
        public int whoami; // me, the host
        public int id;    // id of the owner of this com
        public uint lastsender; // ip of last sender... kinda obvious. but will keep it here
                                //sfr: converts
                                //port info
        public uint lastsenderport;
        // sfr id of last sender
        public uint lastsenderid;
        public uint timestamp;
        public uint[] timestamps;
        public uint[] senders;
        public Reliable_Data rudp_data;
    }

    public class ComGROUP : ComAPIHandle
    {
        public ComAPIHandle apiheader;
        public int buffer_size;
        public int HostID;
        public CAPIList GroupHead;
        public byte[] send_buffer;
        public int send_buffer_start;
        public int max_header;
        public sbyte TCP_buffer_shift;
        public sbyte UDP_buffer_shift;
        public sbyte RUDP_buffer_shift;
        public sbyte DPLAY_buffer_shift;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ComAPIHeader
    {
        public const int GAME_NAME_LENGTH = 4;
        public const int Size = GAME_NAME_LENGTH + sizeof(uint);

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = GAME_NAME_LENGTH)]
        public string gamename; //[GAME_NAME_LENGTH];
        public uint id; ///< this is the ID of the sender
    }

    public class CAPIList
    {
        public CAPIList next;
        public string name;
        public ComAPIHandle com;

#if CAPI_NET_DEBUG_FEATURES
        public  void* data;
        public int size;
        public int sendtime;
#endif
    }
}
