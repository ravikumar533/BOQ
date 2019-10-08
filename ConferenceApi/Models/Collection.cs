using System;
using System.Collections.Generic;

namespace ConferenceApi.Models
{
    public class Collection
    {
        public string Version { get; set; }
        public List<object> Links { get; set; }
        public List<ResponseItem> Items { get; set; }
        public List<object> Queries { get; set; }
        public Template Template { get; set; }
    }
}
