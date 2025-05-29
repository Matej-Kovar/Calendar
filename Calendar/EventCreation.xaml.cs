using Calendar.Model;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;

namespace Calendar;

public partial class EventCreation : ContentPage, INotifyPropertyChanged
{
    public new event PropertyChangedEventHandler? PropertyChanged;

    public ObservableCollection<DayEvent> NewEvent { get; set; }

    private DateTime _startDate = DateTime.Now;
    public DateTime StartDate
    {
        get => _startDate;
        set
        {
            if (_startDate.Date != value.Date)
            {
                if (value.Date > EndDate.Date)
                {
                    EndDate = value;
                }
                _startDate = value.Date + _startDate.TimeOfDay;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StartDate)));
                NewEvent[0].StarDate = StartDate;

            }
        }
    }

    private DateTime _endDate = DateTime.Now;
    public DateTime EndDate
    {
        get => _endDate;
        set
        {
            if (_endDate.Date != value.Date)
            {
                if (value.Date < StartDate.Date)
                {
                    StartDate = value;
                }
                _endDate = value.Date + _endDate.TimeOfDay;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EndDate)));
                NewEvent[0].EndDate = EndDate;
            }
        }
    }
    public DateTime StartTime
    {
        get => _startDate;
        set
        {
            if (_startDate.TimeOfDay != value.TimeOfDay)
            {
                if ((_startDate.Date + value.TimeOfDay) > EndDate)
                {
                    EndTime = value;
                }
                _startDate = _startDate.Date + value.TimeOfDay;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StartTime)));
                NewEvent[0].StarDate = StartDate;

            }
        }
    }
    public DateTime EndTime
    {
        get => _endDate;
        set
        {
            if (_endDate.TimeOfDay != value.TimeOfDay)
            {
                if ((_endDate.Date + value.TimeOfDay) < StartDate)
                {
                    StartTime = value;
                }
                _endDate = _endDate.Date + value.TimeOfDay;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EndTime)));
                NewEvent[0].EndDate = EndDate;

            }
        }
    }
    List<Color> colors { get; set; } = new List<Color>();
    public enum InputSelected
    {
        None,
        StartDate,
        EndDate,
        StartTime,
        EndTime,
        Color
    }
    public InputSelected selectedInput { get; set; } = InputSelected.None;
    public int Repeat {  get; set; }
    public Color Color { get; set; } = Colors.Black;
	public EventCreation()
	{
        NewEvent = new ObservableCollection<DayEvent>
            { 
                new DayEvent(StartDate, EndDate, "")
            };
        colors.Add(Colors.Purple);
        colors.Add(Colors.Orchid);
        colors.Add(Colors.Pink);
        colors.Add(Colors.Red);
        colors.Add(Colors.Orange);
        colors.Add(Colors.Yellow);
        colors.Add(Colors.Green);
        colors.Add(Colors.Blue);
        colors.Add(Colors.Purple);
        InitializeComponent();
        RenderInput();
	}

    private async void OnSubmitButtonClicked(object sender, EventArgs e)
    {
        DayEvent newEvent = new DayEvent(StartDate, EndDate, EventName.Text); 
        newEvent.Place = EventPlace.Text;
        newEvent.Description = EventDescription.Text;
        newEvent.Color = Color;
        newEvent.RepeatAfter = Repeat;
        await Shell.Current.GoToAsync("///MainPage", new Dictionary<string, object>
        {
            { "NewEvent", newEvent }
        });
    }

    private void OnInputSelected(object sender, EventArgs e)
    {
        if (sender is Button btn && Enum.TryParse<InputSelected>(btn.CommandParameter?.ToString(), out var input))
        {
            selectedInput = selectedInput == input ? InputSelected.None : input;
            RenderInput();
        }
    }
    public void RenderInput()
    {
        InputSection.Children.Clear();
        if (selectedInput == InputSelected.StartDate || selectedInput == InputSelected.EndDate)
        {
            var calendar = new CalendarView
            {
                Events = NewEvent,
                FontSize = 14,
                GenerateEvents = true,
            };
            calendar.SetBinding(
                CalendarView.SelectedDayProperty,
                new Binding(selectedInput == InputSelected.StartDate ? nameof(StartDate) : nameof(EndDate), source: this, mode: BindingMode.TwoWay)
            );
            calendar.SetBinding(
                CalendarView.EventsProperty,
                new Binding(nameof(NewEvent), source: this, mode: BindingMode.TwoWay)
            );
            InputSection.Children.Add(calendar);
            
        }
        else if (selectedInput == InputSelected.StartTime)
        {
            var timeSelector = new TimeSelectionView(StartTime);
            timeSelector.SetBinding(
                TimeSelectionView.SelectedTimeProperty,
                new Binding(nameof(StartTime), source: this, mode: BindingMode.TwoWay));
            InputSection.Children.Add(timeSelector);
        }
        else if (selectedInput == InputSelected.EndTime)
        {
            var timeSelector = new TimeSelectionView(EndTime);
            timeSelector.SetBinding(
                TimeSelectionView.SelectedTimeProperty,
                new Binding(nameof(EndTime), source: this, mode: BindingMode.TwoWay));
            InputSection.Children.Add(timeSelector);
        }
        else if (selectedInput == InputSelected.Color)
        {
            var colorSelector = new ColorSelectionView(colors);
            colorSelector.SetBinding(
                ColorSelectionView.SelectedColorProperty,
                new Binding(nameof(Color), source: this, mode: BindingMode.TwoWay));
            InputSection.Children.Add(colorSelector);

        }
    }
}