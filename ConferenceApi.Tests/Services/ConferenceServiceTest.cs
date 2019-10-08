using ConferenceApi.Models;
using ConferenceApi.Models.Config;
using ConferenceApi.Services;
using ConferenceApi.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ConferenceApi.Tests.Services
{
    public class ConferenceServiceTest
    {
        private IHttpClientService httpClientService;
        private IOptions<ConferenceRouteSettings> conferenceRouteSettings;
        private IOptions<CacheSettings> cacheSettings;
        private IMemoryCache cache;
        private ConferenceService conferenceService;
        public ConferenceServiceTest()
        {
            ConferenceRouteSettings _conferenceSettings = new ConferenceRouteSettings() {
                BaseUrl = "",
                SessionsListUrl ="sessions",
                SessionUrl = "session",
                SpeakersListUrl="speakers"
            };
            CacheSettings cachesettings = new CacheSettings()
            {
                CacheExpiryInMintues = 2
            };
            var confRouteOptions = Options.Create(_conferenceSettings);
            httpClientService = Substitute.For<IHttpClientService>();
            conferenceRouteSettings = Options.Create(_conferenceSettings);
            cacheSettings = Options.Create(cachesettings);
            cache = Substitute.For<IMemoryCache>();
            conferenceService = new ConferenceService(httpClientService, conferenceRouteSettings, cacheSettings, cache);
        }
        [Fact]
        public async void GetSpeakerSessionCollection_OkResult() {
            var sessionObject = new RootObject()
            {
                Collection = new Collection()
                {
                    Items = new List<ResponseItem>()
                    {
                        new ResponseItem()
                        {
                            Id = 1
                        },
                        new ResponseItem()
                        {
                            Id = 2
                        },
                        new ResponseItem()
                        {
                            Id = 3
                        }
                    }
                }
            };
            var speakerObject = new RootObject()
            {
                Collection = new Collection()
                {
                    Items = new List<ResponseItem>()
                    {
                        new ResponseItem()
                        {
                            Id = 1
                        },
                        new ResponseItem()
                        {
                            Id = 2
                        }
                    }
                }
            };

            httpClientService.GetAsync<RootObject>("sessions").Returns<RootObject>(sessionObject);
            httpClientService.GetAsync<RootObject>("speakers").Returns<RootObject>(speakerObject);

            
            var result = await conferenceService.GetCollectionAsync();
            var model = Assert.IsAssignableFrom<SpeakerSessionCollection>(
                result);
            Assert.Equal(3, model.SessionsList.Count);
            Assert.Equal(2, model.SpeakersList.Count);
        }

        [Fact]
        public async void GetSession_OkResult()
        {
            var sessionObject = new RootObject()
            {
                Collection = new Collection()
                {
                    Items = new List<ResponseItem>()
                    {
                        new ResponseItem()
                        {
                            Id = 1,
                            Href="session/1"
                        },
                        new ResponseItem()
                        {
                            Id = 2,
                             Href="session/2"
                        },
                        new ResponseItem()
                        {
                            Id = 3,
                             Href="session/3"
                        }
                    }
                }
            };

            httpClientService.GetAsync<RootObject>("sessions").Returns<RootObject>(sessionObject);
            httpClientService.GetStringAsAsync("session/1").Returns("Testing");

            var result = await conferenceService.GetSession(1);
            var model = Assert.IsAssignableFrom<Session>(
                result);
            Assert.Equal(1, model.Id);
            Assert.Equal("Testing", model.Description);
        }
    }
}
