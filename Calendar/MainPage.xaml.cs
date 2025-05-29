using Calendar.Model;
using Calendar.ViewModel;
using Microsoft.Maui.Controls.Handlers.Items;
using Microsoft.Maui.Controls.Shapes;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Calendar
{
    public partial class MainPage : ContentPage, IQueryAttributable
    {
        CalendarView Calendar;

        DateTime _selectdDay = DateTime.Now;
        public DateTime SelectedDay
        {
            get {  return _selectdDay; }
            set { _selectdDay = value; UpdateEventList(); }
        }

        public ObservableCollection<DayEvent> Events = new ObservableCollection<DayEvent>();

        public MainPage()
        {
            InitializeComponent();
            Calendar = new CalendarView
            {
                Events = Events,
                FontSize = 16,
                GenerateEvents = true,
            };
            Calendar.SetBinding(
                CalendarView.SelectedDayProperty,
                new Binding(nameof(SelectedDay), source: this, mode: BindingMode.TwoWay)
            );
            CalendarHolder.Children.Add(Calendar);
            Calendar.RenderCalendar();
            UpdateEventList();
        }

        private async void OnAddButtonClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(EventCreation));
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.ContainsKey("NewEvent"))
            {
                DayEvent dayEvent = (DayEvent)query["NewEvent"];
                Events.Add(dayEvent);
                UpdateCalendar();
                query.Clear();
            }
        }

        public void UpdateCalendar()
        {
            Calendar.Events = Events;
            Calendar.RenderCalendar();
        }

        public void UpdateEventList()
        {
            EventDetails.Children.Clear();
            if (Calendar is not null)
            {
                foreach (DayEvent dayEvent in Calendar.Days.Where(d => d.Date == SelectedDay.Date).FirstOrDefault().Events)
                {
                    var detail = new EventDetailView(dayEvent);
                    EventDetails.Children.Add(detail);
                }
            }
        }
    }
}
