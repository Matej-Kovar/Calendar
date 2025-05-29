using Microsoft.Maui.Controls.Shapes;

namespace Calendar;

public class ColorSelectionView : ContentView
{
	FlexLayout flex = new FlexLayout();

	public List<Color> colors;
    public static readonly BindableProperty SelectedColorProperty =
    BindableProperty.Create(
        nameof(SelectedColor),
        typeof(Color),
        typeof(ColorSelectionView),
        Colors.Transparent,
		BindingMode.TwoWay);

    public Color SelectedColor
    {
        get => (Color)GetValue(SelectedColorProperty);
        set => SetValue(SelectedColorProperty, value);
    }

    public ColorSelectionView(List<Color> colors)
	{
		this.colors = colors;
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
		foreach (var color in colors)
		{
			var rect = new Rectangle
			{
				Fill = color,
				HeightRequest = 48,
				WidthRequest = 48,
				Stroke = SelectedColor == color ? Colors.Black: Colors.Transparent,
				StrokeThickness = 3,
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
}