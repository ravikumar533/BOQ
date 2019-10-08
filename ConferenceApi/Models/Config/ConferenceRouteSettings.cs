using System;
namespace ConferenceApi.Models.Config
{
    public class ConferenceRouteSettings
    {
        public string BaseUrl { get; set; }
        public string SessionsListUrl { get; set; }
        public string SessionUrl { get; set; }
        public string SpeakersListUrl { get; set; }
    }
}
