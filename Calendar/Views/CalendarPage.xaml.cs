using Calendar.Models;
using Calendar.ViewModels;
using Microsoft.Maui.Controls.Handlers.Items;
using Microsoft.Maui.Controls.Shapes;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Windows.Input;

namespace Calendar
{

    public partial class CalendarPage : ContentPage, IQueryAttributable
    {
        CalendarPageViewModel viewModel;
        public CalendarPage()
        {
            InitializeComponent();
            viewModel = new CalendarPageViewModel();
            CalendarHolder.Children.Add(viewModel.Calendar);
            viewModel.PropertyChanged += (s, e) =>
            {
                if(e.PropertyName == nameof(viewModel.SelectedDay))
                {
                    UpdateEventList();
                }

            };

            UpdateEventList();
        }

        #region public methods
        public void UpdateEventList()
        {
            EventDetails.Children.Clear();

            var selectedDay = viewModel.Calendar.ViewModel.Days.FirstOrDefault(d => d.Date == viewModel.SelectedDay.Date);
            if (selectedDay is null)
                return;

            if (selectedDay.Events.Count == 0)
            {
                EventDetails.Children.Add(new Label
                {
                    Text = "Na tento den není naplánovaná událost",
                    Opacity = .5,
                    HorizontalOptions = LayoutOptions.Center,
                });
            }
            else
            {
                foreach (DayEventViewModel dayEvent in selectedDay.Events)
                {
                    var detail = new EventDetailView(dayEvent);
                    detail.GestureRecognizers.Add(new TapGestureRecognizer
                    {
                        Command = new Command(() => OnEventSelected(dayEvent))
                    });
                    EventDetails.Children.Add(detail);
                }
            }
        }
        public void ApplyQueryAttributes(IDictionary<string, object?> query)
        {
            viewModel.ApplyQueryAttributes(query);
            UpdateEventList();
        }
        public void OnEventSelected(DayEventViewModel selectedEvent)
        {
            viewModel.SelectedEvent = selectedEvent;
        }
        public void OnAddButtonClicked(object sender, EventArgs e)
        {
            viewModel.NewEvent(sender, e);
        }
#endregion

        protected override void OnAppearing()
        {
            viewModel.selectedEvent = null;
            base.OnAppearing();
        }
    }
}
