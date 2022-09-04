using RestaurantBLL;
using RestaurantBLL.Interface;
using RestaurantDAL;
using RestaurantDAL.Interface;

namespace RestaurantWebAPI.Extensions
{
    public static class IServiceCollectionExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddContext(this IServiceCollection services)
        {
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            return services;
        }

        public static IServiceCollection AddDAL(this IServiceCollection services)
        {
            services.AddTransient<IManageRestaurantDAL, ManageRestaurantDAL>();
            return services;
        }

        public static IServiceCollection AddBLL(this IServiceCollection services)
        {
            services.AddTransient<IManageRestaurantBLL, ManageRestaurantBLL>();
            return services;
        }
    }
}
