using Calendar.Models;
using Calendar.Views;
using Calendar.ViewModels;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;

namespace Calendar;

public partial class EventCreation : ContentPage, IQueryAttributable
{
    CalendarView calendarView { get; set; }
    public EventCreationViewModel viewModel { get; }
    public enum InputSelected { None, StartDate, EndDate, StartTime, EndTime, Color }
    public EventCreation()
    {
        viewModel = new EventCreationViewModel();
        BindingContext = viewModel;
        InitializeComponent();
        calendarView = new CalendarView
        {
            FontSize = 14,
            GenerateEvents = true,
            Events = new ObservableCollection<DayEventViewModel> { viewModel.NewEvent },
        };
        calendarView.ViewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(CalendarViewModel.SelectedDay))
            {
                if (SelectedInput == InputSelected.StartDate)
                    viewModel.StartDate = calendarView.SelectedDay;
                else
                    viewModel.EndDate = calendarView.SelectedDay;
            }
        };

        EventName.SetBinding(Entry.TextProperty, new Binding(nameof(viewModel.EventName), source: viewModel, mode: BindingMode.TwoWay));
        EventDescription.SetBinding(Entry.TextProperty, new Binding(nameof(viewModel.EventDescription), source: viewModel, mode: BindingMode.TwoWay));
        EventPlace.SetBinding(Entry.TextProperty, new Binding(nameof(viewModel.EventPlace), source: viewModel, mode: BindingMode.TwoWay));
        EventRepeatAfter.SetBinding(Entry.TextProperty, new Binding(nameof(viewModel.EventRepeatAfter), source: viewModel, mode: BindingMode.TwoWay));
        viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(viewModel.IsModifying) && viewModel.IsModifying)
            {
                CreateTrashButton();
            }
            if (e.PropertyName == nameof(viewModel.IsDataValid) && !viewModel.IsDataValid)
            {
                InvalidBorder(EventNameBorder);
            }
        };
        viewModel.NewEvent.PropertyChanged += (s, e) =>
        {
            if (!viewModel.IsInit)
            {
                //Debug.WriteLine($"Rendered Input on change of {e.PropertyName}");
                calendarView.Events = new ObservableCollection<DayEventViewModel> { viewModel.NewEvent };
                //Dispatcher.Dispatch(() => calendarView.RenderCalendar());
                
            }
        };
    }

    #region public methods
    public void RenderInput()
    {
        InputSection.Children.Clear();

        switch (SelectedInput)
        {
            case InputSelected.StartDate:
            case InputSelected.EndDate:
                calendarView.SelectedDay = SelectedInput == InputSelected.StartDate ? viewModel.StartDate.Date : viewModel.EndDate.Date;
                calendarView.RenderCalendar();
                InputSection.Children.Add(calendarView);
                break;

            case InputSelected.StartTime:
                var startTimeSelector = new TimeSelectionView(viewModel.StartTime);
                startTimeSelector.SetBinding(TimeSelectionView.SelectedTimeProperty,
                    new Binding(nameof(viewModel.StartTime), source: viewModel, mode: BindingMode.TwoWay));
                InputSection.Children.Add(startTimeSelector);
                break;

            case InputSelected.EndTime:
                var endTimeSelector = new TimeSelectionView(viewModel.EndTime);
                endTimeSelector.SetBinding(TimeSelectionView.SelectedTimeProperty,
                    new Binding(nameof(viewModel.EndTime), source: viewModel, mode: BindingMode.TwoWay));
                InputSection.Children.Add(endTimeSelector);
                break;

            case InputSelected.Color:
                var colorSelector = new ColorSelectionView(viewModel.Colors);
                colorSelector.SetBinding(ColorSelectionView.SelectedColorProperty,
                    new Binding(nameof(viewModel.Color), source: viewModel, mode: BindingMode.TwoWay));
                InputSection.Children.Add(colorSelector);
                break;
        }
    }

    private void OnInputSelected(object sender, EventArgs e)
    {
        if (sender is Button btn &&
            Enum.TryParse<InputSelected>(btn.CommandParameter?.ToString(), out var input))
        {
            SelectedInput = SelectedInput == input ? InputSelected.None : input;
            //Debug.WriteLine("Rendered input on Input selected");
            RenderInput();
        }
    }

    public void CreateTrashButton()
    {
        var button = new Button
        {
            Text = (string)Application.Current!.Resources["TrashIcon"],
            Style = (Style)Application.Current.Resources["IconButton"]
        };
        button.Clicked += OnRemoveButtonClicked;

        Controls.Children.Add(Controls.Children[1]);
        Controls.Children[1] = button;
    }

    public void InvalidBorder(Border border)
    {
        border.Stroke = (Color)Application.Current!.Resources["Pink"];
        border.StrokeThickness = 3;
    }

    public void OnRemoveButtonClicked(object? sender, EventArgs e)
    {
        viewModel.RemoveEvent();
    }

    public void OnSubmitButtonClicked(object sender, EventArgs e)
    {
        viewModel.CreateNewEvent();
    }

    public void OnBackButtonClicked(object sender, EventArgs e)
    {
        viewModel.NavigateBack();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        viewModel.ApplyQueryAttributes(query);
    }
    #endregion

    public InputSelected SelectedInput { get; set; } = InputSelected.None;
}
