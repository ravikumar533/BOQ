using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ConferenceApi.Models;
using ConferenceApi.Models.Config;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Linq;
using ConferenceApi.Services.Interfaces;
using ConferenceApi.Constants;

namespace ConferenceApi.Services
{
    public class ConferenceService : IConferenceService
    {
        private IHttpClientService httpClientService;
        private ConferenceRouteSettings conferenceRouteSettings;
        private CacheSettings cacheSettings;
        private IMemoryCache cache;
        public ConferenceService(IHttpClientService clientService,

            IOptions<ConferenceRouteSettings> confRouteSettings,
            IOptions<CacheSettings> cachesettings,
            IMemoryCache memoryCache)
        {
            httpClientService = clientService;
            conferenceRouteSettings = confRouteSettings.Value;
            cache = memoryCache;
            cacheSettings = cachesettings.Value;
        }

        public async Task<SpeakerSessionCollection> GetCollectionAsync()
        {   
            var sessionResponse = await httpClientService.GetAsync<RootObject>($"{conferenceRouteSettings.BaseUrl}{conferenceRouteSettings.SessionsListUrl}");
            var speakerResponse = await httpClientService.GetAsync<RootObject>($"{conferenceRouteSettings.BaseUrl}{conferenceRouteSettings.SpeakersListUrl}");

            var result = new SpeakerSessionCollection()
            {
                SessionsList = sessionResponse.Collection?.Items,
                SpeakersList = speakerResponse.Collection?.Items
            };
            return result;
            
        }

        public async Task<Session> GetSession(int sessionId)
        {
            
            var sessionResponse = await httpClientService.GetStringAsAsync($"{conferenceRouteSettings.BaseUrl}{conferenceRouteSettings.SessionUrl}/{sessionId}");
               
            var sessions = await GetSessionsFromCache();

            var selectedSession = sessions.Where(s => s.Id == sessionId)
                                        .Select(s =>
                                                new Session
                                                {
                                                    Data = s.Data,
                                                    Href = s.Href,
                                                    Id = s.Id,
                                                    Links = s.Links
                                                }
                                                ).FirstOrDefault();
            if (selectedSession != null)
                selectedSession.Description = sessionResponse;

            return selectedSession;
            
        }
        private async Task<List<ResponseItem>> GetSessionsFromCache()
        {
            List<ResponseItem> cacheSessions;
            if (!cache.TryGetValue(CacheKey.SessionCollections, out cacheSessions))
            {
                // Key not in cache, so get data.
                var rootObject = await httpClientService.GetAsync<RootObject>($"{conferenceRouteSettings.BaseUrl}{conferenceRouteSettings.SessionsListUrl}");
                var result = rootObject.Collection?.Items;
                cacheSessions = new List<ResponseItem>();
                foreach (var cacheitem in result) {
                    string[] urlsegements = (Uri.IsWellFormedUriString(cacheitem.Href, UriKind.Absolute)) ? new Uri(cacheitem.Href).Segments : cacheitem.Href.Split("/");
                    int sessionId;
                    if(urlsegements.Length > 0)
                    if (int.TryParse(urlsegements[urlsegements.Length - 1], out sessionId))
                    {
                        if (sessionId > 0)
                            cacheitem.Id = sessionId;
                    }

                    cacheSessions.Add(cacheitem);
                }

                // Set cache options.
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(cacheSettings.CacheExpiryInMintues));

                // Save data in cache.
                cache.Set(CacheKey.SessionCollections, cacheSessions, cacheEntryOptions);
            }
            return cacheSessions;
        }
    }
}
