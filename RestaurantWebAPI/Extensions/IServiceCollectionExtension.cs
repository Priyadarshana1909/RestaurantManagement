using RestaurantBLL;
using RestaurantBLL.Interface;
using RestaurantDAL;
using RestaurantDAL.Interface;

namespace RestaurantWebAPI.Extensions
{
    /// <summary>
    /// Service collection extension
    /// used in resolving dependancy in start up
    /// </summary>
    public static class IServiceCollectionExtension
    {
        #region Http Context
        /// <summary>
        /// Extension method for adding context
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddContext(this IServiceCollection services)
        {
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            return services;
        }
        #endregion

        #region "DAL"
        /// <summary>
        /// Extension method for resolving DAL dependancy
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddDAL(this IServiceCollection services)
        {
            services.AddTransient<IManageReportDAL, ManageReportDAL>();
            services.AddTransient<IManageBillDAL, ManageBillDAL>();
            services.AddTransient<IManageDiningTableDAL, ManageDiningTableDAL>();
            services.AddTransient<IManageOrderDAL, ManageOrderDAL>();
            services.AddTransient<IManageMenuItemDAL, ManageMenuItemDAL>();
            services.AddTransient<IManageRestaurantDAL, ManageRestaurantDAL>();
            services.AddTransient<IManageCuisineDAL, ManageCuisineDAL>();
            services.AddTransient<IManageCustomerDAL, ManageCustomerDAL>();
            return services;
        }
        #endregion

        #region "BLL"
        /// <summary>
        /// Extension method for resolving BLL dependancy
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddBLL(this IServiceCollection services)
        {
            services.AddTransient<IManageReportBLL, ManageReportBLL>();
            services.AddTransient<IManageBillBLL, ManageBillBLL>();
            services.AddTransient<IManageDiningTableBLL, ManageDiningTableBLL>();
            services.AddTransient<IManageOrderBLL, ManageOrderBLL>();
            services.AddTransient<IManageMenuItemBLL, ManageMenuItemBLL>();
            services.AddTransient<IManageRestaurantBLL, ManageRestaurantBLL>();
            services.AddTransient<IManageCuisineBLL, ManageCuisineBLL>();
            services.AddTransient<IManageCustomerBLL, ManageCustomerBLL>();
            return services;
        }
        #endregion
    }
}
