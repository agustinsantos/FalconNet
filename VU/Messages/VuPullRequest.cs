using VU_BOOL = System.Boolean;


namespace FalconNet.VU
{
    //--------------------------------------------------
    public class VuPullRequest : VuRequestMessage
    {

        public VuPullRequest(VU_ID entityId, VuTargetEntity target)
            : base(VU_MSG_DEF.VU_PULL_REQUEST_MESSAGE, entityId, target)
        {
            // empty
        }
        public VuPullRequest(VU_ID senderid, VU_ID target)
            : base(VU_MSG_DEF.VU_PUSH_REQUEST_MESSAGE, senderid, target)
        {
            // empty
        }
        //TODO public virtual ~VuPullRequest();

        protected override VU_ERRCODE Process(VU_BOOL autod)
        {
            VU_ERRCODE retval = VU_ERRCODE.VU_NO_OP;

            if (!IsLocal() && Destination() == VUSTATIC.vuLocalSession)
            {
                if (Entity() != null)
                {
                    retval = Entity().Handle(this);
                }
                else
                {
                    VuTargetEntity sender = (VuTargetEntity)VUSTATIC.vuDatabase.Find(Sender());
                    if (sender != null && sender.IsTarget())
                    {
                        VuMessage resp = new VuErrorMessage(VUERROR.VU_NO_SUCH_ENTITY_ERROR, Sender(),
                                                  EntityId(), sender);
                        resp.RequestReliableTransmit();
                        VuMessageQueue.PostVuMessage(resp);
                        retval = VU_ERRCODE.VU_SUCCESS;
                    }
                }
            }

            return retval;
        }

    }


}
