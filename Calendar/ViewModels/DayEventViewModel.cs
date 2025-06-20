﻿using Calendar.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calendar.ViewModels
{
    public class DayEventViewModel: INotifyPropertyChanged
    {
        public DayEventViewModel(DayEvent model)
        {
            this.Model = model;
        }
        public DayEventViewModel? GetEvent(DateTime date)
        {
            date = date.Date;
            if (RepeatAfter == null || RepeatAfter.Count == 0 || RepeatAfter.All(x => x == 0))
            {
                if(date >= StarDate.Date && date <= EndDate.Date)
                {
                    return new DayEventViewModel(Model);
                }
                else
                {
                    return null;
                }
            }

            DateTime baseStart = StarDate.Date;
            DateTime baseEnd = EndDate.Date;
            TimeSpan duration = baseEnd - baseStart;

            int cycleLength = RepeatAfter.Sum() + (EndDate - StarDate).Days;

            if (cycleLength == 0)
            {
                if (date >= StarDate.Date && date <= EndDate.Date)
                {
                    return new DayEventViewModel(Model);
                }
                else
                {
                    return null;
                }
            }

            int daysSinceStart = (date - baseStart).Days;
            int fullCyclesPassed = Math.Max(0, daysSinceStart / cycleLength);

            DateTime cycleStart = baseStart.AddDays(fullCyclesPassed * cycleLength);

            DateTime currentStart = cycleStart;
            DateTime currentEnd = currentStart + duration;

            for (int i = 0; i < RepeatAfter.Count; i++)
            {
                if (date >= currentStart && date <= currentEnd)
                {
                    DayEvent newDayEvent = new DayEvent
                    {
                        EndDate = currentEnd.Date + Model.EndDate.TimeOfDay,
                        StarDate = currentStart.Date + Model.StarDate.TimeOfDay,
                        ColorHex = Model.ColorHex,
                        Name = Model.Name,
                        Description = Model.Description,
                        Place = Model.Place,
                        RepeatAfter = Model.RepeatAfter,
                        Id = Id
                    };
                    return new DayEventViewModel(newDayEvent);
                }
                int offset = RepeatAfter[i];
                currentStart = currentEnd.AddDays(offset);
                currentEnd = currentStart + duration;
            }

            return null;
        }
        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        #region public properties
        public DayEvent Model { get; set; }

        public Guid Id
        {
            get => Model.Id;
        }

        public Color Color
        {
            get => Color.FromArgb(Model.ColorHex);
            set 
            {
                Model.ColorHex = value.ToArgbHex();
                OnPropertyChanged(nameof(Color));
            } 
        }
        public DateTime StarDate 
        { 
            get => Model.StarDate;
            set
            {
                Model.StarDate = value;
                OnPropertyChanged(nameof(StarDate));
            }
        }
        public DateTime EndDate 
        { 
            get => Model.EndDate;
            set
            {
                Model.EndDate = value;
                OnPropertyChanged(nameof(EndDate));
            }
        }
        public string Name 
        { 
            get => Model.Name;
            set
            {
                Model.Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        public string? Description 
        { 
            get => Model.Description;
            set
            {
                Model.Description = value;
                OnPropertyChanged(nameof(Description));
            }
        }
        public string? Place 
        { 
            get => Model.Place;
            set
            {
                Model.Place = value;
                OnPropertyChanged(nameof(Place));
            }
        }
        public List<int> RepeatAfter 
        { 
            get => Model.RepeatAfter;
            set 
            {
                Model.RepeatAfter = value;
                OnPropertyChanged(nameof(RepeatAfter));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        #endregion
    }
}
