using Calendar.Model;
using Calendar.ViewModel;
using Microsoft.Maui.Controls.Handlers.Items;
using Microsoft.Maui.Controls.Shapes;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Calendar
{
    public partial class MainPage : ContentPage, IQueryAttributable
    {
        CalendarView Calendar;

        public ObservableCollection<DayEvent> Events = new ObservableCollection<DayEvent>();

        public MainPage()
        {
            InitializeComponent();
            Calendar = new CalendarView
            {
                Events = Events,
                FontSize = 16,
                GenerateEvents = true,
            };
            CalendarHolder.Children.Add(Calendar);
        }

        private async void OnAddButtonClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(EventCreation));
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.ContainsKey("NewEvent"))
            {
                DayEvent dayEvent = (DayEvent)query["NewEvent"];
                Events.Add(dayEvent);
                UpdateCalendar();
                query.Clear();
            }
        }

        public void UpdateCalendar()
        {
            Calendar.Events = Events;
            Calendar.RenderCalendar();
        }
    }
}
