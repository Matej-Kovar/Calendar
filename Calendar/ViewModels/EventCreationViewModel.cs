using Calendar.Models;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

    namespace Calendar.ViewModels
    {
        public class EventCreationViewModel : INotifyPropertyChanged, IQueryAttributable
        {
            public event PropertyChangedEventHandler? PropertyChanged;
            public ObservableCollection<DayEvent> NewEvent { get; } = new() { new DayEvent(DateTime.Now, DateTime.Now, "") };
            public DayEvent? OriginalEvent { get; set; }

            private DateTime startDate = DateTime.Now;
            public DateTime StartDate
            {
                get => startDate;
                set
                {
                    if (startDate.Date != value.Date)
                    {
                        if (value.Date > EndDate.Date)
                            EndDate = value;

                        startDate = value.Date + startDate.TimeOfDay;
                        OnPropertyChanged(nameof(StartDate));
                        NewEvent[0].StarDate = StartDate;
                    }
                }
            }

            private DateTime endDate = DateTime.Now;
            public DateTime EndDate
            {
                get => endDate;
                set
                {
                    if (endDate.Date != value.Date)
                    {
                        if (value.Date < StartDate.Date)
                            StartDate = value;

                        endDate = value.Date + endDate.TimeOfDay;
                        OnPropertyChanged(nameof(EndDate));
                        NewEvent[0].EndDate = EndDate;
                    }
                }
            }

            public DateTime StartTime
            {
                get => startDate;
                set
                {
                    if (startDate.TimeOfDay != value.TimeOfDay)
                    {
                        if ((startDate.Date + value.TimeOfDay) > EndDate)
                            EndTime = value;

                        startDate = startDate.Date + value.TimeOfDay;
                        OnPropertyChanged(nameof(StartTime));
                        NewEvent[0].StarDate = StartDate;
                    }
                }
            }

            public DateTime EndTime
            {
                get => endDate;
                set
                {
                    if (endDate.TimeOfDay != value.TimeOfDay)
                    {
                        if ((endDate.Date + value.TimeOfDay) < StartDate)
                            StartTime = value;

                        endDate = endDate.Date + value.TimeOfDay;
                        OnPropertyChanged(nameof(EndTime));
                        NewEvent[0].EndDate = EndDate;
                    }
                }
            }

        private Color color = (Color)Application.Current.Resources["Blue"];
            public Color Color
            {
                get => color;
                set
                {
                    color = value;
                    NewEvent[0].Color = value;
                    OnPropertyChanged(nameof(Color));
                }
            }

            public enum InputSelected { None, StartDate, EndDate, StartTime, EndTime, Color }
        private InputSelected selectedInput = InputSelected.None;
        public InputSelected SelectedInput
        {
            get => selectedInput;
            set
            {
                if (selectedInput != value)
                {
                    selectedInput = value;
                    OnPropertyChanged(nameof(SelectedInput));
                }
            }
        }

        public List<int> Repeat { get; set; } = new();

            public List<Color> Colors { get; } = new()
        {
            (Color)Application.Current.Resources["Yellow"],
            (Color)Application.Current.Resources["Orange"],
            (Color)Application.Current.Resources["Pink"],
            (Color)Application.Current.Resources["Magenta"],
            (Color)Application.Current.Resources["Purple"],
            (Color)Application.Current.Resources["Blue"]
        };
            public ICommand SubmitCommand { get; }
            public ICommand RemoveCommand { get; }
            public ICommand SelectInputCommand { get; }

            public string EventName { get; set; } = string.Empty;
            public string EventPlace { get; set; } = string.Empty;
            public string EventDescription { get; set; } = string.Empty;
            public string EventRepeatText { get; set; } = string.Empty;

            public EventCreationViewModel()
            {
                SubmitCommand = new Command(OnSubmit);
                RemoveCommand = new Command(OnRemove);
                SelectInputCommand = new Command<InputSelected>(input => SelectedInput = SelectedInput == input ? InputSelected.None : input);
            }

            private async void OnSubmit()
            {
                if (!string.IsNullOrWhiteSpace(EventName))
                {
                    List<int> repeatPattern = new();
                    if (EventRepeatText != null)
                    {
                        string[] temp = EventRepeatText.Split(',');
                        foreach (string s in temp)
                        {
                            if (int.TryParse(s.Trim(), out var number))
                                repeatPattern.Add(number);
                        }
                    }

                    var newEvent = new DayEvent(StartDate, EndDate, EventName)
                    {
                        Place = EventPlace,
                        Description = EventDescription,
                        Color = Color,
                        RepeatAfter = repeatPattern
                    };

                    await Shell.Current.GoToAsync("///CalendarPage", new Dictionary<string, object?>
                {
                    { "NewEvent", newEvent },
                    { "OriginalEvent", OriginalEvent }
                });
                }
            }

            private async void OnRemove()
            {
                await Shell.Current.GoToAsync("///CalendarPage", new Dictionary<string, object?>
            {
                { "Remove?", true },
                { "OriginalEvent", OriginalEvent }
            });
            }

            public void ApplyQueryAttributes(IDictionary<string, object> query)
            {
                if (query.TryGetValue("SelectedDate", out var selectedDate))
                {
                    StartDate = (DateTime)selectedDate;
                    EndDate = StartDate;
                }

                if (query.TryGetValue("SelectedEvent", out var selectedEvent) && selectedEvent is DayEvent loadedEvent)
                {
                    OriginalEvent = loadedEvent;
                    Color = loadedEvent.Color;
                    StartDate = loadedEvent.StarDate;
                    EndDate = loadedEvent.EndDate;
                    StartTime = loadedEvent.StarDate;
                    EndTime = loadedEvent.EndDate;
                    NewEvent[0] = loadedEvent;
                    EventDescription = loadedEvent.Description;
                    EventName = loadedEvent.Name;
                    EventPlace = loadedEvent.Place;
                    EventRepeatText = string.Join(", ", loadedEvent.RepeatAfter);
                }

                query.Clear();
            }

            private void OnPropertyChanged(string propertyName) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
