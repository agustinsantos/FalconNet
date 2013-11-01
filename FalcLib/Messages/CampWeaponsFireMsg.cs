using System;
using FalconNet.VU;
using VU_MSG_TYPE = System.Byte;

namespace FalconNet.FalcLib
{
    // =============================================================================
    // KCK: This message is used to broadcast damage data to the entire game for
    // Campaign Entities. This is called only from a Campaign Entity's ApplyDamage()
    // function, but is called regardless of wether damage was applied in order to
    // do the distant visual effects.
    //
    // It contains a list of all weapons being fired and the quantity, as well as
    // decodable damage data. The weapon list is used for visual effects on anymore.
    // =============================================================================

    public static class CampWeaponsFireStatic
    {
        public const int MAX_TYPES_PER_CAMP_FIRE_MESSAGE = 8;
        public static byte[] gDamageStatusBuffer = new byte[256];
        public static byte[] gDamageStatusPtr;
#if TODO
        // Functions to fire on sim entities from campaign units
        public static SimBaseClass GetSimTarget(CampEntity target, byte targetId);
        public static void FireOnSimEntity(CampEntity shooter, CampEntity campTarg, short[] weapon, byte[] shots, byte targetId = 255);
        public static void FireOnSimEntity(CampEntity shooter, SimBaseClass simTarg, short weaponId);
        public static void SendSimDamageMessage(CampEntity shooter, SimBaseClass target, float rangeSq, int damageType, int weapId);
#endif
    }

    /*
     * Message Type Campaign Weap Fire
     */
    public class FalconCampWeaponsFire : FalconEvent
    {
        public FalconCampWeaponsFire(VU_ID entityId, VuTargetEntity target, bool loopback = true)
            : base((VU_MSG_TYPE)FalconMsgID.CampWeaponFireMsg, HandlingThread.CampaignThread, entityId, target, loopback)
        { throw new NotImplementedException(); }
        
        public FalconCampWeaponsFire(VU_MSG_TYPE type, VU_ID senderid, VU_ID target)
            : base((VU_MSG_TYPE)FalconMsgID.CampWeaponFireMsg, HandlingThread.CampaignThread, senderid, target)
        { throw new NotImplementedException(); }

        //TODO public  ~FalconCampWeaponsFire();

        public override int Size()
        { throw new NotImplementedException(); }
        public override int Decode(byte[] buf, ref int pos, int length)
        { throw new NotImplementedException(); }
        public override int Encode(byte[] buf, ref int pos)
        { throw new NotImplementedException(); }
        public class DATA_BLOCK
        {
            public VU_ID shooterID;
            public VU_ID fWeaponUID;
            public short[] weapon = new short[CampWeaponsFireStatic.MAX_TYPES_PER_CAMP_FIRE_MESSAGE];
            public byte[] shots = new byte[CampWeaponsFireStatic.MAX_TYPES_PER_CAMP_FIRE_MESSAGE];
            public byte fPilotId;
            public byte dPilotId;
            public ushort size;
            public byte[] data;
        };
        public DATA_BLOCK dataBlock;

        protected override VU_ERRCODE Process(bool autodisp)
        { throw new NotImplementedException(); }

    }
}
