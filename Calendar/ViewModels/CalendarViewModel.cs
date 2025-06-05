using Calendar.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Calendar.ViewModels
{
    public class CalendarViewModel : INotifyPropertyChanged
    {
        private DateTime controlDate = DateTime.Now;
        private DateTime selectedDay = DateTime.Now;

        public CalendarViewModel()
        {
            NextMonthCommand = new Command(() => ControlDate = ControlDate.AddMonths(1));
            PreviousMonthCommand = new Command(() => ControlDate = ControlDate.AddMonths(-1));
            LoadMonth();
        }

        #region public methods
        public void LoadMonth()
        {
            var month = ControlDate;
            Days.Clear();
            var firstOfMonth = new DateTime(month.Year, month.Month, 1);
            var lastOfMonth = new DateTime(month.Year, month.Month, DateTime.DaysInMonth(month.Year, month.Month));
            int offsetBefore = (int)firstOfMonth.DayOfWeek;
            int offsetAfter = 7 - (int)lastOfMonth.DayOfWeek;
            var startDate = firstOfMonth.AddDays(-offsetBefore);
            int total = DateTime.DaysInMonth(month.Year, month.Month) + offsetAfter + offsetBefore - 1;

            for (int i = 0; i < total; i++)
            {
                var date = startDate.AddDays(i);
                var dayModel = new DayModel
                {
                    Date = date,
                    Events = Events.Select(e => e.GetEvent(date)).Where(e => e is not null).ToList()
                };

                Days.Add(new DayViewModel(dayModel)
                {
                    IsCurrentMonth = date.Month == month.Month,
                    IsToday = date.Date == DateTime.Now.Date,
                    IsSelected = date.Date == SelectedDay.Date
                });
            }
        }

        public void SetSelectedDay(DayViewModel tappedDay)
        {
            foreach (var day in Days)
                day.IsSelected = false;

            tappedDay.IsSelected = true;

            SelectedDay = tappedDay.Date;
        }
        #endregion

        private void OnPropertyChanged([CallerMemberName] string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public ICommand NextMonthCommand { get; }
        public ICommand PreviousMonthCommand { get; }

        #region public properties
        public DateTime ControlDate
        {
            get => controlDate;
            set
            {
                if (controlDate.Month != value.Month)
                {
                    controlDate = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Header));
                    LoadMonth();
                }
            }
        }

        public DateTime SelectedDay
        {
            get => selectedDay;
            set
            {
                if (selectedDay != value)
                {
                    selectedDay = value;
                    OnPropertyChanged();
                    ControlDate = selectedDay;
                }
            }
        }

        public string Header => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(ControlDate.ToString("MMMM yyyy"));

        public ObservableCollection<DayViewModel> Days { get; set; } = new();

        public ObservableCollection<DayEventViewModel> Events { get; set; } = new();

        public string[] DayNames { get; } = DateTimeFormatInfo.CurrentInfo.DayNames.Select(n => n.Substring(0, 3).ToUpper()).ToArray();
        
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}