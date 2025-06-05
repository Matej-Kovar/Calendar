
namespace Calendar;

public partial class TimeSelectionView : ContentView
{
    bool updating = false;
    DateTime TempSelectedTime;

    public static readonly BindableProperty SelectedTimeProperty =
    BindableProperty.Create(
    nameof(SelectedTime),
    typeof(DateTime),
    typeof(CalendarView),
    DateTime.Now,
    BindingMode.TwoWay
    );

    public TimeSelectionView(DateTime defaultTime)
    {
        InitializeComponent();
        TempSelectedTime = defaultTime;
        string hours = TempSelectedTime.Hour.ToString();
        string minutes = TempSelectedTime.Minute.ToString();
        HoursEntry.Placeholder = hours.Length == 1 ? "0" + hours : hours;
        MinutesEntry.Placeholder = minutes.Length == 1 ? "0" + minutes : minutes;
    }
    private void HoursEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (!updating)
        {
            updating = true;
            int temp = 0;
            Dispatcher.Dispatch(() =>
            {
                HoursEntry.Text = timeParser(e.NewTextValue, 23, ref temp);
                TempSelectedTime = new DateTime(TempSelectedTime.Year, TempSelectedTime.Month, TempSelectedTime.Day, temp, TempSelectedTime.Minute, 0);
                updating = false;
            });

        }
    }

    private void MinutesEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (!updating)
        {
            updating = true;
            int temp = 0;
            Dispatcher.Dispatch(() =>
            {
                MinutesEntry.Text = timeParser(e.NewTextValue, 59, ref temp);
                TempSelectedTime = new DateTime(TempSelectedTime.Year, TempSelectedTime.Month, TempSelectedTime.Day, TempSelectedTime.Hour, temp, 0);
                updating = false;
            }); 

        }
    }

    private void OnSubmitButtonClicked(object sender, EventArgs e)
    {
        SelectedTime = TempSelectedTime;
    }

    private string timeParser(string input, int max, ref int intVar)
    {
        intVar = 0;

        if (!int.TryParse(input, out int value))
            return "00";

        if (value <= 0)
        {
            intVar = 0;
            return "00";
        }

        int candidate = value % 100;
        if (candidate <= max)
        {
            intVar = candidate;
            return candidate.ToString("D2");
        }

        candidate = value % 10;
        if (candidate <= max)
        {
            intVar = candidate;
            return candidate.ToString("D2");
        }

        return "00";
    }

    public DateTime SelectedTime
    {
        get => (DateTime)GetValue(SelectedTimeProperty);
        set => SetValue(SelectedTimeProperty, value);
    }
}