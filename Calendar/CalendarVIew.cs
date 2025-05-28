using Calendar.Model;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Layouts;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Calendar
{
    public partial class CalendarView : ContentView
    {
        private readonly Grid _calendarGrid = new();

        public CalendarView()
        {
            DayNames = DateTimeFormatInfo.CurrentInfo.DayNames.Select(n => n.Substring(0, 3).ToUpper()).ToArray();
            Content = _calendarGrid;
            RenderCalendar();
        }

        // Bindable properties
        public static readonly BindableProperty DaysProperty =
            BindableProperty.Create(nameof(Days), typeof(ObservableCollection<CalendarDay>), typeof(CalendarView), new ObservableCollection<CalendarDay>(), propertyChanged: (b, o, n) => ((CalendarView)b).RenderCalendar());

        public static readonly BindableProperty OnDayTappedProperty =
            BindableProperty.Create(nameof(OnDayTapped), typeof(Action<CalendarDay>), typeof(CalendarView), null);

        public static readonly BindableProperty GenerateEventsProperty =
            BindableProperty.Create(nameof(GenerateEvents), typeof(bool), typeof(CalendarView), true, propertyChanged: (b, o, n) => ((CalendarView)b).RenderCalendar());

        public static readonly BindableProperty FontSizeProperty =
            BindableProperty.Create(nameof(FontSize), typeof(double), typeof(CalendarView), 16.0, propertyChanged: (b, o, n) => ((CalendarView)b).RenderCalendar());

        public ObservableCollection<CalendarDay> Days
        {
            get => (ObservableCollection<CalendarDay>)GetValue(DaysProperty);
            set => SetValue(DaysProperty, value);
        }

        public Action<CalendarDay>? OnDayTapped
        {
            get => (Action<CalendarDay>?)GetValue(OnDayTappedProperty);
            set => SetValue(OnDayTappedProperty, value);
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
                    FontAttributes = FontAttributes.Bold,
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
                Text = day.Date.Day.ToString(),
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center
            };

            var border = new Border
            {
                Stroke = day.Stroke,
                StrokeThickness = FontSize / 8,
                BackgroundColor = day.Background,
                Padding = FontSize / 4,
                WidthRequest = FontSize * 2.25,
                HeightRequest = FontSize * 2.25,
                StrokeShape = new RoundRectangle { CornerRadius = FontSize * 4.5 },
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

            if (GenerateEvents && day.Events.Any())
            {
                var bubbleLayout = new FlexLayout
                {
                    Wrap = FlexWrap.Wrap,
                    JustifyContent = FlexJustify.Center,
                    AlignItems = FlexAlignItems.Center,
                    Padding = new Thickness(FontSize/2, 0, FontSize/2, 0)

                };

                foreach (var e in day.Events.Take(6))
                {
                    bubbleLayout.Children.Add(new Ellipse
                    {
                        Fill = e.Color,
                        Stroke = Colors.Transparent,
                        WidthRequest = FontSize / 2,
                        HeightRequest = FontSize / 2
                    });
                }

                stack.Children.Add(bubbleLayout);
            }

            return stack;
        }
    }
}