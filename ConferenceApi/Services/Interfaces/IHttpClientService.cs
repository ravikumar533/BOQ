using System;
using System.Threading.Tasks;

namespace ConferenceApi.Services.Interfaces
{
    public interface IHttpClientService
    {
        Task<T> GetAsync<T>(string requestUrl);
        Task<string> GetStringAsAsync(string requestUrl);
    }
}
