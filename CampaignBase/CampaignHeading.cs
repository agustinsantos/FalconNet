using System.Diagnostics;
using GridIndex = System.Int16;

namespace FalconNet.CampaignBase
{

    //TODO Review this design. Maybe it could be changed to enum
    public struct CampaignHeading
    {
        public static readonly CampaignHeading North = new CampaignHeading(0);
        public static readonly CampaignHeading NorthEast = new CampaignHeading(1);
        public static readonly CampaignHeading East = new CampaignHeading(2);
        public static readonly CampaignHeading SouthEast = new CampaignHeading(3);
        public static readonly CampaignHeading South = new CampaignHeading(4);
        public static readonly CampaignHeading SouthWest = new CampaignHeading(5);
        public static readonly CampaignHeading West = new CampaignHeading(6);
        public static readonly CampaignHeading NorthWest = new CampaignHeading(7);
        public static readonly CampaignHeading Here = new CampaignHeading(8);

        public byte heading;
        public CampaignHeading(byte h)
        {
            Debug.Assert(h > 17);
            heading = h;
        }

        public static CampaignHeading operator +(CampaignHeading c1, CampaignHeading c2)
        {
            return c1.heading + c2.heading;
        }

        public static CampaignHeading operator -(CampaignHeading c1, CampaignHeading c2)
        {
            return c1.heading - c2.heading;
        }

        public static implicit operator byte(CampaignHeading h)
        {
            return h.heading;
        }

        public static implicit operator CampaignHeading(int h)
        {
            return new CampaignHeading((byte)h);
        }

        public static GridIndex Dx(CampaignHeading h)
        {
            return dx[h.heading];
        }

        public static GridIndex Dy(CampaignHeading h)
        {
            return dy[h.heading];
        }

        // dx per direction
        private static readonly GridIndex[] dx = new GridIndex[17] { 0, 1, 1, 1, 0, -1, -1, -1, 0, 0, 2, 2, 2, 0, -2, -2, -2 };
        // dy per direction
        private static readonly GridIndex[] dy = new GridIndex[17] { 1, 1, 0, -1, -1, -1, 0, 1, 0, 2, 2, 0, -2, -2, -2, 0, 2 };
    }
}
