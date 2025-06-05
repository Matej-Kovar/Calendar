using Calendar.Models;
using Calendar.Views;
using Microsoft.Maui.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using static Calendar.EventCreation;

namespace Calendar.ViewModels
{
    public class EventCreationViewModel : INotifyPropertyChanged
    {
        #region private variables
        private DateTime startDate = DateTime.Now;
        private DateTime endDate = DateTime.Now;
        private bool isDataValid = true;
        private bool isModifying = false;
        private string eventName = string.Empty;
        private string eventDescription = string.Empty;
        private string eventPlace = string.Empty;
        private string eventRepeatAfter = string.Empty;
        private Color color = (Color)Application.Current.Resources["Blue"];
        #endregion

        #region public methods
        public async void CreateNewEvent()
        {
            if (!string.IsNullOrWhiteSpace(EventName))
            {
                IsDataValid = true;
                List<int> repeatPattern = new List<int>();
                var temp0 = EventRepeatAfter;
                if (temp0 != null)
                {
                    string[] temp = temp0.Split(',');
                    foreach (string s in temp)
                    {
                        if (int.TryParse(s.Trim(), out var number))
                        {
                            repeatPattern.Add(number);
                        }
                    }
                }
                var newEvent = new DayEventViewModel(new DayEvent(StartDate, EndDate, EventName))
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
            else
            {
                IsDataValid = false;
            }
        }
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            IsModifying = false;
            if (query.TryGetValue("SelectedDate", out var selectedDate))
            {
                StartDate = (DateTime)selectedDate;
                EndDate = StartDate;
            }

            if (query.TryGetValue("SelectedEvent", out var selectedEvent) && selectedEvent is DayEventViewModel loadedEvent)
            {
                OriginalEvent = loadedEvent;
                Color = loadedEvent.Color;
                StartDate = loadedEvent.StarDate;
                EndDate = loadedEvent.EndDate;
                StartTime = loadedEvent.StarDate;
                EndTime = loadedEvent.EndDate;
                NewEvent = loadedEvent;
                EventDescription = loadedEvent.Description;
                EventName = loadedEvent.Name;
                EventPlace = loadedEvent.Place;
                string temp = string.Empty;
                foreach (int i in loadedEvent.RepeatAfter)
                {
                    temp += i.ToString() + ", ";
                }
                if (temp.Length > 2)
                {
                    temp = temp.Remove(temp.Length - 2);
                }
                EventRepeatAfter = temp;
                IsModifying = true;
            }
            query.Clear();
        }
        public async void NavigateBack()
        {
            await Shell.Current.GoToAsync("///CalendarPage");
        }
        public async void RemoveEvent()
        {
            await Shell.Current.GoToAsync("///CalendarPage", new Dictionary<string, object?>
        {
            { "Remove?", true },
            { "OriginalEvent", OriginalEvent }
        });
        }
        #endregion
        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        #region public properties
        public event PropertyChangedEventHandler? PropertyChanged;
        public DayEventViewModel? OriginalEvent { get; set; }
        public DayEventViewModel NewEvent { get; set; } = new DayEventViewModel(new DayEvent(DateTime.Now, DateTime.Now, ""));
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
                    NewEvent.StarDate = StartDate;
                }
            }
        }
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
                    NewEvent.EndDate = EndDate;
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
                    NewEvent.StarDate = StartDate;
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
                    NewEvent.EndDate = EndDate;
                }
            }
        }
        public bool IsDataValid
        {
            get => isDataValid;
            set
            {
                if (isDataValid != value)
                {
                    isDataValid = value;
                    OnPropertyChanged(nameof(IsDataValid));
                }
            }
        }
        public bool IsModifying
        {
            get => isModifying;
            set
            {
                isModifying = value;
                OnPropertyChanged(nameof(IsModifying));

            }
        }
        public string EventName
        {
            get => eventName;
            set
            {
                if (eventName != value)
                {
                    eventName = value;
                    OnPropertyChanged(nameof(EventName));
                }
            }
        }
        public string EventDescription
        {
            get => eventDescription;
            set
            {
                if (eventDescription != value)
                {
                    eventDescription = value;
                    OnPropertyChanged(nameof(EventDescription));
                }
            }
        }
        public string EventPlace
        {
            get => eventPlace;
            set
            {
                if (eventPlace != value)
                {
                    eventPlace = value;
                    OnPropertyChanged(nameof(EventPlace));
                }
            }
        }
        public string EventRepeatAfter
        {
            get => eventRepeatAfter;
            set
            {
                if (eventRepeatAfter != value)
                {
                    eventRepeatAfter = value;
                    OnPropertyChanged(nameof(EventRepeatAfter));
                }
            }
        }
        public Color Color
        {
            get => color;
            set
            {
                color = value;
                NewEvent.Color = color;
                OnPropertyChanged(nameof(Color));
            }
        }
        public List<int> Repeat { get; set; } = new();

        public readonly List<Color> Colors = new()
    {
        (Color)Application.Current.Resources["Yellow"],
        (Color)Application.Current.Resources["Orange"],
        (Color)Application.Current.Resources["Pink"],
        (Color)Application.Current.Resources["Magenta"],
        (Color)Application.Current.Resources["Purple"],
        (Color)Application.Current.Resources["Blue"]
    };
        #endregion
    }
}