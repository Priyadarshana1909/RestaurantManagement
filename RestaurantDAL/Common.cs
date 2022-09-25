using Microsoft.Extensions.Configuration;

namespace RestaurantDAL
{
    public static class Common
    {
        /// <summary>
        /// Function to get the connectionstring from appsettings.json
        /// </summary>
        /// <returns></returns>
        public static string GetConnectionString()
        {
            return new ConfigurationBuilder().AddJsonFile("appsettings.json")
               .Build().GetConnectionString("DefaultConnection");
        }
    }
}
