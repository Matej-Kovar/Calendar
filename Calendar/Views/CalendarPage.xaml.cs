using Calendar.Models;
using Calendar.ViewModels;
using Microsoft.Maui.Controls.Handlers.Items;
using Microsoft.Maui.Controls.Shapes;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Windows.Input;

namespace Calendar
{

    public partial class CalendarPage : ContentPage, IQueryAttributable
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

        public CalendarPage()
        {
            InitializeComponent();

            var temp = LoadEvents(path);
            foreach (DayEvent e in temp)
                Events.Add(e);

            Calendar = new CalendarView
            {
                FontSize = 16,
                GenerateEvents = true,
                Events = Events,
                SelectedDay = SelectedDay
            };

            CalendarHolder.Children.Add(Calendar);

            Calendar.ViewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(Calendar.ViewModel.SelectedDay))
                {
                    SelectedDay = Calendar.SelectedDay;
                }
            };

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
            }
            else if (query.TryGetValue("Remove?", out var temp) && temp is bool remove)
            {
                if (remove && query.TryGetValue("OriginalEvent", out var origObj) && origObj is DayEvent originalEvent)
                {
                    Events.Remove(originalEvent);
                    UpdateCalendar();
                }
            }
                query.Clear();
        }

        public void UpdateCalendar()
        {
            SaveEvents(path);
            Calendar.Events = Events;
            Calendar.RenderCalendar();
            UpdateEventList();
        }

        public void UpdateEventList()
        {
            EventDetails.Children.Clear();

            var selectedDay = Calendar.ViewModel.Days.FirstOrDefault(d => d.Date == SelectedDay.Date);
            if (selectedDay is null)
                return;

            if (selectedDay.Events.Count == 0)
            {
                EventDetails.Children.Add(new Label
                {
                    Text = "Na tento den není naplánovaná událost",
                    Opacity = .5,
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

        public void OnEventSelected(DayEvent selectedEvent)
        {
            SelectedEvent = selectedEvent;
        }

        public List<DayEvent> LoadEvents(string path)
        {
            List<DayEvent> loadedEvents = new List<DayEvent>();
            if(/*File.Exists(path)*/false)
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
