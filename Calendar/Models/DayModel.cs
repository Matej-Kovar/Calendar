using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calendar.Models
{
    public class DayModel
    {
        public DateTime Date { get; set; }
        public List<DayEvent> Events { get; set; } = new List<DayEvent>();
    }
}
