
namespace Calendar;

public partial class TimeSelectionView : ContentView
{
    public static readonly BindableProperty SelectedTimeProperty =
    BindableProperty.Create(
    nameof(SelectedTime),
    typeof(DateTime),
    typeof(CalendarView),
    DateTime.Now,
    BindingMode.TwoWay
    );
    public DateTime SelectedTime
    {
        get => (DateTime)GetValue(SelectedTimeProperty);
        set => SetValue(SelectedTimeProperty, value);
    }

    DateTime TempSelectedTime;

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
        int temp = 0;
        HoursEntry.Text = timeParser(e.NewTextValue, 23, ref temp);
        TempSelectedTime = new DateTime(TempSelectedTime.Year, TempSelectedTime.Month, TempSelectedTime.Day, temp, TempSelectedTime.Minute, 0);
    }

    private void MinutesEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        int temp = 0;
        MinutesEntry.Text = timeParser(e.NewTextValue, 59, ref temp);
        TempSelectedTime = new DateTime(TempSelectedTime.Year, TempSelectedTime.Month, TempSelectedTime.Day, TempSelectedTime.Hour, temp, 0);
    }

    private void OnSubmitButtonClicked(object sender, EventArgs e)
    {
        SelectedTime = TempSelectedTime;
    }

    private string timeParser(string input, int max, ref int intVar)
    {
        string output = "00";
        if (int.TryParse(input, out int minute))
        {
            if (minute <= 0)
            {
                output = "00";
                intVar = 0;
            }
            else if (minute > max)
            {
                if ((minute % 100) > max)
                {
                    intVar = minute % 10;
                    output = intVar.ToString();
                }
                else
                {
                    intVar = minute % 100;
                    output = intVar.ToString();
                }
            }
            else if(minute < 10)
            {
                output = "0" + minute.ToString();
                intVar = minute;
            }
            else
            {
                output = minute.ToString();
                intVar = minute;
            }

        }
        return output;
    }
}