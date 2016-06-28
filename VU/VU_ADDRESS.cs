using FalconNet.Common.Encoding;
using FalconNet.VU.Comms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VU_BYTE = System.Byte;

namespace FalconNet.VU
{
    /// <summary>
    /// Represents an entity address. All entities are composed of
    /// an IP address and 2 receive ports(one for reliable)
    /// </summary>
    public class VU_ADDRESS
    {
        public VU_ADDRESS(uint ip = 0,                                //< entity IP
                        ushort recvPort = 0,//CAPI_UDP_PORT,         //< port where he receives
                        ushort reliableRecvPort = 0 //CAPI_TCP_PORT  //< port where he receives reliable data
                        )
        {
            this.ip = ip;
            this.recvPort = recvPort;
            this.reliableRecvPort = reliableRecvPort;
        }

        // returns the struct size
        public int Size()
        {
            // ip + ports
            return sizeof(long) + sizeof(short) * 2;
        }


        // equality: everything equal
        public static bool operator ==(VU_ADDRESS lhs, VU_ADDRESS rhs)
        {
            return (lhs.ip == rhs.ip) &&
                      (lhs.recvPort == rhs.recvPort) &&
                      (lhs.reliableRecvPort == rhs.reliableRecvPort);
        }
        public static bool operator !=(VU_ADDRESS lhs, VU_ADDRESS rhs)
        {
            return !(lhs == rhs);
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return this == (VU_ADDRESS)obj;
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            int hash = 23;
            hash = hash * 31 + recvPort;
            hash = hash * 31 + reliableRecvPort;
            hash = (int)(hash * 31 + ip);
            return hash;
        }

        /// <summary>
        /// reads Vu address from stream
        /// </summary>
        /// <param name="stream"></param>
        public void Decode(Stream stream)
        {
            recvPort = UInt16EncodingLE.Decode(stream);
            reliableRecvPort = UInt16EncodingLE.Decode(stream);
            ip = UInt32EncodingLE.Decode(stream);
        }

        // writes Vu address to stream, returns ammount of written data
        public void Encode(Stream stream)
        {
            UInt16EncodingLE.Encode(stream, recvPort);
            UInt16EncodingLE.Encode(stream, recvPort);
            UInt32EncodingLE.Encode(stream, recvPort);
        }

        // returns if an address comes from private network
        public bool IsPrivate()
        {
            return CommApi.ComAPIPrivateIP(this.ip) != 0;
        }

        // ports and ip data in host order
        private ushort recvPort, reliableRecvPort;
        private uint ip;
    }
}
