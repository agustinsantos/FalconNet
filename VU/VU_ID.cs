using FalconNet.Common.Encoding;
using System;
using System.IO;
using VU_ID_NUMBER = System.UInt64;
using VU_KEY = System.UInt64;

namespace FalconNet.VU
{
    public class VU_ID
    {
        public const int VU_NULL_ENTITY_ID = 0;
        public const int VU_GLOBAL_GROUP_ENTITY_ID = 1;
        public const int VU_PLAYER_POOL_ENTITY_ID = 2;
        public const int VU_SESSION_ENTITY_ID = 3;
        public const int VU_FIRST_ENTITY_ID = 4;       // first generated

        public static VU_ID FalconNullId = new VU_ID();
        public static VU_ID vuNullId = new VU_ID(0, 0);

        public VU_ID() { }
        public VU_ID(VU_SESSION_ID sessionpart, VU_ID_NUMBER idpart)
        {
            creator_ = sessionpart;
            num_ = idpart;
        }
        public static bool operator ==(VU_ID lhs, VU_ID rhs)
        {
            return (lhs.num_ == rhs.num_ ? (lhs.creator_ == rhs.creator_ ? true : false)
                          : false);
        }

        public static bool operator !=(VU_ID lhs, VU_ID rhs)
        {
            return (lhs.num_ == rhs.num_ ? (lhs.creator_ == rhs.creator_ ? false : true)
                          : true);
        }

        public static bool operator >(VU_ID lhs, VU_ID rhs)
        {
            if (lhs.creator_ > rhs.creator_)
                return true;
            if (lhs.creator_ == rhs.creator_)
            {
                if (lhs.num_ > rhs.num_)
                    return true;
            }
            return false;
        }
        public static bool operator >=(VU_ID lhs, VU_ID rhs)
        {
            if (lhs.creator_ > rhs.creator_)
                return true;
            if (lhs.creator_ == rhs.creator_)
            {
                if (lhs.num_ >= rhs.num_)
                    return true;
            }
            return false;
        }
        public static bool operator <(VU_ID lhs, VU_ID rhs)
        {
            if (lhs.creator_ < rhs.creator_)
                return true;
            if (lhs.creator_ == rhs.creator_)
            {
                if (lhs.num_ < rhs.num_)
                    return true;
            }
            return false;
        }
        public static bool operator <=(VU_ID lhs, VU_ID rhs)
        {
            if (lhs.creator_ < rhs.creator_)
                return true;
            if (lhs.creator_ == rhs.creator_)
            {
                if (lhs.num_ <= rhs.num_)
                    return true;
            }
            return false;
        }

        public static explicit operator VU_KEY(VU_ID id)
        {
            return (VU_KEY)(((ushort)id.creator_ << 16)
                    | ((ushort)id.num_));
        }

        public override bool Equals(Object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to VU_ID return false.
            VU_ID p = obj as VU_ID;
            if ((System.Object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (num_ == p.num_) && (creator_ == p.creator_);
        }

        public bool Equals(VU_ID obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (num_ == obj.num_) && (creator_ == obj.creator_);
        }

        public override int GetHashCode()
        {
            return (int)(((ushort)this.creator_ << 16)
                    | ((ushort)this.num_));
        }

        public override string ToString()
        {
            return String.Format("ID(Num={0}, Creator={1})", num_, creator_);
        }
        // DATA

        public VU_ID_NUMBER num_ = 0;
        public VU_SESSION_ID creator_ = 0;
    }

    public static class VU_IDEncodingLE
    {
        public static void Encode(ByteWrapper buffer, VU_ID val)
        {
            UInt64EncodingLE.Encode(buffer, val.num_);
            VU_SESSION_IDEncodingLE.Encode(buffer, val.creator_);
        }
        public static void Encode(Stream stream, VU_ID val)
        {
            UInt64EncodingLE.Encode(stream, val.num_);
            VU_SESSION_IDEncodingLE.Encode(stream, val.creator_);
        }

        public static VU_ID Decode(ByteWrapper buffer)
        {
            VU_ID rst = new VU_ID();
            rst.num_ = UInt64EncodingLE.Decode(buffer);
            rst.creator_ = VU_SESSION_IDEncodingLE.Decode(buffer);
            return rst;
        }
        public static VU_ID Decode(Stream stream)
        {
            VU_ID rst = new VU_ID();
            rst.num_ = UInt64EncodingLE.Decode(stream);
            rst.creator_ = VU_SESSION_IDEncodingLE.Decode(stream);
            return rst;
        }

        public static int Size
        {
            get { return UInt64EncodingLE.Size + VU_SESSION_IDEncodingLE.Size; }
        }
    }
}
