using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calendar.ViewModels;

namespace Calendar.Models
{
    public class DayModel
    {
        public DateTime Date { get; set; }
        public List<DayEventViewModel> Events { get; set; } = new List<DayEventViewModel>();
    }
}
