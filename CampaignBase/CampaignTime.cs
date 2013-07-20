
namespace FalconNet.CampaignBase
{
    public struct CampaignTime
    {
        public ulong time;

        //TODO this must be defined as time interval
        public const ulong CampaignSeconds = 1000;
        public const ulong CampaignMinutes = 60000;
        public const ulong CampaignHours = 3600000;
        public const ulong CampaignDay = 86400000;
        public const ulong INFINITE_TIME = 4294967295;		// Max value of CampaignTime

        public CampaignTime(ulong t)
        {
            time = t;
        }

        public static CampaignTime operator +(CampaignTime c1, CampaignTimeInterval c2)
        {
            return new CampaignTime(c1.time + c2.interval);
        }

        public static CampaignTime operator -(CampaignTime c1, CampaignTimeInterval c2)
        {
            return new CampaignTime(c1.time - c2.interval);
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

    public struct CampaignTimeInterval
    {
        public ulong interval;

        public CampaignTimeInterval(ulong i)
        {
            interval = i;
        }

        public static implicit operator ulong(CampaignTimeInterval t)
        {
            return t.interval;
        }

        public static implicit operator CampaignTimeInterval(ulong t)
        {
            return new CampaignTimeInterval(t);
        }
    }
}
