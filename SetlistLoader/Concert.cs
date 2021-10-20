using System;
using System.Collections.Generic;

namespace SetlistLoader
{
    public class Concert
    {
        public string Artist { get; set; }

        public DateTime Date { get; set; }

        public Venue Venue { get; set; }

        public List<string> Setlist { get; set; }
    }
}
