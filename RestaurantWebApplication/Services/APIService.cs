using RestaurantWebApplication.Services.Interface;
using RestSharp;

namespace RestaurantWebApplication.Services
{
    /// <summary>
    /// API Service
    /// </summary>
    public class APIService : IAPIService
    {
        private IRestClient _restclient;
        private IRestRequest _restRequest;
        protected IConfiguration _configuration { get; set; }

        /// <summary>
        /// Constructor of dependacy resolver
        /// </summary>
        /// <param name="restclient"></param>
        /// <param name="restRequest"></param>
        /// <param name="Configuration"></param>
        public APIService(IRestClient restclient, IRestRequest restRequest, IConfiguration Configuration)
        {
            _restclient = restclient;
            _restRequest = restRequest;
            _configuration = Configuration;
        }

        #region Generic Execute Request Method
        /// <summary>
        /// Generic method to communicate with web api
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uri">Web API Endpoint Url</param>
        /// <param name="method">Http method type</param>
        /// <param name="requestObject">Request Object</param>
        /// <returns></returns>
        public async Task<T> ExecuteRequest<T>(string uri, HttpMethod method, object? requestObject = null)
        {
            try
            {
                _restclient.BaseUrl = new Uri(GetBaseUrl());

                _restRequest.Resource = uri;
                _restRequest.Method = GetRestClientMethod(method);
                _restRequest.RequestFormat = DataFormat.Json;
                _restRequest.Parameters?.Clear();

                if (requestObject != null)
                {
                    _restRequest.AddJsonBody(requestObject);
                }
                var response = await _restclient.ExecuteAsync<T>(_restRequest);
                ValidateResponse<T>(response);
                return response.Data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Private methods
        private Method GetRestClientMethod(HttpMethod method)
        {
            var httpMethod = method.ToString().ToLower();
            switch (httpMethod)
            {
                case "post":
                    return Method.POST;
                case "get":
                    return Method.GET;
                case "put":
                    return Method.PUT;
                case "delete":
                    return Method.DELETE;
                case "patch":
                    return Method.PATCH;
            }

            throw new Exception("invalid method");
        }

        private string GetBaseUrl()
        {
            return _configuration["APISettings:APIUrl"];
        }

        // validate the data
        private void ValidateResponse<T>(IRestResponse<T> response)
        {
            if (response != null)
            {
                if (response.ErrorException != null)
                {
                    // it's really gone wrong "Machine Actively Refused connection" type errors.
                    // or any other domain type issue
                    throw response.ErrorException;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    response.Data = default(T);
                    throw new Exception(response.Content);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    response.Data = default(T);
                    throw new Exception(response.Content);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    response.Data = default(T);
                    throw new Exception(response.StatusDescription);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    response.Data = default(T);
                    throw new Exception(response.Content);
                }
            }
        }
        #endregion
    }
}
