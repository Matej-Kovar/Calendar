using Microsoft.Maui.Controls.Shapes;

namespace Calendar.Views;

public class ColorSelectionView : ContentView
{
	FlexLayout flex = new FlexLayout();

    public static readonly BindableProperty SelectedColorProperty =
    BindableProperty.Create(
        nameof(SelectedColor),
        typeof(Color),
        typeof(ColorSelectionView),
        Microsoft.Maui.Graphics.Colors.Transparent,
        BindingMode.TwoWay);
    public ColorSelectionView(List<Color> colors)
    {
        this.Colors = colors;
        Content = flex;
        RenderColorSelection();
    }

    public void RenderColorSelection()
    {
        flex.Children.Clear();
        flex.Wrap = Microsoft.Maui.Layouts.FlexWrap.Wrap;
        flex.Padding = 8;
        flex.HorizontalOptions = LayoutOptions.Center;
        flex.VerticalOptions = LayoutOptions.Center;
        foreach (var color in Colors)
        {
            var rect = new Rectangle
            {
                Fill = color,
                HeightRequest = 48,
                WidthRequest = 48,
                Stroke = SelectedColor == color ? (Color)Application.Current!.Resources["TextColor"] : Microsoft.Maui.Graphics.Colors.Transparent,
                StrokeThickness = 4,
                RadiusX = 8,
                RadiusY = 8,
                Margin = 1
            };
            rect.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => OnColorChanged(rect.Fill))
            });
            flex.Children.Add(rect);
        }
    }
    public void OnColorChanged(Brush brush)
    {
        var temp = (SolidColorBrush)brush;
        SelectedColor = temp.Color;
        RenderColorSelection();
    }

    public List<Color> Colors;
    public Color SelectedColor
    {
        get => (Color)GetValue(SelectedColorProperty);
        set => SetValue(SelectedColorProperty, value);
    }
}