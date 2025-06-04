using Calendar.Models;
using Calendar.Views;
using Calendar.ViewModels;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;

namespace Calendar;

public partial class EventCreation : ContentPage, INotifyPropertyChanged, IQueryAttributable
{
    public new event PropertyChangedEventHandler? PropertyChanged;

    public DayEvent? OriginalEvent { get; set; }

    public ObservableCollection<DayEvent> NewEvent { get; } = new()
    {
        new DayEvent(DateTime.Now, DateTime.Now, "")
    };

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
                RenderInput();
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
                RenderInput();
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
            NewEvent[0].Color = color;
            OnPropertyChanged(nameof(Color));
        }
    }

    public enum InputSelected { None, StartDate, EndDate, StartTime, EndTime, Color }
    public InputSelected SelectedInput { get; set; } = InputSelected.None;

    public List<int> Repeat { get; set; } = new();

    private readonly List<Color> colors = new()
    {
        (Color)Application.Current.Resources["Yellow"],
        (Color)Application.Current.Resources["Orange"],
        (Color)Application.Current.Resources["Pink"],
        (Color)Application.Current.Resources["Magenta"],
        (Color)Application.Current.Resources["Purple"],
        (Color)Application.Current.Resources["Blue"]
    };

    public EventCreation()
    {
        InitializeComponent();
        RenderInput();
    }

    private void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private async void OnSubmitButtonClicked(object sender, EventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(EventName.Text))
        {
            List<int> repeatPattern = new List<int>();
            var temp0 = EventRepeatAfter.Text;
            if (temp0 != null) {
                string[] temp = temp0.Split(',');
                foreach (string s in temp)
                {
                    if (int.TryParse(s.Trim(), out var number))
                    {
                        repeatPattern.Add(number);
                    }
                }
            }
            var newEvent = new DayEvent(StartDate, EndDate, EventName.Text)
            {
                Place = EventPlace.Text,
                Description = EventDescription.Text,
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
            EventNameBorder.Stroke = (Color)Application.Current.Resources["Pink"];
            EventNameBorder.StrokeThickness = 2;
        }
    }

    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///CalendarPage");
    }

    private void OnInputSelected(object sender, EventArgs e)
    {
        if (sender is Button btn &&
            Enum.TryParse<InputSelected>(btn.CommandParameter?.ToString(), out var input))
        {
            SelectedInput = SelectedInput == input ? InputSelected.None : input;
            RenderInput();
        }
    }

    public void RenderInput()
    {
        InputSection.Children.Clear();

        switch (SelectedInput)
        {
            case InputSelected.StartDate:
            case InputSelected.EndDate:
                var calendar = new CalendarView
                {
                    FontSize = 14,
                    GenerateEvents = true,
                    Events = NewEvent,
                    SelectedDay = SelectedInput == InputSelected.StartDate ? StartDate : EndDate
                };

                calendar.ViewModel.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(CalendarViewModel.SelectedDay))
                    {
                        if (SelectedInput == InputSelected.StartDate)
                            StartDate = calendar.SelectedDay;
                        else
                            EndDate = calendar.SelectedDay;
                    }
                };
                InputSection.Children.Add(calendar);
                break;

            case InputSelected.StartTime:
                var startTimeSelector = new TimeSelectionView(StartTime);
                startTimeSelector.SetBinding(TimeSelectionView.SelectedTimeProperty,
                    new Binding(nameof(StartTime), source: this, mode: BindingMode.TwoWay));
                InputSection.Children.Add(startTimeSelector);
                break;

            case InputSelected.EndTime:
                var endTimeSelector = new TimeSelectionView(EndTime);
                endTimeSelector.SetBinding(TimeSelectionView.SelectedTimeProperty,
                    new Binding(nameof(EndTime), source: this, mode: BindingMode.TwoWay));
                InputSection.Children.Add(endTimeSelector);
                break;

            case InputSelected.Color:
                var colorSelector = new ColorSelectionView(colors);
                colorSelector.SetBinding(ColorSelectionView.SelectedColorProperty,
                    new Binding(nameof(Color), source: this, mode: BindingMode.TwoWay));
                InputSection.Children.Add(colorSelector);
                break;
        }
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
            EventDescription.Text = loadedEvent.Description;
            EventName.Text = loadedEvent.Name;
            EventPlace.Text = loadedEvent.Place;
            string temp = string.Empty;
            foreach (int i in loadedEvent.RepeatAfter)
            {
                temp += i.ToString() + ", ";
            }
            if(temp.Length > 2)
            {
                temp = temp.Remove(temp.Length - 2);
            }
            EventRepeatAfter.Text = temp;
            IsModifying();
        }

        query.Clear();
    }

    public void IsModifying()
    {
        var button = new Button
        {
            Text = (string)Application.Current.Resources["TrashIcon"],
            Style = (Style)Application.Current.Resources["IconButton"]
        };
        button.Clicked += OnRemoveButtonClicked;

        Controls.Children.Add(Controls.Children[1]);
        Controls.Children[1] = button;
    }

    public async void OnRemoveButtonClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///CalendarPage", new Dictionary<string, object?>
        {
            { "Remove?", true },
            { "OriginalEvent", OriginalEvent }
        });
    }
}
