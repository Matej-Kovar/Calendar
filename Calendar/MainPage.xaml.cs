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

        DayEvent? _selectedEvent;

        public DayEvent? SelectedEvent
        {
            get { return _selectedEvent; }
            set { _selectedEvent = value; OnAddButtonClicked(null, null); }
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
            await Shell.Current.GoToAsync(nameof(EventCreation), new Dictionary<string, object?>
            {
                { "SelectedDate", SelectedDay },
                { "SelectedEvent", SelectedEvent }
            });
        }

        public void ApplyQueryAttributes(IDictionary<string, object?> query)
        {
            SelectedEvent = null;
            if (query.TryGetValue("NewEvent", out var newObj) && newObj is DayEvent dayEvent)
            {
                bool replaced = false;

                if (query.TryGetValue("OriginalEvent", out var origObj) && origObj is DayEvent originalEvent)
                {
                    int index = Events.IndexOf(originalEvent);
                    if (index >= 0)
                    {
                        Events[index] = dayEvent;
                        replaced = true;
                    }
                }

                if (!replaced)
                {
                    Events.Add(dayEvent);
                }

                UpdateCalendar();
                query.Clear();
            }
        }

        public void UpdateCalendar()
        {
            Calendar.Events = Events;
            Calendar.RenderCalendar();
            UpdateEventList();
        }

        public void UpdateEventList()
        {
            EventDetails.Children.Clear();
            if (Calendar is not null)
            {
                var selectedDay = Calendar.Days.FirstOrDefault(d => d.Date == SelectedDay.Date);
                if (selectedDay != null)
                {
                    if (selectedDay.Events.Count == 0)
                    {
                        EventDetails.Children.Add(new Label
                        {
                            Text = "Na tento den není naplánovaná událost",
                            Opacity = .5,
                            //VerticalOptions = LayoutOptions.Center,
                            HorizontalOptions = LayoutOptions.Center,
                           
                        });
                    }
                    else
                    {
                        foreach (DayEvent dayEvent in selectedDay.Events)
                        {
                            var detail = new EventDetailView(dayEvent);
                            detail.GestureRecognizers.Add(new TapGestureRecognizer
                            {
                                Command = new Command(() => OnEventSelected(dayEvent))
                            });
                            EventDetails.Children.Add(detail);
                        }
                    }
                }
            }
        }

        public void OnEventSelected(DayEvent selectedEvent)
        {
            SelectedEvent = selectedEvent;
        }
    }
}
