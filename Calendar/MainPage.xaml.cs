using Calendar.ViewModel;
using Calendar.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Input;

namespace Calendar
{
    public partial class MainPage : ContentPage
    {
        private CalendarViewModel ViewModel;

        public MainPage()
        {
            InitializeComponent();
            ViewModel = new CalendarViewModel();
            BindingContext = ViewModel;
        }

        void OnDayTapped(object sender, TappedEventArgs e)
        {
            if (e.Parameter is CalendarDay tappedDay)
            {
                ViewModel.SetSelectedDay(tappedDay);
            }
        }
    }
}
