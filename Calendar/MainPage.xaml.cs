using Calendar.Model;
using Calendar.ViewModel;
using Microsoft.Maui.Controls.Handlers.Items;
using Microsoft.Maui.Controls.Shapes;
using System.Windows.Input;

namespace Calendar
{
    public partial class MainPage : ContentPage
    {
        private CalendarViewModel ViewModel;
        CalendarView Calendar;

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
                    ViewModel.LoadMonth(ControlDate);
                    Calendar.Days = ViewModel.Days;
                    Calendar.RenderCalendar();
                }
            }
        }
        public MainPage()
        {
            //swipe nefunguje
            double panX = 0;
            var panGesture = new PanGestureRecognizer();
            panGesture.PanUpdated += (s, e) =>
            {
                switch (e.StatusType)
                {
                    case GestureStatus.Started:
                        panX = 0;
                        break;
                    case GestureStatus.Running:
                        panX += e.TotalX;
                        break;
                    case GestureStatus.Completed:
                        if (panX > 100)
                            PreviousMonth.Execute(null);
                        else if (panX < -100)
                            NextMonth.Execute(null);
                        break;
                }
            };

            InitializeComponent();
            ViewModel = new CalendarViewModel();
            BindingContext = ViewModel;
            ViewModel.LoadMonth(ControlDate);
            CalendarHolder.GestureRecognizers.Add(panGesture);
            Calendar = new CalendarView
            {
                FontSize = 16,
                GenerateEvents = true,
                OnDayTapped = OnDayTapped,
                Days = ViewModel.Days,
            };
            CalendarHolder.Children.Add(Calendar);
        }

        void OnDayTapped(CalendarDay tappedDay)
        {
            ViewModel.SetSelectedDay(tappedDay);
        }
        public ICommand NextMonth => new Command(() => ControlDate = ControlDate.AddMonths(1));
        public ICommand PreviousMonth => new Command(() => ControlDate = ControlDate.AddMonths(-1));

        private async void OnAddButtonClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(EventCreation));
        }
    }
}
