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
        public string[] DayNames { get; set; }
        public DateTime SelectedDay { get; set; } = DateTime.Now;

        private DateTime _controlDate = DateTime.Now;
        public DateTime ControlDate
        {
            get => _controlDate;
            set
            {
                if (_controlDate != value)
                {
                    _controlDate = value;
                    OnPropertyChanged(nameof(ControlDate));
                    LoadMonth(_controlDate);
                }
            }
        }

        public ICommand NextMonth => new Command(() => ControlDate = ControlDate.AddMonths(1));
        public ICommand PreviousMonth => new Command(() => ControlDate = ControlDate.AddMonths(-1));

        public CalendarViewModel()
        {
            DayNames = DateTimeFormatInfo.CurrentInfo.DayNames.Select(n => n.Substring(0, 3).ToUpper()).ToArray();
            LoadMonth(ControlDate);
        }

        void LoadMonth(DateTime month)
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

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
