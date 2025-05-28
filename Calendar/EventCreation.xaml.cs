using Calendar.Model;
using System.ComponentModel;
using System.Globalization;

namespace Calendar;

public partial class EventCreation : ContentPage, INotifyPropertyChanged
{
    public new event PropertyChangedEventHandler? PropertyChanged;

    private DateTime _startDate = DateTime.Now;
    public DateTime StartDate
    {
        get => _startDate;
        set
        {
            if (_startDate != value)
            {
                _startDate = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StartDate)));
            }
        }
    }

    private DateTime _endDate = DateTime.Now;
    public DateTime EndDate
    {
        get => _endDate;
        set
        {
            if (_endDate != value)
            {
                _endDate = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EndDate)));
            }
        }
    }
    public enum InputSelected
    {
        None,
        StartDate,
        EndDate,
        StartTime,
        EndTime
    }
    public InputSelected selectedInput { get; set; } = InputSelected.None;
    public int Repeat {  get; set; }
    public Color Color { get; set; } = Colors.Black;
	public EventCreation()
	{
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
                FontSize = 14,
                GenerateEvents = false,
            };

            calendar.SetBinding(
                CalendarView.SelectedDayProperty,
                new Binding(selectedInput == InputSelected.StartDate ? nameof(StartDate) : nameof(EndDate), source: this, mode: BindingMode.TwoWay)
            );
            InputSection.Children.Add(calendar);
            
        }
    }
}