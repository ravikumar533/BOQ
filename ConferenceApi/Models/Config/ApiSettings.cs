using System;
namespace ConferenceApi.Models.Config
{
    public class ApiSettings
    {
        public CacheSettings ApiCacheSettings { get; set; }
        public ConferenceKeySettings ApiConferenceKeySettings { get; set; }
        public ConferenceRouteSettings ApiConferenceRouteSettings { get; set; }
        public AuthSettings ApiAuthSettings { get; set; }
    }
}
