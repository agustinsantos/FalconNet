using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace FalconDB
{
    public class FalconDBContext : DbContext
    {
        public FalconDBContext()
            : base("FalconDBContext")
        {
        }

        public DbSet<PlayerPilot> PlayerPilots { get; set; }
        public DbSet<PilotInfo> NonPlayerPilots { get; set; }
 
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
