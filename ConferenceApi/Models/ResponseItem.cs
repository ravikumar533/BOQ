using System;
using System.Collections.Generic;

namespace ConferenceApi.Models
{
    public class ResponseItem
    {
        public int Id { get; set; }
        public string Href { get; set; }
        public List<Datum> Data { get; set; }
        public List<Link> Links { get; set; }
    }
}
