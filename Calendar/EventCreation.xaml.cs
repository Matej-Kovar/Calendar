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
    public enum InputSelected
    {
        None,
        StartDate,
        EndDate,
        StartTime,
        EndTime
    }
    public InputSelected selectedInput { get; set; } = InputSelected.None;
    public DateTime EndDate { get; set; } = DateTime.Now;
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

    public void RenderInput()
    {
        InputSection.Children.Clear();
        if (/*selectedInput == InputSelected.StartDate || selectedInput == InputSelected.EndDate*/true)
        {
            var calendar = new CalendarView
            {
                FontSize = 14,
                GenerateEvents = false,
            };
            calendar.SetBinding(CalendarView.SelectedDayProperty, new Binding(nameof(StartDate), source: this, mode: BindingMode.TwoWay));
            InputSection.Children.Add(calendar);
            
        }
    }
}