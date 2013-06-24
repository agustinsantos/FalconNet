using FalconNet.VU;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace FalconNet.Sim
{
    public class FackClass
    {

        private bool mMasterCaution;
        private bool NeedsWarnReset;	//MI for Warn Reset switch
        private bool DidManWarnReset;	//MI for Warn Reset switch


        private FaultClass mFaults;
        private CautionClass mCautions;


        public bool NeedAckAvioncFault;
        public bool IsFlagSet()
        {

            return mCautions.IsFlagSet();
        }
        public void ClearFlag()
        {
            Debug.WriteLine("remove call\n");
        }

        public void SetFault(FaultClass.type_FSubSystem subsystem,
                                      FaultClass.type_FFunction function,
                                      FaultClass.type_FSeverity severity,
                                      bool doWarningMsg)
        {
            if (!SimDriver.playerEntity) return;
            FaultClass.str_FEntry entry;

            Debug.Assert(doWarningMsg == false || SimDriver.playerEntity.mFaults == this); // should only apply to us
            GetFault(subsystem, out entry);

            // Set the fault
            mFaults.SetFault(subsystem, function, severity, doWarningMsg);

            // Adjust needed cautions
            if (entry.elFunction == FaultClass.type_FFunction.nofault)
            {
                if (subsystem == FaultClass.type_FSubSystem.eng_fault)
                {
                    mCautions.SetCaution(engine);
                }
                else if (subsystem == FaultClass.type_FSubSystem.iff_fault)
                {
                    mCautions.SetCaution(iff_fault);
                }
            }
            if (!g_bRealisticAvionics)
            {
                mMasterCaution = true;
                NeedsWarnReset = true; //MI Warn Reset
            }
            else if (doWarningMsg)
            {
                if (subsystem == FaultClass.type_FSubSystem.amux_fault ||
                    subsystem == FaultClass.type_FSubSystem.blkr_fault ||
                    subsystem == FaultClass.type_FSubSystem.bmux_fault ||
                    subsystem == FaultClass.type_FSubSystem.cadc_fault ||
                    subsystem == FaultClass.type_FSubSystem.cmds_fault ||
                    subsystem == FaultClass.type_FSubSystem.dlnk_fault ||
                    subsystem == FaultClass.type_FSubSystem.dmux_fault ||
                    subsystem == FaultClass.type_FSubSystem.dte_fault ||
                    subsystem == FaultClass.type_FSubSystem.eng_fault ||
                    subsystem == FaultClass.type_FSubSystem.epod_fault ||
                    subsystem == FaultClass.type_FSubSystem.fcc_fault ||
                    subsystem == FaultClass.type_FSubSystem.fcr_fault ||
                    subsystem == FaultClass.type_FSubSystem.flcs_fault ||
                    subsystem == FaultClass.type_FSubSystem.fms_fault ||
                    subsystem == FaultClass.type_FSubSystem.gear_fault ||
                    subsystem == FaultClass.type_FSubSystem.gps_fault ||
                    subsystem == FaultClass.type_FSubSystem.harm_fault ||
                    subsystem == FaultClass.type_FSubSystem.hud_fault ||
                    subsystem == FaultClass.type_FSubSystem.iff_fault ||
                    subsystem == FaultClass.type_FSubSystem.ins_fault ||
                    subsystem == FaultClass.type_FSubSystem.isa_fault ||
                    subsystem == FaultClass.type_FSubSystem.mfds_fault ||
                    subsystem == FaultClass.type_FSubSystem.msl_fault ||
                    subsystem == FaultClass.type_FSubSystem.ralt_fault ||
                    subsystem == FaultClass.type_FSubSystem.rwr_fault ||
                    subsystem == FaultClass.type_FSubSystem.sms_fault ||
                    subsystem == FaultClass.type_FSubSystem.tcn_fault ||
                    subsystem == FaultClass.type_FSubSystem.ufc_fault)
                {
                    SimDriver.playerEntity.NeedsToPlayCaution = true;//caution
                    SetMasterCaution();	//set our MasterCaution immediately
                    SimDriver.playerEntity.WhenToPlayCaution = vuxGameTime + 7 * CampaignSeconds;
                    NeedAckAvioncFault = true;
                }
                else
                {
                    /*//these are warnings
                    if(function == FaultClass.dual ||
                        function == FaultClass.efire ||
                        function == FaultClass.hydr)
                    { */
                    SimDriver.playerEntity.NeedsToPlayWarning = true;// warning
                    if (!SimDriver.playerEntity.NeedsToPlayWarning)
                        SimDriver.playerEntity.WhenToPlayWarning = vuxGameTime + (ulong)1.5 * CampaignSeconds;
                    SetWarnReset();
                }
            }
        }
        public void SetFault(int systemBits, bool doWarningMsg)
        {
            FaultClass.type_FSubSystem subSystem = mFaults.PickSubSystem(systemBits);
            FaultClass.type_FFunction function = mFaults.PickFunction(subSystem);

            SetFault(subSystem, function, FaultClass.type_FSeverity.fail, doWarningMsg);
        }
        public void SetFault(type_CSubSystem subsystem)
        {
            if (!SimDriver.playerEntity)
                return;

            Debug.Assert(SimDriver.playerEntity.mFaults == this); // should only apply to us
            if (!mCautions.GetCaution(subsystem))
            {
                mCautions.SetCaution(subsystem);

                // No Master Caution for low_altitude warming - just bitchin' betty :-) - RH
                if (subsystem != type_CSubSystem.alt_low)
                {
                    if (!g_bRealisticAvionics)
                    {
                        mMasterCaution = true;
                        NeedsWarnReset = true;
                    }
                    else
                    {
                        //these are warnings
                        if (GetFault(type_CSubSystem.tf_fail) ||	//never get's set currently
                            GetFault(type_CSubSystem.obs_wrn) || //never get's set currently
                            GetFault(type_CSubSystem.eng_fire) ||
                            GetFault(type_CSubSystem.hyd) ||
                            GetFault(type_CSubSystem.oil_press) ||
                            GetFault(type_CSubSystem.dual_fc) ||
                            GetFault(type_CSubSystem.to_ldg_config))
                        {
                            if (!SimDriver.playerEntity.NeedsToPlayWarning)
                                SimDriver.playerEntity.WhenToPlayWarning = vuxGameTime + (ulong)1.5 * CampaignSeconds;
                            SimDriver.playerEntity.NeedsToPlayWarning = true;// warning
                            SetWarnReset();
                        }
                        //these are actually cautions
                        if (subsystem != type_CSubSystem.fuel_low_fault)
                        {
                            if (GetFault(type_CSubSystem.stores_config_fault) ||
                                GetFault(type_CSubSystem.flt_cont_fault) ||
                                GetFault(type_CSubSystem.le_flaps_fault) ||
                                GetFault(type_CSubSystem.engine) ||
                                GetFault(type_CSubSystem.overheat_fault) ||
                                GetFault(type_CSubSystem.avionics_fault) ||
                                GetFault(type_CSubSystem.radar_alt_fault) ||
                                GetFault(type_CSubSystem.iff_fault) ||
                                GetFault(type_CSubSystem.ecm_fault) ||
                                GetFault(type_CSubSystem.hook_fault) ||
                                GetFault(type_CSubSystem.nws_fault) ||
                                GetFault(type_CSubSystem.cabin_press_fault) ||
                                GetFault(type_CSubSystem.fwd_fuel_low_fault) ||
                                GetFault(type_CSubSystem.aft_fuel_low_fault) ||
                                GetFault(type_CSubSystem.probeheat_fault) ||
                                GetFault(type_CSubSystem.seat_notarmed_fault) ||
                                GetFault(type_CSubSystem.buc_fault) ||
                                GetFault(type_CSubSystem.fueloil_hot_fault) ||
                                GetFault(type_CSubSystem.anti_skid_fault) ||
                                GetFault(type_CSubSystem.nws_fault) ||
                                GetFault(type_CSubSystem.oxy_low_fault) ||
                                GetFault(type_CSubSystem.sec_fault) ||
                                GetFault(type_CSubSystem.lef_fault))
                            {
                                if (!SimDriver.playerEntity.NeedsToPlayCaution &&
                                    !cockpitFlightData.IsSet(FlightData.MasterCaution))
                                {
                                    SimDriver.playerEntity.WhenToPlayCaution = vuxGameTime + 7 * CampaignSeconds;
                                }
                                SimDriver.playerEntity.NeedsToPlayCaution = true;//caution
                                SetMasterCaution();	//set our MasterCaution immediately
                            }
                        }
                        if (subsystem == type_CSubSystem.fuel_low_fault)	//MI need flashing on HUD
                            SetWarnReset();
                    }
                }
            }
        }

        public void ClearFault(FaultClass.type_FSubSystem subsystem)
        {

            mFaults.ClearFault(subsystem);

            if (subsystem == FaultClass.type_FSubSystem.eng_fault)
            {
                mCautions.ClearCaution(type_CSubSystem.engine);
            }
            else if (subsystem == FaultClass.type_FSubSystem.iff_fault)
            {
                mCautions.ClearCaution(type_CSubSystem.iff_fault);
            }
        }
        public void ClearFault(type_CSubSystem subsystem)
        {

            if (g_bRealisticAvionics)
            {
                //warnings
                if (!GetFault(type_CSubSystem.tf_fail) &&	//never get's set currently
                    !GetFault(type_CSubSystem.obs_wrn) && //never get's set currently
                    !GetFault(type_CSubSystem.eng_fire) &&
                    !GetFault(type_CSubSystem.hyd) &&
                    !GetFault(type_CSubSystem.oil_press) &&
                    !GetFault(type_CSubSystem.dual_fc) &&
                    !GetFault(type_CSubSystem.to_ldg_config) &&
                    !GetFault(type_CSubSystem.fuel_low_fault) &&
                    !GetFault(type_CSubSystem.fuel_trapped) &&
                    !GetFault(type_CSubSystem.fuel_home))
                {
                    ClearWarnReset();
                }
                //Cautions
                if (!GetFault(type_CSubSystem.stores_config_fault) &&
                    !GetFault(type_CSubSystem.flt_cont_fault) &&
                    !GetFault(type_CSubSystem.le_flaps_fault) &&
                    !GetFault(type_CSubSystem.engine) &&
                    !GetFault(type_CSubSystem.overheat_fault) &&
                    !GetFault(type_CSubSystem.avionics_fault) &&
                    !GetFault(type_CSubSystem.radar_alt_fault) &&
                    !GetFault(type_CSubSystem.iff_fault) &&
                    !GetFault(type_CSubSystem.ecm_fault) &&
                    !GetFault(type_CSubSystem.hook_fault) &&
                    !GetFault(type_CSubSystem.nws_fault) &&
                    !GetFault(type_CSubSystem.cabin_press_fault) &&
                    !GetFault(type_CSubSystem.fwd_fuel_low_fault) &&
                    !GetFault(type_CSubSystem.aft_fuel_low_fault) &&
                    !GetFault(type_CSubSystem.probeheat_fault) &&
                    !GetFault(type_CSubSystem.seat_notarmed_fault) &&
                    !GetFault(type_CSubSystem.buc_fault) &&
                    !GetFault(type_CSubSystem.fueloil_hot_fault) &&
                    !GetFault(type_CSubSystem.anti_skid_fault) &&
                    !GetFault(type_CSubSystem.nws_fault) &&
                    !GetFault(type_CSubSystem.oxy_low_fault) &&
                    !GetFault(type_CSubSystem.sec_fault) &&
                    !GetFault(type_CSubSystem.elec_fault) &&
                    !GetFault(type_CSubSystem.lef_fault) &&
                    !NeedAckAvioncFault)
                {
                    ClearMasterCaution();

                }
            }
            mCautions.ClearCaution(subsystem);
        }
        public void ClearFault(FaultClass.type_FSubSystem ss, FaultClass.type_FFunction type) { mFaults.ClearFault(ss, type); }

        public void ClearAvioncFault() { NeedAckAvioncFault = false; }

        public void GetFault(FaultClass.type_FSubSystem subsystem, out FaultClass.str_FEntry pentry)
        {

            mFaults.GetFault(subsystem, pentry);
        }
        public bool GetFault(FaultClass.type_FSubSystem subsystem)
        {

            return mFaults.GetFault(subsystem);
        }

        public bool GetFault(type_CSubSystem subsystem)
        {

            return mCautions.GetCaution(subsystem);
        }

        public int MasterCaution() { return mMasterCaution; }
        public int Breakable(FaultClass.type_FSubSystem id) { return mFaults.Breakable(id); }
        public void ClearMasterCaution() { mMasterCaution = false; }
        public void SetMasterCaution() { mMasterCaution = true; }
        public void TotalPowerFailure()
        {
            mFaults.TotalPowerFailure(); // JPO
            //MI need to route these thru the appropriate function
            //since we have electrics in non realistic mode, we have to go this way....
            if (g_bRealisticAvionics)
            {
                SetCaution(type_CSubSystem.radar_alt_fault);
                SetCaution(type_CSubSystem.le_flaps_fault);
                SetCaution(type_CSubSystem.hook_fault);
                SetCaution(type_CSubSystem.nws_fault);
                SetCaution(type_CSubSystem.ecm_fault);
                SetCaution(type_CSubSystem.iff_fault);
            }
            else
            {
                mCautions.SetCaution(type_CSubSystem.radar_alt_fault);
                mCautions.SetCaution(type_CSubSystem.le_flaps_fault);
                mCautions.SetCaution(type_CSubSystem.hook_fault);
                mCautions.SetCaution(type_CSubSystem.nws_fault);
                mCautions.SetCaution(type_CSubSystem.ecm_fault);
                mCautions.SetCaution(type_CSubSystem.iff_fault);
            }

            if (!g_bRealisticAvionics)
                mMasterCaution = true;
            /*if(!SimDriver.playerEntity.NeedsToPlayCaution)
                SimDriver.playerEntity.WhenToPlayCaution = vuxGameTime + 7*CampaignSeconds;
            SimDriver.playerEntity.NeedsToPlayCaution = true;*/
        }
        public bool WarnReset() { return NeedsWarnReset; }	//MI
        public bool DidManWarn() { return DidManWarnReset; }	//MI
        public void SetManWarnReset() { DidManWarnReset = true; }	//MI
        public void ClearManWarnReset() { DidManWarnReset = false; }	//MI
        public void ClearWarnReset() { NeedsWarnReset = false; }
        public void SetWarnReset() { NeedsWarnReset = true; }
        public int GetFFaultCount() { return mFaults.GetFaultCount(); }
        //MI
        public void SetWarning(type_CSubSystem subsystem)
        {
            if (!SimDriver.playerEntity)
                return;

            ShiAssert(SimDriver.playerEntity.mFaults == this); // should only apply to us
            if (!mCautions.GetCaution(subsystem))
            {
                mCautions.SetCaution(subsystem);

                if (!SimDriver.playerEntity.NeedsToPlayWarning)
                    SimDriver.playerEntity.WhenToPlayWarning = vuxGameTime + (ulong)1.5 * CampaignSeconds;
                if (!GetFault(type_CSubSystem.fuel_low_fault) &&
                    !GetFault(type_CSubSystem.fuel_home))//no betty for bingo
                    SimDriver.playerEntity.NeedsToPlayWarning = true;// warning
                SetWarnReset();
            }
        }
        public void SetCaution(type_CSubSystem subsystem)
        {
            if (!SimDriver.playerEntity)
                return;

            ShiAssert(SimDriver.playerEntity.mFaults == this); // should only apply to us
            if (!mCautions.GetCaution(subsystem))
            {
                mCautions.SetCaution(subsystem);

                if (!SimDriver.playerEntity.NeedsToPlayCaution &&
                    !cockpitFlightData.IsSet(FlightData.MasterCaution))
                {
                    SimDriver.playerEntity.WhenToPlayCaution = vuxGameTime + 7 * CampaignSeconds;
                }
                SimDriver.playerEntity.NeedsToPlayCaution = true;//caution
                SetMasterCaution();	//set our MasterCaution immediately
            }
        }

        public void GetFaultNames(FaultClass.type_FSubSystem subsystem, int funcNum, FaultClass.str_FNames* pnames)
        {

            mFaults.GetFaultNames(subsystem, funcNum, pnames);
        }
        public void AddTakeOff(VU_TIME thetime) { mFaults.AddMflList(thetime, FaultClass.type_FSubSystem.takeoff, 0); }
        public void AddLanding(VU_TIME thetime) { mFaults.AddMflList(thetime, FaultClass.type_FSubSystem.landing, 0); }
        public void SetStartTime(VU_TIME thetime) { mFaults.SetStartTime(thetime); }
        public bool GetMflEntry(int n, out string name, out int subsys, out int count, char[] timestr)
        {
            return mFaults.GetMflEntry(n, name, subsys, count, timestr);
        }
        public int GetMflListCount() { return mFaults.GetMflListCount(); }
        public void ClearMfl() { mFaults.ClearMfl(); }
        public bool FindFirstFunction(FaultClass.type_FSubSystem sys, int* functionp)
        { return mFaults.FindFirstFunction(sys, functionp); }
        public bool FindNextFunction(FaultClass.type_FSubSystem sys, int* functionp)
        { return mFaults.FindNextFunction(sys, functionp); }
        public bool GetFirstFault(FaultClass.type_FSubSystem* subsystemp, int* functionp)
        { return mFaults.GetFirstFault(subsystemp, functionp); }
        public bool GetNextFault(FaultClass.type_FSubSystem* subsystemp, int* functionp)
        { return mFaults.GetNextFault(subsystemp, functionp); }

        public FackClass()
        {

            mMasterCaution = 0;
            NeedsWarnReset = 0;	//MI Warn reset switch
            DidManWarnReset = 0;	//MI Warn reset switch
            NeedAckAvioncFault = FALSE;
        }

        // TODO ~FackClass();
    }
}
