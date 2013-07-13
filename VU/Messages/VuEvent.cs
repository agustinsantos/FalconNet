using FalconNet.Common.Encoding;
using System.IO;
using VU_BOOL = System.Boolean;
using VU_TIME = System.UInt64;
using VU_MSG_TYPE = System.Byte;

namespace FalconNet.VU
{
    //--------------------------------------------------
    public abstract class VuEvent : VuMessage
    {

        //TODO public virtual ~VuEvent();


        protected VuEvent(VU_MSG_TYPE type, VU_ID entityId, VuTargetEntity target, VU_BOOL loopback = false)
            : base(type, entityId, target, loopback)
        {

            updateTime_ = VUSTATIC.vuxGameTime;
        }

        protected VuEvent(VU_MSG_TYPE type, VU_ID senderid, VU_ID target)
            : base(type, senderid, target)
        {
            updateTime_ = VUSTATIC.vuxGameTime;
        }
        protected override VU_ERRCODE Activate(VuEntity ent)
        {
            SetEntity(ent);
            if (IsLocal() && ent != null)
                updateTime_ = ent.LastUpdateTime();

            //  vuDatabase.Handle(this);
            return VU_ERRCODE.VU_SUCCESS;
        }
        //protected abstract VU_ERRCODE Process(VU_BOOL autod);

        // DATA

        // these fields are filled in on Activate()
        public VU_TIME updateTime_;
    }
    public static class VuEventEncodingLE
    {
        public static void Encode(ByteWrapper buffer, VuEvent val)
        {
            VuMessageEncodingLE.Encode(buffer, val);
            UInt64EncodingLE.Encode(buffer, val.updateTime_);
        }
        public static void Encode(Stream stream, VuEvent val)
        {
            VuMessageEncodingLE.Encode(stream, val);
            UInt64EncodingLE.Encode(stream, val.updateTime_);
        }

        public static VuEvent Decode(ByteWrapper buffer, VuEvent rst)
        {
            VuMessageEncodingLE.Decode(buffer, rst);
            rst.updateTime_ = UInt64EncodingLE.Decode(buffer);
            return rst;
        }
        public static VuEvent Decode(Stream stream, VuEvent rst)
        {
            VuMessageEncodingLE.Decode(stream, rst);
            rst.updateTime_ = UInt64EncodingLE.Decode(stream);
            return rst;
        }

        public static int Size
        {
            get { return VuMessageEncodingLE.Size + UInt64EncodingLE.Size; }
        }
    }


}
