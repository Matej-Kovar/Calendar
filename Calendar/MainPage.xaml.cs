using Calendar.Model;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;

namespace Calendar
{
    public partial class MainPage : ContentPage
    {
        public BindableProperty ControlDatePropety = BindableProperty.Create(
            nameof(ControlDate),
            typeof(DateTime),
            declaringType: typeof(MainPage),
            defaultValue: DateTime.Now,
            defaultBindingMode: BindingMode.TwoWay
        );

        public DateTime ControlDate { 
            get => (DateTime)GetValue( ControlDatePropety );
            set => SetValue( ControlDatePropety, value );
        }

        public DateTime SelectedDay { get; set; } = DateTime.Now;

        public ObservableCollection<CalendarDay> Days { get; set; } = new ObservableCollection<CalendarDay>();

        public string[] DayNames { get; set; } = DateTimeFormatInfo.CurrentInfo.DayNames.Select(n => (string)n.Substring(0, 3).ToUpper()).ToArray();
        public MainPage()
        {
            InitializeComponent();
            GetDays(DateTime.Now);
            BindingContext = this;
        }

        public void GetDays(DateTime startDay)
        {
            Days.Clear();
            var firstOfMonth = new DateTime(startDay.Year, startDay.Month, 1);


            int diffToSunday = (int)firstOfMonth.DayOfWeek;
            var startDate = firstOfMonth.AddDays(-diffToSunday);


            int daysInMonth = DateTime.DaysInMonth(startDay.Year, startDay.Month);
            var lastOfMonth = new DateTime(startDay.Year, startDay.Month, daysInMonth);

            int daysToAdd = (int)(lastOfMonth - startDate).TotalDays + 1;
            int extraDays = 7 - (daysToAdd % 7);
            int totalDays = daysToAdd + (extraDays == 7 ? 0 : extraDays);
            int daysCount = DateTime.DaysInMonth(startDay.Year, startDay.Month);
            for (int i = 0; i < totalDays; i++)
            {
                var date = startDate.AddDays(i);
                Days.Add(new CalendarDay
                {
                    Date = date,
                    IsCurrentMonth = date.Month == startDay.Month
                });
            }

            CalendarDay? selectedDay = Days.Where(x => x.Date == SelectedDay.Date).FirstOrDefault();
            CalendarDay? today = Days.Where(x => x.Date == DateTime.Now.Date).FirstOrDefault();
            if (selectedDay != null)
            {
                selectedDay.IsSelected = true;
            }
            if (today != null)
            {
                today.IsToday = true;
            }
        }

        public ICommand SetSelectedDay => new Command<CalendarDay>((newDay) =>
        {
            SelectedDay = newDay.Date;
            foreach (CalendarDay day in Days)
            {
                day.IsSelected = false;
            }
            newDay.IsSelected = true;
        });

        public ICommand NextMonth => new Command(() => 
        { 
            ControlDate = ControlDate.AddMonths(1);
            GetDays(ControlDate);
        });

        public ICommand PreviousMonth => new Command(() =>
        {
            ControlDate = ControlDate.AddMonths(-1);
            GetDays(ControlDate);
        });
    }
}
