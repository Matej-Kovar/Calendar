using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace Calendar.Models
{
    public class DayEvent
    {
        public DayEvent(DateTime starDate, DateTime endTime, string name)
        {
            StarDate = starDate;
            EndDate = endTime;
            Name = name;
            Color temp = (Color)Application.Current.Resources["PrimaryColor"];
            ColorHex = temp.ToArgbHex();
        }
        public DayEvent() { }
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime StarDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Name { get; set; }
        //public List<EventTag> Tags { get; set; } = new List<EventTag>();
        public string? Description { get; set; }
        public string? Place { get; set; }
        public List<int> RepeatAfter { get; set; } = new List<int>();
        public string ColorHex { get; set; }

    }
}
