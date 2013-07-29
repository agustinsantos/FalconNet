using FalconNet.Common.Encoding;
using System.IO;
using VU_BOOL = System.Boolean;


namespace FalconNet.VU
{
    public class VuErrorMessage : VuMessage
    {

        public VuErrorMessage(VUERROR errorType, VU_ID srcmsgid, VU_ID entityid, VuTargetEntity target)
            : base(VU_MSG_DEF.VU_ERROR_MESSAGE, entityid, target, false)
        {
            srcmsgid_ = srcmsgid;
            etype_ = errorType;
        }
        public VuErrorMessage(VU_ID senderid, VU_ID targetid)
            : base(VU_MSG_DEF.VU_ERROR_MESSAGE, senderid, targetid)
        {
            etype_ = VUERROR.VU_UNKNOWN_ERROR;
            srcmsgid_.num_ = 0;
            srcmsgid_.creator_ = 0;
        }

        internal VuErrorMessage() : base(VU_MSG_DEF.VU_ERROR_MESSAGE, null, null) { }

        //TODO public virtual ~VuErrorMessage();

        public VUERROR ErrorType() { return etype_; }

        protected override VU_ERRCODE Process(VU_BOOL autod)
        {
            if (Entity() != null)
            {
                Entity().Handle(this);
                return VU_ERRCODE.VU_SUCCESS;
            }
            return VU_ERRCODE.VU_NO_OP;
        }


        internal VU_ID srcmsgid_;
        internal VUERROR etype_;
    }

    public static class VuErrorMessageEncodingLE
    {
        public static void Encode(Stream stream, VuErrorMessage val)
        {
            VuMessageEncodingLE.Encode(stream, val);
            VU_IDEncodingLE.Encode(stream, val.srcmsgid_);
            Int16EncodingLE.Encode(stream, (short)val.etype_);
        }

        public static void Decode(Stream stream, VuErrorMessage rst)
        {
            VuMessageEncodingLE.Decode(stream, rst);
            VU_IDEncodingLE.Decode(stream, rst.srcmsgid_);
            rst.etype_ = (VUERROR)Int16EncodingLE.Decode(stream);
        }

        public static int Size
        {
            get { return VuMessageEncodingLE.Size + VU_IDEncodingLE.Size + Int16EncodingLE.Size; }
        }
    }
}
