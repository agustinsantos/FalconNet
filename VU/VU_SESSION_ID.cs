using System;
using VU_MSG_TYPE = System.UInt64;
using VU_KEY = System.UInt64;
using VU_ID_NUMBER = System.UInt64;
using FalconNet.Common.Encoding;
using System.IO;

namespace FalconNet.VU
{


    public class VU_SESSION_ID
    {
        public VU_SESSION_ID() { }
        public VU_SESSION_ID(uint value) { value_ = value; }
        
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

        public override bool Equals(Object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to VU_SESSION_ID return false.
            VU_SESSION_ID p = obj as VU_SESSION_ID;
            if ((System.Object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (value_ == p.value_);
        }

        public bool Equals(VU_SESSION_ID obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (value_ == obj.value_);
        }
        public override int GetHashCode()
        {
            return (int)this.value_;
        }
        public override string ToString()
        {
            return String.Format("VU_SESSION_ID({0})", value_);
        }
        public static explicit operator uint(VU_SESSION_ID id) { return id.value_; }
        public static implicit operator VU_SESSION_ID(uint id) { return new VU_SESSION_ID(id); }

        // DATA
        public uint value_ = 0;
    }

    public static class VU_SESSION_IDEncodingLE
    {
        public static void Encode(Stream stream, VU_SESSION_ID val)
        {
            UInt32EncodingLE.Encode(stream, val.value_);
        }

        public static void Decode(Stream stream, VU_SESSION_ID rst)
        {
            rst.value_ = UInt32EncodingLE.Decode(stream);
        }

        public static int Size
        {
            get { return UInt32EncodingLE.Size; }
        }

    }
}
