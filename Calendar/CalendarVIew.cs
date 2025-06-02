using Calendar.Model;
using Calendar.ViewModel;
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
        private readonly Grid _calendarGrid = new();

        VerticalStackLayout _verticalStackLayout = new();

        private DateTime _controlDate = DateTime.Now;
        public DateTime ControlDate
        {
            get => _controlDate;
            set
            {
                if (_controlDate != value)
                {
                    _controlDate = value;
                    OnPropertyChanged(nameof(ControlDate));
                    RenderCalendar();
                }
            }
        }

        public string Header
        {
            get
            {
                string header = ControlDate.ToString("MMMM yyyy");
                return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(header);
            }
        }
            

        public CalendarView()
        {
            DayNames = DateTimeFormatInfo.CurrentInfo.DayNames.Select(n => n.Substring(0, 3).ToUpper()).ToArray();
            _verticalStackLayout.Children.Clear();
            _verticalStackLayout.Children.Add(generateControls());
            _verticalStackLayout.Children.Add(_calendarGrid);
            Content = _verticalStackLayout;
            Dispatcher.Dispatch(() => RenderCalendar());
        }

        public static readonly BindableProperty GenerateEventsProperty =
            BindableProperty.Create(nameof(GenerateEvents), typeof(bool), typeof(CalendarView), true);

        public static readonly BindableProperty FontSizeProperty =
            BindableProperty.Create(nameof(FontSize), typeof(double), typeof(CalendarView), 16.0);
        public List<CalendarDay> Days { get; set; } = new List<CalendarDay>();

        public static readonly BindableProperty EventsProperty =
            BindableProperty.Create(
            nameof(Events),
            typeof(ObservableCollection<DayEvent>),
            typeof(CalendarView),
            new ObservableCollection<DayEvent>(),
            BindingMode.TwoWay
        );

        public static readonly BindableProperty SelectedDayProperty =
            BindableProperty.Create(
                nameof(SelectedDay),
                typeof(DateTime),
                typeof(CalendarView),
                DateTime.Now,
                BindingMode.TwoWay,
                propertyChanged: OnSelectedDayChanged
            );
        private static void OnSelectedDayChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var calendar = (CalendarView)bindable;
            
            calendar.Dispatcher.Dispatch(() => calendar.RenderCalendar());
        }

        public DateTime SelectedDay
        {
            get => (DateTime)GetValue(SelectedDayProperty);
            set => SetValue(SelectedDayProperty, value);
        }

        public void LoadMonth(DateTime month, ObservableCollection<DayEvent> events)
        {
            Days.Clear();
            var firstOfMonth = new DateTime(month.Year, month.Month, 1);
            var lastOfMonth = new DateTime(month.Year, month.Month, DateTime.DaysInMonth(month.Year, month.Month));
            int offsetBefore = (int)firstOfMonth.DayOfWeek;
            int offsetAfter = 7 - (int)lastOfMonth.DayOfWeek;
            var startDate = firstOfMonth.AddDays(-offsetBefore);
            int total = DateTime.DaysInMonth(month.Year, month.Month) + offsetAfter + offsetBefore - 1;

            for (int i = 0; i < total; i++)
            {
                var date = startDate.AddDays(i);
                Days.Add(new CalendarDay
                {
                    Events = events.Where(e => e.isInRange(date)).ToList(),
                    Date = date,
                    IsCurrentMonth = date.Month == month.Month,
                    IsToday = date.Date == DateTime.Now.Date,
                    IsSelected = date.Date == SelectedDay.Date
                });
            }
        }

        public void SetSelectedDay(CalendarDay tappedDay)
        {
            SelectedDay = tappedDay.Date;
            foreach (var day in Days)
                day.IsSelected = false;
            tappedDay.IsSelected = true;
        }
        public ObservableCollection<DayEvent> Events
        {
            get => (ObservableCollection<DayEvent>)GetValue(EventsProperty);
            set => SetValue(EventsProperty, value);
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

        string[] DayNames { get; set; }

        public void RenderCalendar()
        {
            LoadMonth(ControlDate, Events);

            _calendarGrid.Children.Clear();
            _calendarGrid.RowDefinitions.Clear();
            _calendarGrid.ColumnDefinitions.Clear();

            for (int i = 0; i < 7; i++)
            {
                _calendarGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
                _calendarGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            }

            for (int i = 0; i < 7; i++)
            {
                var header = new Label
                {
                    Text = DayNames[i],
                    FontSize = FontSize,
                    HorizontalTextAlignment = TextAlignment.Center
                };
                _calendarGrid.Children.Add(header);
                Grid.SetColumn(header, i);
                Grid.SetRow(header, 0);
            }

            for (int i = 0; i < Days.Count; i++)
            {
                int row = (i + 7) / 7;
                int col = (i + 7) % 7;
                var day = Days[i];
                var dayView = RenderDay(day);

                _calendarGrid.Children.Add(dayView);
                Grid.SetRow(dayView, row);
                Grid.SetColumn(dayView, col);
            }
        }

        private View RenderDay(CalendarDay day)
        {
            var label = new Label
            {
                FontSize = FontSize,
                Style = (Style)Application.Current.Resources["Number"],
                Opacity = day.Opacity,
                Text = day.Date.Day.ToString(),
                TextColor = day.IsToday ? (Color)Application.Current.Resources["TextOnColor"] : (Color)Application.Current.Resources["TextColor"],
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            };

            var border = new Border
            {
                Stroke = day.Stroke,
                StrokeThickness = 2,
                BackgroundColor = day.Background,
                Padding = FontSize / 4,
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

            if (OnDayTapped != null)
            {
                stack.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = new Command(() => OnDayTapped(day))
                });
            }

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

        void OnDayTapped(CalendarDay tappedDay)
        {
            SetSelectedDay(tappedDay);
        }
        public ICommand NextMonth => new Command(() => ControlDate = ControlDate.AddMonths(1));
        public ICommand PreviousMonth => new Command(() => ControlDate = ControlDate.AddMonths(-1));

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
                Text = "\ue901",
                BindingContext = this,
                Style = (Style)Application.Current.Resources["IconButton"],
                HeightRequest = FontSize * 2.25,
                WidthRequest = FontSize * 2.25
            };
            previousButton.SetBinding(Button.CommandProperty, "PreviousMonth");

            var monthLabel = new Label
            {
                HorizontalOptions = LayoutOptions.Start,
                FontSize = FontSize * 1.5,
                BindingContext = this
            };
            monthLabel.SetBinding(Label.TextProperty, new Binding("Header"));

            var nextButton = new Button
            {
                HorizontalOptions = LayoutOptions.End,
                Text = "\ue902",
                BindingContext = this,
                Style = (Style)Application.Current.Resources["IconButton"],
                HeightRequest = FontSize * 2.25,
                WidthRequest = FontSize * 2.25

            };
            nextButton.SetBinding(Button.CommandProperty, "NextMonth");

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