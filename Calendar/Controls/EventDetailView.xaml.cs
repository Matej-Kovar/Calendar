using Calendar.ViewModels;

namespace Calendar;

public partial class EventDetailView : ContentView
{
	public DayEventViewModel dayEvent { get; set; }

    public string TimeRange
    {
        get
        {
            var start = dayEvent.StarDate;
            var end = dayEvent.EndDate;
            var today = DateTime.Now.Date;

            if (start.Date == end.Date)
            {
                return $"Dnes {start:HH\\:mm} až {end:HH\\:mm}";
            }
            else if (start.Date == today)
            {
                return $"Dnes {start:HH\\:mm} až {end:dd.MM.yyyy}";
            }
            else if (end.Date == today)
            {
                return $"{start:dd.MM.yyyy} až Dnes {end:HH\\:mm}";
            }
            else
            {
                return $"{start:dd.MM.yyyy} až {end:dd.MM.yyyy}";
            }
        }
    }
    public EventDetailView(DayEventViewModel dayEvent)
	{
		this.dayEvent = dayEvent;
        InitializeComponent();
        if (dayEvent.Description == null || dayEvent.Description == string.Empty)
        {
            DescriptionLabel.HeightRequest = 0;
        }
		
	}
}