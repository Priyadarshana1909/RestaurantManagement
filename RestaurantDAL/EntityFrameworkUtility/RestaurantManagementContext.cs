using Microsoft.EntityFrameworkCore;

namespace RestaurantDAL.EntityFrameworkUtility
{
    public class RestaurantManagementContext : DbContext
    {
        public RestaurantManagementContext(DbContextOptions<RestaurantManagementContext> options) : base(options)
        {
        }

        public RestaurantManagementContext()
        {

        }

        public DbSet<Restaurant> Restaurant { get; set; }

        public DbSet<RestaurantMenuItem> RestaurantMenuItem { get; set; }

        public DbSet<Order> Order { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //var configuration = new ConfigurationBuilder()
            //    .SetBasePath(Directory.GetCurrentDirectory())
            //    .AddJsonFile("appsettings.json")
            //    .Build();

            //optionsBuilder.AddDbContext<RestaurantManagementContext>(x => x.UseSqlServer(connectionString));
        }
    }
}
