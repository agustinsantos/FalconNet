using System;
using VU_MSG_TYPE = System.UInt64;
using VU_KEY = System.UInt64;
using VU_ID_NUMBER = System.UInt64;
using FalconNet.Common.Encoding;

namespace FalconNet.VU
{


    public class VU_SESSION_ID
    {

        public VU_SESSION_ID() { }
        public VU_SESSION_ID(ulong value) { value_ = value; }
        
        public int Encode(ByteWrapper buffer)
        {
            byte[] buf = EncodingHelpers.EncodeULongBE(value_);
            buffer.Put(buf);
            return buf.Length;
        }

        public static bool operator ==(VU_SESSION_ID lhs, VU_SESSION_ID rhs)
        { return (lhs.value_ == rhs.value_ ? true : false); }
        public static bool operator !=(VU_SESSION_ID lhs, VU_SESSION_ID rhs)
        { return (lhs.value_ != rhs.value_ ? true : false); }
        public static bool operator >(VU_SESSION_ID lhs, VU_SESSION_ID rhs)
        { return (lhs.value_ > rhs.value_ ? true : false); }
        public static bool operator >=(VU_SESSION_ID lhs, VU_SESSION_ID rhs)
        { return (lhs.value_ >= rhs.value_ ? true : false); }
        public static bool operator <(VU_SESSION_ID lhs, VU_SESSION_ID rhs)
        { return (lhs.value_ < rhs.value_ ? true : false); }
        public static bool operator <=(VU_SESSION_ID lhs, VU_SESSION_ID rhs)
        { return (lhs.value_ <= rhs.value_ ? true : false); }
        
        public override int GetHashCode()
        {
            return (int)this.value_;
        }

        public static explicit operator ulong(VU_SESSION_ID id) { return id.value_; }
        public static implicit operator VU_SESSION_ID(ulong id) { return new VU_SESSION_ID(id); }
        // DATA
        public ulong value_ = 0;
    }

}
