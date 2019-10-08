using System;
using System.Collections.Generic;

namespace ConferenceApi.Models
{
    public class SpeakerSessionCollection
    {
        public List<ResponseItem> SpeakersList { get; set; }
        public List<ResponseItem> SessionsList { get; set; }
    }
}
