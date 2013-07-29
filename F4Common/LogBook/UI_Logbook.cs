using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FalconNet.F4Common
{
    // TODO 
    // These definitions are originally at ui_lgbk.cpp 
    // translated here to avoid ciclic dependence
    public static class UI_Logbook
    {
        public static string gStringMgr;

        public static long[] gFullRanksTxt = new long[(int)LB_RANK.NUM_RANKS] { 
                                    TextIds.TXT_SEC_LT,
                                    TextIds.TXT_LEIUTENANT,
                                    TextIds.TXT_CAPTAIN,
                                    TextIds.TXT_MAJOR,
                                    TextIds.TXT_LT_COL,
                                    TextIds.TXT_COLONEL,
                                    TextIds.TXT_BRIG_GEN,
                                };

        public static long[] gRanksTxt = new long[(int)LB_RANK.NUM_RANKS] {
                                    TextIds.TXT_ABBRV_RANK_SEC_LT,
                                    TextIds.TXT_ABBRV_RANK_LIEUTENANT,
                                    TextIds.TXT_ABBRV_RANK_CAPTAIN,
                                    TextIds.TXT_ABBRV_RANK_MAJOR,
                                    TextIds.TXT_ABBRV_RANK_LT_COL,
                                    TextIds.TXT_ABBRV_RANK_COLONEL,
                                    TextIds.TXT_ABBRV_RANK_BRIG_GEN,
                                };
    }
}
