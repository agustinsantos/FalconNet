using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace FalconNet.Sim
{
    //-------------------------------------------------
    // A whole slew of caution lights
    //-------------------------------------------------

    public enum type_CSubSystem
    {
        tf_fail,
        obs_wrn,
        alt_low,
        eng_fire,
        engine,
        hyd,
        oil_press,
        dual_fc,
        canopy,
        to_ldg_config,
        flt_cont_fault,
        le_flaps_fault,
        overheat_fault,
        fuel_low_fault,
        avionics_fault,
        radar_alt_fault,
        iff_fault,
        ecm_fault,
        hook_fault,
        nws_fault,
        cabin_press_fault,
        oxy_low_fault,
        fwd_fuel_low_fault,
        aft_fuel_low_fault,
        fuel_trapped,
        fuel_home,
        sec_fault,
        probeheat_fault,
        stores_config_fault,
        buc_fault,
        fueloil_hot_fault,
        anti_skid_fault,
        seat_notarmed_fault,
        equip_host_fault,
        elec_fault,
        lef_fault,	//MI

        lastFault
    };

    //-------------------------------------------------
    // A whole slew of threat warning lights
    //-------------------------------------------------

    public enum type_TWSubSystem
    {
        handoff, missile_launch,
        pri_mode, sys_test,
        tgt_t, unk,
        search, activate_power,
        system_power, low_altitude
    };




    //-------------------------------------------------
    // Class Defintion
    //-------------------------------------------------

    public class CautionClass
    {
        public const int BITS_PER_VECTOR = 32;

        public const int NumVectors = ((int)type_CSubSystem.lastFault / BITS_PER_VECTOR) + 1;

        private uint[] mpBitVector = new uint[NumVectors];

        private void SetCaution(int subsystem)
        {
            int vectorNum = 0;
            int bitNum;

            vectorNum = subsystem / BITS_PER_VECTOR;
            bitNum = subsystem - vectorNum * BITS_PER_VECTOR;

            mpBitVector[vectorNum] |= (uint)(0x01 << bitNum);
        }

        private void ClearCaution(int subsystem)
        {

            int vectorNum = 0;
            int bitNum;

            vectorNum = subsystem / BITS_PER_VECTOR;
            bitNum = subsystem - vectorNum * BITS_PER_VECTOR;

            mpBitVector[vectorNum] &= (uint)(~(0x01 << bitNum));

        }

        private bool GetCaution(int subsystem)
        {

            int vectorNum = 0;
            int bitNum;

            vectorNum = subsystem / BITS_PER_VECTOR;
            bitNum = subsystem - vectorNum * BITS_PER_VECTOR;

            return ((mpBitVector[vectorNum] & (0x01 << bitNum)) != 0);
        }



        public bool IsFlagSet()
        {
            int i;
            uint flag = 0;


            for (i = 0; i < NumVectors; i++)
            {
                flag |= mpBitVector[i];
            }

            return (flag != 0);
        }

        public void ClearFlag()
        {
            Debug.WriteLine("remove call\n");
        }

        public void SetCaution(type_CSubSystem subsystem)
        {
            SetCaution((int)subsystem);
        }

        public void SetCaution(type_TWSubSystem subsystem)
        {
            SetCaution((int)subsystem);
        }

        public void ClearCaution(type_CSubSystem subsystem)
        {
            ClearCaution((int)subsystem);
        }

        public void ClearCaution(type_TWSubSystem subsystem)
        {
            ClearCaution((int)subsystem);
        }

        public bool GetCaution(type_CSubSystem subsystem)
        {
            return GetCaution((int)subsystem);
        }

        public bool GetCaution(type_TWSubSystem subsystem)
        {
            return GetCaution((int)subsystem);
        }

        public CautionClass()
        {
            int i;

            for (i = 0; i < NumVectors; i++)
            {
                mpBitVector[i] = 0x0;
            }
        }
    }
}
