using VU_BOOL = System.Boolean;

namespace FalconNet.VU
{
    public class VuGetRequest : VuRequestMessage
    {

        public VuGetRequest(VU_SPECIAL_GET_TYPE sgt, VuSessionEntity sess = null)
            : base(VU_MSG_DEF.VU_GET_REQUEST_MESSAGE, VU_ID.vuNullId,
                     (sess != null ? sess
                      : ((sgt == VU_SPECIAL_GET_TYPE.VU_GET_GLOBAL_ENTS) ? (VuTargetEntity)VUSTATIC.vuGlobalGroup
                         : (VuTargetEntity)VUSTATIC.vuLocalSessionEntity.Game())))
        {
        }
        public VuGetRequest(VU_ID entityId, VuTargetEntity target)
            : base(VU_MSG_DEF.VU_GET_REQUEST_MESSAGE, entityId, target)
        {
            // empty
        }
        public VuGetRequest(VU_ID senderid, VU_ID target)
            : base(VU_MSG_DEF.VU_GET_REQUEST_MESSAGE, senderid, target)
        {
            // empty
        }
        //TODO public virtual ~VuGetRequest();

        protected override VU_ERRCODE Process(VU_BOOL autod)
        {
            VuTargetEntity sender = (VuTargetEntity)VUSTATIC.vuDatabase.Find(Sender());

            if (!IsLocal())
            {
                //		MonoPrint ("Get Request %08x\n", entityId_);

                if (sender != null && sender.IsTarget())
                {
                    VuMessage resp = null;
                    if (autod)
                    {
                        resp = new VuErrorMessage(VUERROR.VU_NOT_AVAILABLE_ERROR, Sender(), EntityId(), sender);
                    }
                    else if (entityId_ == VU_ID.vuNullId)
                    {
                        // get ALL ents
                        if (tgtid_ == VUSTATIC.vuGlobalGroup.Id() || tgtid_ == VUSTATIC.vuLocalSession)
                        {
                            // get all _global_ ents
                            VuDatabaseIterator iter = new VuDatabaseIterator();
                            VuEntity ent = iter.GetFirst();

                            while (ent != null)
                            {
                                if (!ent.IsPrivate() && ent.IsGlobal())
                                {// ent.IsLocal() && 
                                    if (ent.Id() != sender.Id())
                                    {
                                        if (ent.IsLocal())
                                        {
                                            // MonoPrint("Get Request: Sending Full Update on %08x to %08x\n", ent.Id().creator_.value_, sender.Id().creator_.value_);
                                            resp = new VuFullUpdateEvent(ent, sender);
                                            resp.RequestOutOfBandTransmit();
                                            resp.RequestReliableTransmit();
                                            VuMessageQueue.PostVuMessage(resp);
                                        }
                                        else
                                        {
                                            // MonoPrint("Get Request: Sending Broadcast Global on %08x to %08x\n", ent.Id().creator_.value_, sender.Id().creator_.value_);
                                            resp = new VuBroadcastGlobalEvent(ent, sender);
                                            resp.RequestReliableTransmit();
                                            resp.RequestOutOfBandTransmit();
                                            VuMessageQueue.PostVuMessage(resp);
                                        }
                                    }
                                }

                                ent = iter.GetNext();
                            }
                            return VU_ERRCODE.VU_SUCCESS;
                        }
                        else if (tgtid_ == VUSTATIC.vuLocalSessionEntity.GameId())
                        {
                            // get all _game_ ents
                            VuDatabaseIterator iter = new VuDatabaseIterator();
                            VuEntity ent = iter.GetFirst();

                            while (ent != null)
                            {
                                if (!ent.IsPrivate() && ent.IsLocal() && !ent.IsGlobal())
                                {
                                    if (ent.Id() != sender.Id())
                                    {
                                        resp = new VuFullUpdateEvent(ent, sender);
                                        resp.RequestReliableTransmit();
                                        //					resp.RequestLowPriorityTransmit();
                                        VuMessageQueue.PostVuMessage(resp);
                                    }
                                }
                                ent = iter.GetNext();
                            }
                            return VU_ERRCODE.VU_SUCCESS;
                        }
                    }
                    else if (Entity() != null && Entity().OwnerId() == VUSTATIC.vuLocalSession)
                    {
                        resp = new VuFullUpdateEvent(Entity(), sender);
                    }
                    else if (Destination() == VUSTATIC.vuLocalSession)
                    {
                        // we were asked specifically, so send the error response
                        resp = new VuErrorMessage(VUERROR.VU_NO_SUCH_ENTITY_ERROR, Sender(), EntityId(), sender);
                    }
                    if (resp != null)
                    {
                        resp.RequestReliableTransmit();
                        VuMessageQueue.PostVuMessage(resp);
                        return VU_ERRCODE.VU_SUCCESS;
                    }
                }
                else
                {
                    if (entityId_ == VU_ID.vuNullId)
                    {
                        // get all _global_ ents
                        VuDatabaseIterator iter = new VuDatabaseIterator();
                        VuEntity ent = iter.GetFirst();
                        VuMessage resp = null;

                        while (ent != null)
                        {
                            if (!ent.IsPrivate() && ent.IsGlobal())
                            {// ent.IsLocal() && 
                                if ((sender != null) && (ent.Id() != sender.Id()))
                                {
                                    if (ent.IsLocal())
                                    {
                                        // MonoPrint("Get Request: Sending Full Update on %08x to %08x\n", ent.Id().creator_.value_, sender.Id().creator_.value_);
                                        resp = new VuFullUpdateEvent(ent, sender);
                                        resp.RequestOutOfBandTransmit();
                                        VuMessageQueue.PostVuMessage(resp);
                                    }
                                    else
                                    {
                                        // MonoPrint("Get Request: Sending Broadcast Global on %08x to %08x\n", ent.Id().creator_.value_, sender.Id().creator_.value_);
                                        resp = new VuBroadcastGlobalEvent(ent, sender);
                                        resp.RequestOutOfBandTransmit();
                                        VuMessageQueue.PostVuMessage(resp);
                                    }
                                }
                            }

                            ent = iter.GetNext();
                        }
                        return VU_ERRCODE.VU_SUCCESS;
                    }
                    else if ((Entity()) != null && (Entity().IsLocal()))
                    {
                        VuMessage resp = null;

                        resp = new VuFullUpdateEvent(Entity(), sender);
                        VuMessageQueue.PostVuMessage(resp);
                        return VU_ERRCODE.VU_SUCCESS;
                    }
                }
            }
            return VU_ERRCODE.VU_NO_OP;
        }
    }


}
