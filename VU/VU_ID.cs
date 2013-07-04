using System;
using VU_ID_NUMBER = System.UInt64;
using VU_KEY = System.UInt64;

namespace FalconNet.VU
{
    public class VU_ID
    {

        public static VU_ID FalconNullId = new VU_ID();
        public static VU_ID vuNullId = new VU_ID(0,0);

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
}
