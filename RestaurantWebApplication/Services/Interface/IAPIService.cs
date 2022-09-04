namespace RestaurantWebApplication.Services.Interface
{
    public interface IAPIService
    {
        Task<T> ExecuteRequest<T>(string uri, HttpMethod method
         , object? requestObject = null);
    }
}
