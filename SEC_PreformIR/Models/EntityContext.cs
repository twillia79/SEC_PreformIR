using Microsoft.EntityFrameworkCore;

namespace SEC_PreformIR.Models
{
    public class EntityContext : DbContext
    {

        protected readonly IConfiguration Configuration;

        public EntityContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer("Data Source=oh-dc1; Initial Catalog=TN; User Id=tndbuser; Password=tndbuser");
        }

        public DbSet<TemperatureModel> Temperatures { get; set; }
        //public DbSet<MachineModel> Machines { get; set; }
    }
}
