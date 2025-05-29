using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calendar.Model
{
    public class DayEvent
    {
        public DayEvent(DateTime starDate, DateTime endTime, string name)
        {
            StarDate = starDate;
            EndDate = endTime;
            Name = name;
        }

        public DateTime StarDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Name { get; set; }
        //public List<EventTag> Tags { get; set; } = new List<EventTag>();
        public string? Description { get; set; }
        public string? Place { get; set; }
        public int? RepeatAfter {  get; set; }
        public Color Color { get; set; } = Colors.Black;

        public bool isInRange(DateTime date)
        {
            return date.Date >= StarDate.Date && date.Date <= EndDate.Date;
        }
    }
}
