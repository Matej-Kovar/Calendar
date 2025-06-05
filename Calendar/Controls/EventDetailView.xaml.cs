using Calendar.ViewModels;
using Calendar.Resources;

namespace Calendar;

public partial class EventDetailView : ContentView
{
    public EventDetailView(DayEventViewModel dayEvent)
    {
        this.dayEvent = dayEvent;
        InitializeComponent();
        if (dayEvent.Description == null || dayEvent.Description == string.Empty)
        {
            DescriptionLabel.HeightRequest = 0;
        }

    }
    public DayEventViewModel dayEvent { get; set; }

    public string TimeRange
    {
        get
        {
            var start = dayEvent.StarDate;
            var end = dayEvent.EndDate;
            var today = DateTime.Now.Date;

            if (start.Date == end.Date && start.Date == today.Date)
            {
                return $"{Strings.Today} {Strings.EventStartDate} {start:HH\\:mm} {Strings.EventEndDate} {end:HH\\:mm}";
            }
            else if (start.Date == today)
            {
                return $"{Strings.Today} {Strings.EventStartDate} {start:HH\\:mm} {Strings.EventEndDate} {end:dd.MM.yyyy}";
            }
            else if (end.Date == today)
            {
                return $"{Strings.EventStartDate} {start:dd.MM.yyyy} {Strings.EventEndDate} {Strings.Today} {end:HH\\:mm}";
            }
            else
            {
                return $"{Strings.EventStartDate} {start:dd.MM.yyyy} {Strings.EventEndDate} {end:dd.MM.yyyy}";
            }
        }
    }

}