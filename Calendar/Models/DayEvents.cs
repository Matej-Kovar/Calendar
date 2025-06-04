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
        public DayEvent() { }
        public DayEvent(DateTime starDate, DateTime endTime, string name)
        {
            StarDate = starDate;
            EndDate = endTime;
            Name = name;
            Color temp = (Color)Application.Current.Resources["PrimaryColor"];
            ColorHex = temp.ToArgbHex();
        }

        //public Guid Id { get; } = Guid.NewGuid();
        public DateTime StarDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Name { get; set; }
        //public List<EventTag> Tags { get; set; } = new List<EventTag>();
        public string? Description { get; set; }
        public string? Place { get; set; }
        public List<int> RepeatAfter { get; set; } = new List<int>();
        public string ColorHex { get; set; }

        [JsonIgnore]
        public Color Color
        {
            get => Color.FromArgb(ColorHex);
            set => ColorHex = value.ToArgbHex();
        }

        public bool isInRange(DateTime date)
        {
            date = date.Date;
            if (RepeatAfter == null || RepeatAfter.Count == 0)
            {
                return date >= StarDate.Date && date <= EndDate.Date;
            }

            if (RepeatAfter.All(x => x == 0))
                return date >= StarDate.Date && date <= EndDate.Date;

            DateTime baseStart = StarDate.Date;
            DateTime baseEnd = EndDate.Date;
            TimeSpan duration = baseEnd - baseStart;

            int cycleLength = RepeatAfter.Sum();

            if (cycleLength == 0)
                return date >= baseStart && date <= baseEnd;

            int daysSinceStart = (date - baseStart).Days;
            int fullCyclesPassed = Math.Max(0, daysSinceStart / cycleLength);

            DateTime cycleStart = baseStart.AddDays(fullCyclesPassed * cycleLength);

            // Now simulate inside this one cycle
            DateTime currentStart = cycleStart;
            DateTime currentEnd = currentStart + duration;

            for (int i = 0; i < RepeatAfter.Count; i++)
            {
                if (date >= currentStart && date <= currentEnd)
                    return true;

                int offset = RepeatAfter[i];
                currentStart = currentStart.AddDays(offset);
                currentEnd = currentStart + duration;
            }

            return false;
        }
    }
}
