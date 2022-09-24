using RestaurantWebApplication.Services;
using RestaurantWebApplication.Services.Interface;
using RestSharp;

namespace RestaurantWebApplication.Extensions
{
    /// <summary>
    /// Service collection extension
    /// used in resolving dependancy in start up
    /// </summary>
    public static class IServiceCollectionExtension
    {
        #region Dependancy resolver for rest client and api service
        /// <summary>
        /// Extension method for resolving dependancy in start up
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddRestClientAndAPIService(this IServiceCollection services)
        {
            services.AddTransient<IRestClient, RestClient>();
            services.AddTransient<IRestRequest, RestRequest>();
            services.AddTransient<IAPIService, APIService>();
            return services;
        }
        #endregion
    }
}
