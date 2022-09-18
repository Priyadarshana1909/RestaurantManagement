using Microsoft.Extensions.Configuration;

namespace RestaurantDAL
{
    public static class Common
    {
        public static string GetConnectionString()
        {
            return new ConfigurationBuilder().AddJsonFile("appsettings.json")
               .Build().GetConnectionString("DefaultConnection");
        }
    }
}
