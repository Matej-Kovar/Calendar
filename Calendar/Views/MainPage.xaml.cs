using Calendar.Model;
using Calendar.ViewModel;
using Microsoft.Maui.Controls.Handlers.Items;
using Microsoft.Maui.Controls.Shapes;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Windows.Input;

namespace Calendar
{
    public partial class MainPage : ContentPage, IQueryAttributable
    {
        CalendarView Calendar;

        string path = System.IO.Path.Combine(FileSystem.AppDataDirectory, "events.json");

        DateTime selectdDay = DateTime.Now;
        public DateTime SelectedDay
        {
            get {  return selectdDay; }
            set { selectdDay = value; UpdateEventList(); }
        }

        DayEvent? selectedEvent;

        public DayEvent? SelectedEvent
        {
            get { return selectedEvent; }
            set { selectedEvent = value; OnAddButtonClicked(new object(), new EventArgs()); }
        }

        public ObservableCollection<DayEvent> Events = new ObservableCollection<DayEvent>();

        public MainPage()
        {
            InitializeComponent();
            /*Events.Add(new DayEvent {
                Name = "Repeating Event",
                Description = "This event should repeat every 3,1 days",
                StarDate = DateTime.Now,
                EndDate = DateTime.Now.AddMinutes(8),
                RepeatAfter = [7],
                Color = (Color)Application.Current.Resources["Yellow"]

            });*/
            var temp = LoadEvents(path);
            foreach (DayEvent e in temp)
            {
                Events.Add(e);
            }
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
                        //modify saved event
                    }
                }

                if (!replaced)
                {
                    Events.Add(dayEvent);
                    //add event to saved events
                }

                UpdateCalendar();
            }
            else if (query.TryGetValue("Remove?", out var temp) && temp is bool remove)
            {
                if (remove && query.TryGetValue("OriginalEvent", out var origObj) && origObj is DayEvent originalEvent)
                {
                    Events.Remove(originalEvent);
                    //remove event from saved events
                    UpdateCalendar();
                }
            }
                query.Clear();
        }

        public void UpdateCalendar()
        {
            Calendar.Events = Events;
            SaveEvents(path);
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

        public List<DayEvent> LoadEvents(string path)
        {
            List<DayEvent> loadedEvents = new List<DayEvent>();
            if(File.Exists(path))
            {
                string json = File.ReadAllText(path);
                var temp = JsonSerializer.Deserialize<List<DayEvent>>(json);;
                if(temp != null)
                {
                    loadedEvents = temp;
                }
            }
            return loadedEvents;
        }

        public void SaveEvents(string path)
        {
            var json = JsonSerializer.Serialize(Events.ToList()/*, new JsonSerializerOptions { WriteIndented = true }*/);
            File.WriteAllTextAsync(path, json);
        }
    }
}
