using System;
using FalconNet.VU;
using VU_BYTE = System.Byte;
using FalconNet.FalcLib;
using Unit = FalconNet.Campaign.UnitClass;
using FalconNet.Common;
using FalconNet.CampaignBase;
//using FalconNet.Sim;
using VU_ID_NUMBER = System.UInt64;

namespace FalconNet.Campaign
{

    // =========================
    // Air unit Class 
    // =========================

    public class AirUnitClass : UnitClass
    {
        // =========================
        // Types and Defines
        // =========================

        public const int RESERVE_MINUTES = 15;					// How much extra fuel to load. Setable?


        // constructors and serial functions
        public AirUnitClass(ushort type, VU_ID_NUMBER id)
            : base(type, id)
        {
        }

#if TODO
        public AirUnitClass(byte[] stream, ref int offset)
            : base(stream, ref offset)
        { throw new NotImplementedException(); }
        public AirUnitClass(byte[] bytes, ref int offset, int version)
            : base(bytes, ref offset, version)
        {
        }
        //TODO public virtual ~AirUnitClass();
        public override int SaveSize()
        { throw new NotImplementedException(); }
        public override int Save(VU_BYTE[] stream)
        { throw new NotImplementedException(); }
#endif
        // event Handlers
        public override VU_ERRCODE Handle(VuFullUpdateEvent evnt)
        { throw new NotImplementedException(); }

        // Required pure virtuals handled by AirUnitClass
        public override MoveType GetMovementType()
        { throw new NotImplementedException(); }
        public override int GetUnitSpeed()
        { throw new NotImplementedException(); }
        public override CampaignTime UpdateTime()
        {
            return new CampaignTime((ulong)AIInput.AIR_UPDATE_CHECK_INTERVAL * CampaignTime.CampaignSeconds);
        }

        public override float Vt() { return GetUnitSpeed() * Phyconst.KPH_TO_FPS; }

        public override float Kias() { return Vt() * Phyconst.FTPSEC_TO_KNOTS; }

        // core functions
        public override bool IsHelicopter()
        { throw new NotImplementedException(); }
        public override bool OnGround()
        { throw new NotImplementedException(); }

        // =========================================
        // Air Unit functions
        // =========================================

        public static int GetUnitScore(Unit u, MoveType mt)
        { throw new NotImplementedException(); }
    }
}