using Calendar.Models;
using Calendar.ViewModels;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Layouts;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Calendar
{
    public partial class CalendarView : ContentView
    {
        private readonly Grid calendarGrid = new();

        VerticalStackLayout verticalStackLayout = new();

        private CalendarViewModel viewModel = new();
        public CalendarViewModel ViewModel => viewModel;

        public ObservableCollection<DayEvent> Events
        {
            get => ViewModel.Events;
            set
            {
                ViewModel.Events = value;
                ViewModel.LoadMonth();
                Dispatcher.Dispatch(() => RenderCalendar());
            }
        }

        public DateTime SelectedDay
        {
            get => ViewModel.SelectedDay;
            set { ViewModel.SelectedDay = value; viewModel.LoadMonth(); }
        }
        public CalendarView()
        {
            BindingContext = viewModel;

            verticalStackLayout.Children.Clear();
            verticalStackLayout.Children.Add(generateControls());
            verticalStackLayout.Children.Add(calendarGrid);
            Content = verticalStackLayout;

            viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(viewModel.Days) || e.PropertyName == nameof(viewModel.ControlDate) || e.PropertyName == nameof(viewModel.Events))
                {
                    Dispatcher.Dispatch(() => RenderCalendar());
                }
            };
            //Dispatcher.Dispatch(() => RenderCalendar());
        }

        public static readonly BindableProperty GenerateEventsProperty =
            BindableProperty.Create(nameof(GenerateEvents), typeof(bool), typeof(CalendarView), true);

        public static readonly BindableProperty FontSizeProperty =
            BindableProperty.Create(nameof(FontSize), typeof(double), typeof(CalendarView), 16.0);

        private static void OnSelectedDayChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var calendar = (CalendarView)bindable;
            calendar.viewModel.ControlDate = calendar.viewModel.SelectedDay.Date;
        }

        private static void OnEventsChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var calendar = (CalendarView)bindable;
            calendar.Events = (ObservableCollection<DayEvent>)newValue;
        }

        public bool GenerateEvents
        {
            get => (bool)GetValue(GenerateEventsProperty);
            set => SetValue(GenerateEventsProperty, value);
        }

        public double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        public void RenderCalendar()
        {
            Debug.WriteLine("Started calendar rendering");

            calendarGrid.Children.Clear();
            calendarGrid.RowDefinitions.Clear();
            calendarGrid.ColumnDefinitions.Clear();

            for (int i = 0; i < 7; i++)
            {
                calendarGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
                calendarGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            }

            for (int i = 0; i < 7; i++)
            {
                var header = new Label
                {
                    Text = viewModel.DayNames[i],
                    FontSize = FontSize,
                    HorizontalTextAlignment = TextAlignment.Center
                };
                calendarGrid.Children.Add(header);
                Grid.SetColumn(header, i);
                Grid.SetRow(header, 0);
            }

            for (int i = 0; i < viewModel.Days.Count; i++)
            {
                int row = (i + 7) / 7;
                int col = (i + 7) % 7;
                var day = viewModel.Days[i];
                var dayView = RenderDay(day);

                calendarGrid.Children.Add(dayView);
                Grid.SetRow(dayView, row);
                Grid.SetColumn(dayView, col);
            }
            Debug.WriteLine("Finished calendar rendering");
        }

        private View RenderDay(DayViewModel day)
        {
            var label = new Label
            {
                FontSize = FontSize,
                Style = Application.Current!.Resources["Number"] as Style,
                Opacity = day.Opacity,
                Text = day.Date.Day.ToString(),
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            };
            if (day.IsToday)
            {
                label.Style = (Style)Application.Current.Resources["TextOnColorStyle"];
            }

            var border = new Border
            {
                Stroke = day.Stroke,
                StrokeThickness = 3,
                BackgroundColor = day.Background,
                Padding = FontSize / 8,
                WidthRequest = FontSize * 2.25,
                HeightRequest = FontSize * 2.25,
                StrokeShape = new RoundRectangle { CornerRadius = FontSize * 4.25 },
                Content = label,
                BindingContext = day
            };

            border.SetBinding(Border.StrokeProperty, "Stroke");
            border.SetBinding(Border.BackgroundColorProperty, "Background");

            var stack = new VerticalStackLayout
            {
                Spacing = FontSize / 8,
                Padding = FontSize / 4
            };

            stack.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => viewModel.SetSelectedDay(day))
            });

            stack.Children.Add(border);

            if (GenerateEvents)
            {
                var bubbleLayout = new FlexLayout
                {
                    Wrap = FlexWrap.Wrap,
                    JustifyContent = FlexJustify.Center,
                    AlignItems = FlexAlignItems.Center,
                    HeightRequest = 20,
                    WidthRequest = 26
                };

                foreach (var e in day.Events.Take(6))
                {
                    bubbleLayout.Children.Add(new Ellipse
                    {
                        Fill = e.Color,
                        Stroke = Colors.Transparent,
                        WidthRequest = FontSize / 2,
                        HeightRequest = FontSize / 2,
                    });
                }

                stack.Children.Add(bubbleLayout);
            }

            return stack;
        }

        FlexLayout generateControls()
        {
            var layout = new FlexLayout
            {
                JustifyContent = FlexJustify.SpaceBetween,
                Padding = FontSize / 2,
            };

            var previousButton = new Button
            {
                HorizontalOptions = LayoutOptions.End,
                Text = (string)Application.Current.Resources["ArrowLeftIcon"],
                BindingContext = viewModel,
                Style = (Style)Application.Current!.Resources["IconButton"],
                HeightRequest = FontSize * 2.25,
                WidthRequest = FontSize * 2.25
            };
            previousButton.SetBinding(Button.CommandProperty, nameof(viewModel.PreviousMonthCommand));

            var monthLabel = new Label
            {
                HorizontalOptions = LayoutOptions.Start,
                FontSize = FontSize * 1.5,
                BindingContext = viewModel
            };
            monthLabel.SetBinding(Label.TextProperty, nameof(viewModel.Header));

            var nextButton = new Button
            {
                HorizontalOptions = LayoutOptions.End,
                Text = (string)Application.Current.Resources["ArrowRightIcon"],
                BindingContext = viewModel,
                Style = (Style)Application.Current.Resources["IconButton"],
                HeightRequest = FontSize * 2.25,
                WidthRequest = FontSize * 2.25

            };
            nextButton.SetBinding(Button.CommandProperty, nameof(viewModel.NextMonthCommand));

            var buttons = new HorizontalStackLayout();
            buttons.Spacing = FontSize / 4;
            buttons.Add(previousButton);
            buttons.Add(nextButton);

            layout.Children.Add(monthLabel);
            layout.Children.Add(buttons);

            return layout;
        }


    }
}