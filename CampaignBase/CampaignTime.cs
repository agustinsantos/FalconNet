
namespace FalconNet.CampaignBase
{
    public struct CampaignTime
    {
        public ulong time;
        public const ulong CampaignSeconds = 1000;
        public const ulong CampaignMinutes = 60000;
        public const ulong CampaignHours = 3600000;
        public const ulong CampaignDay = 86400000;
        public const ulong INFINITE_TIME = 4294967295;		// Max value of CampaignTime

        public CampaignTime(ulong t)
        {
            time = t;
        }

        public static CampaignTime operator +(CampaignTime c1, CampaignTime c2)
        {
            return new CampaignTime(c1.time + c2.time);
        }

        public static CampaignTime operator -(CampaignTime c1, CampaignTime c2)
        {
            return new CampaignTime(c1.time - c2.time);
        }

        public static implicit operator ulong(CampaignTime t)
        { 
            return t.time;
        }

        public static implicit operator CampaignTime(ulong t)
        {
            return new CampaignTime(t);
        }
    }


}
