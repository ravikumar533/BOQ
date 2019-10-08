using System;
using System.Net.Http;
using System.Threading.Tasks;
using ConferenceApi.Services.Interfaces;
using Newtonsoft.Json;

namespace ConferenceApi.Services
{
    public class HttpClientService : IHttpClientService
    {
        private HttpClient httpClient;
        public HttpClientService(HttpClient client)
        {
            httpClient = client;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestUrl"></param>
        /// <returns></returns>
        public async Task<T> GetAsync<T>(string requestUrl)
        {
            var responseBody = await httpClient.GetStringAsync(requestUrl);
            return JsonConvert.DeserializeObject<T>(responseBody);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestUrl"></param>
        /// <returns></returns>
        public async Task<string> GetStringAsAsync(string requestUrl)
        {
            return await httpClient.GetStringAsync(requestUrl);
        }
    }
}
