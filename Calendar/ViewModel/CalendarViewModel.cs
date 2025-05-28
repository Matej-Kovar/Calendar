using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Calendar.Model;

namespace Calendar.ViewModel
{
    public class CalendarViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<CalendarDay> Days { get; set; } = new ObservableCollection<CalendarDay>();
        public List<DayEvent> Events { get; set; } = new List<DayEvent>();
        public DateTime SelectedDay { get; set; } = DateTime.Now;

        public CalendarViewModel()
        {
            var Event = new DayEvent
            (
                new DateTime(2025, 5, 12),
                new DateTime(2025, 5, 16),
                "EVENT01"
            );
            Event.Color = Color.FromRgba("#dd0000");
            Events.Add( Event );
            var Event2 = new DayEvent
            (
                new DateTime(2025, 4, 22),
                new DateTime(2025, 5, 3),
                "EVENT01"
            );
            Event2.Color = Color.FromRgba("#a5be00");
            Events.Add(Event2);
            var Event3 = new DayEvent
            (
                new DateTime(2025, 5, 6),
                new DateTime(2025, 5, 7),
                "EVENT01"
            );
            Event3.Color = Color.FromRgba("#0036fa");
            Events.Add(Event3);
            var Event4 = new DayEvent
            (
                new DateTime(2025, 5, 25),
                new DateTime(2025, 5, 25),
                "EVENT01"
            );
            Event4.Color = Color.FromRgba("#ffb900");
            var Event5 = new DayEvent
            (
                new DateTime(2025, 5, 4),
                new DateTime(2025, 5, 9),
                "EVENT01"
            );
            Event5.Color = Color.FromRgba("#dd0000");
            Events.Add(Event5);
            var Event6 = new DayEvent
            (
                new DateTime(2025, 5, 7),
                new DateTime(2025, 5, 15),
                "EVENT01"
            );
            Event6.Color = Color.FromRgba("#a5be00");
            Events.Add(Event6);
            var Event7 = new DayEvent
            (
                new DateTime(2025, 5, 4),
                new DateTime(2025, 5, 8),
                "EVENT01"
            );
            Event7.Color = Color.FromRgba("#0036fa");
            Events.Add(Event7);
            var Event8 = new DayEvent
            (
                new DateTime(2025, 5, 24),
                new DateTime(2025, 5, 29),
                "EVENT01"
            );
            Event8.Color = Color.FromRgba("#ffb900");
            Events.Add(Event8);
            Events.Add(Event4);
            Events.Add(Event4);
        }
        public void LoadMonth(DateTime month)
        {
            Days.Clear();
            var firstOfMonth = new DateTime(month.Year, month.Month, 1);
            var lastOfMonth = new DateTime(month.Year, month.Month, DateTime.DaysInMonth(month.Year, month.Month));
            int offsetBefore = (int)firstOfMonth.DayOfWeek;
            int offsetAfter = 7 - (int)lastOfMonth.DayOfWeek;
            var startDate = firstOfMonth.AddDays(-offsetBefore);
            int total = DateTime.DaysInMonth(month.Year, month.Month) + offsetAfter + offsetBefore -1;

            for (int i = 0; i < total; i++)
            {
                var date = startDate.AddDays(i);
                Days.Add(new CalendarDay
                {
                    Events = Events.Where(e => e.isInRange(date)).ToList(),
                    Date = date,
                    IsCurrentMonth = date.Month == month.Month,
                    IsToday = date.Date == DateTime.Now.Date,
                    IsSelected = date.Date == SelectedDay.Date
                });
            }
        }

        public void SetSelectedDay(CalendarDay tappedDay)
        {
            SelectedDay = tappedDay.Date;
            foreach (var day in Days)
                day.IsSelected = false;
            tappedDay.IsSelected = true;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
