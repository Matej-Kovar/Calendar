using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calendar.Models
{
    public class EventTag
    {
        public EventTag(Color color, string name)
        {
            Color = color;
            Name = name;
        }

        public Color Color { get; set; }
        public string Name { get; set; }
    }
}
