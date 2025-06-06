using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Calendar.Models;

namespace Calendar.ViewModels
{
    public class CalendarPageViewModel: INotifyPropertyChanged
    {
        string path = System.IO.Path.Combine(FileSystem.AppDataDirectory, "events.json");
        DateTime selectdDay = DateTime.Now;
        public CalendarPageViewModel()
        {
            var temp = LoadEvents(path);
            foreach (DayEventViewModel e in temp)
                Events.Add(e);

            Calendar = new CalendarView
            {
                FontSize = 16,
                GenerateEvents = true,
                Events = Events,
                SelectedDay = SelectedDay
            };

            Calendar.ViewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(Calendar.ViewModel.SelectedDay))
                {
                    SelectedDay = Calendar.SelectedDay;
                }
            };
        }

        #region public methods
        public async void NewEvent(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(EventCreation), new Dictionary<string, object?>
            {
                { "SelectedDate", SelectedDay },
                { "SelectedEvent", SelectedEvent }
            });
        }
        public void ApplyQueryAttributes(IDictionary<string, object?> query)
        {
            if (query.TryGetValue("NewEvent", out var newObj) && newObj is DayEventViewModel dayEvent)
            {
                bool replaced = false;

                if (query.TryGetValue("OriginalEvent", out var origObj) && origObj is DayEventViewModel originalEvent)
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
                if (remove && query.TryGetValue("OriginalEvent", out var origObj) && origObj is DayEventViewModel originalEvent)
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
        }
        public List<DayEventViewModel> LoadEvents(string path)
        {
            List<DayEventViewModel> loadedEvents = new List<DayEventViewModel>();
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                var temp = JsonSerializer.Deserialize<List<DayEvent>>(json); ;
                if (temp != null)
                {
                    foreach (var dayEvent in temp)
                    {
                        loadedEvents.Add(new DayEventViewModel(dayEvent));
                    }
                }
            }
            return loadedEvents;
        }
        public void SaveEvents(string path)
        {
            var json = JsonSerializer.Serialize(Events.Select(e => e.Model).ToList()/*, new JsonSerializerOptions { WriteIndented = true }*/);
            File.WriteAllTextAsync(path, json);
        }
        #endregion

        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        #region public properties
        public CalendarView Calendar { get; set; }
        public DateTime SelectedDay
        {
            get { return selectdDay; }
            set { selectdDay = value; OnPropertyChanged(nameof(SelectedDay)); }
        }
        public DayEventViewModel? selectedEvent { get; set; }
        public DayEventViewModel? SelectedEvent
        {
            get { return selectedEvent; }
            set { selectedEvent = Events.First(e => e.Id == value!.Id); NewEvent(new object(), new EventArgs()); }
        }
        public ObservableCollection<DayEventViewModel> Events { get; set; } = new ObservableCollection<DayEventViewModel>();
        public event PropertyChangedEventHandler? PropertyChanged;
        #endregion
    }

}
