using Calendar.Model;
using Calendar.ViewModel;
using Microsoft.Maui.Controls.Handlers.Items;
using Microsoft.Maui.Controls.Shapes;
using System.Windows.Input;

namespace Calendar
{
    public partial class MainPage : ContentPage
    {
        private CalendarViewModel ViewModel;

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
                    RenderCalendar(_controlDate);
                }
            }
        }
        public MainPage()
        {
            //swipe nefunguje
            double panX = 0;
            var panGesture = new PanGestureRecognizer();
            panGesture.PanUpdated += (s, e) =>
            {
                switch (e.StatusType)
                {
                    case GestureStatus.Started:
                        panX = 0;
                        break;
                    case GestureStatus.Running:
                        panX += e.TotalX;
                        break;
                    case GestureStatus.Completed:
                        if (panX > 100)
                            PreviousMonth.Execute(null);
                        else if (panX < -100)
                            NextMonth.Execute(null);
                        break;
                }
            };

            InitializeComponent();
            ViewModel = new CalendarViewModel();
            BindingContext = ViewModel;
            RenderCalendar(ControlDate);
            CalendarHolder.GestureRecognizers.Add(panGesture);
        }

        void OnDayTapped(CalendarDay tappedDay)
        {
            ViewModel.SetSelectedDay(tappedDay);
        }

        void RenderCalendar(DateTime month)
        {
            CalendarGrid.Children.Clear();
            CalendarGrid.RowDefinitions.Clear();
            CalendarGrid.ColumnDefinitions.Clear();

            for (int col = 0; col < 7; col++)
            {
                CalendarGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            }

            for (int row = 0; row < 7; row++)
            {
                CalendarGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            }

            ViewModel.LoadMonth(month);

            for(int i  = 0; i < 7; i++)
            {
                var dayNameComponent = new Label
                {
                    Text = ViewModel.DayNames[i],
                    FontSize = 16, 
                   HorizontalTextAlignment = TextAlignment.Center
                };
                CalendarGrid.Children.Add(dayNameComponent);
                Grid.SetColumn(dayNameComponent, i);
            }

            for (int i = 0; i < ViewModel.Days.Count; i++)
            {
                int row = (i + 7) / 7;
                int col = (i + 7) % 7;
                var day = ViewModel.Days[i];

                var border = RenderDay(day);

                CalendarGrid.Children.Add(border);
                Grid.SetRow(border, row);
                Grid.SetColumn(border, col);

            }

        }

        public Border RenderDay(CalendarDay day)
        {
            /*for (int i = 0; i < 12; i++)
            {
                day.Events.Add(new DayEvent(day.Date, day.Date, "test"));
            }*/
            var selectHighlight = new Border
            {
                Stroke = day.Stroke,
                StrokeThickness = 2,
                BackgroundColor = day.Background,
                Padding = 4,
                WidthRequest = 36,
                HeightRequest = 36,
                StrokeShape = new RoundRectangle
                {
                    CornerRadius = 72
                },
                Content = new Label
                {
                    Text = day.Date.Day.ToString(),
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center
                }
            };

            selectHighlight.SetBinding(Border.StrokeProperty, new Binding("Stroke"));
            selectHighlight.SetBinding(Border.BackgroundColorProperty, new Binding("Background"));
            selectHighlight.BindingContext = day;

            var eventNotifications = new FlexLayout
            {
                Wrap = Microsoft.Maui.Layouts.FlexWrap.Wrap,
                JustifyContent = Microsoft.Maui.Layouts.FlexJustify.Center,
                AlignItems = Microsoft.Maui.Layouts.FlexAlignItems.Center,
            };

            for (int i = 0; i < Math.Min(6, day.Events.Count); i++)
            {
                var eventBubble = new Ellipse
                {
                    Fill = day.Events[i].Color,
                    Stroke= Colors.Transparent,
                    WidthRequest = 8,
                    HeightRequest = 8,
                };
                eventNotifications.Children.Add(eventBubble);
            }

            var verticalStack = new VerticalStackLayout
            {
                Spacing = 2,
            };
            verticalStack.Children.Add(selectHighlight);
            verticalStack.Children.Add(eventNotifications);

            var border = new Border
            {
                Stroke = Colors.Transparent,
                Padding = 4,
                GestureRecognizers =
                    {
                        new TapGestureRecognizer
                        {
                            Command = new Command(() => OnDayTapped(day))
                        }
                    },
                Content = verticalStack
            };

            return border;
        }

        public ICommand NextMonth => new Command(() => ControlDate = ControlDate.AddMonths(1));
        public ICommand PreviousMonth => new Command(() => ControlDate = ControlDate.AddMonths(-1));
    }
}
