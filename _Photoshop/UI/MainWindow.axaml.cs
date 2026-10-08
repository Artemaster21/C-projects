using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;

namespace MyPhotoshop;

public partial class MainWindow : Window
{
	private Bitmap _originalBmp;
	private Photo _originalPhoto;
	private readonly List<NumericUpDown> _parametersControls = [];
	private readonly List<IFilter> _filters = [];

	public MainWindow()
	{
		InitializeComponent();

		FiltersSelect.SelectionChanged += FilterChanged;
		ApplyButton.Click += Process;
		FiltersSelect.ItemsSource ??= _filters;

		LoadBitmap("cat.jpg");
	}

	private void LoadBitmap(string path)
	{
		_originalBmp = new Bitmap(path);
		_originalPhoto = Convertors.Bitmap2Photo(_originalBmp);

		OriginalImage.Source = _originalBmp.AsAvaloniaBitmap();
		ProcessedImage.Source = _originalBmp.AsAvaloniaBitmap();
	}

	public void AddFilter(IFilter filter)
	{
		_filters.Add(filter);

		if (FiltersSelect.SelectedIndex != -1)
			return;

		FiltersSelect.SelectedIndex = 0;
		ApplyButton.IsEnabled = true;
	}

	private void FilterChanged(object? sender, SelectionChangedEventArgs e)
	{
		if (FiltersSelect.SelectedItem is not IFilter filter) return;

		ParametersPanel.Children.Clear();
		_parametersControls.Clear();

		foreach (var param in filter.GetParameters())
		{
			var label = new TextBlock
			{
				Text = param.Name,
				Margin = new Thickness(0, 5)
			};
			ParametersPanel.Children.Add(label);

			var box = new NumericUpDown
			{
				Minimum = (decimal)param.MinValue,
				Maximum = (decimal)param.MaxValue,
				Value = (decimal)param.DefaultValue,
				Increment = (decimal)param.Increment / 3m,
				Width = 150,
				FormatString = "F2",
			};
			ParametersPanel.Children.Add(box);
			_parametersControls.Add(box);
		}
	}

	private void Process(object? sender, RoutedEventArgs e)
	{
		var data = _parametersControls
			.Select(z => (double)(z.Value ?? 0))
			.ToArray();

		var filter = (IFilter)FiltersSelect.SelectedItem!;
		var result = filter.Process(_originalPhoto, data);

		var resultBmp = Convertors.Photo2Bitmap(result).AsAvaloniaBitmap();
		var originalBmp = _originalBmp.AsAvaloniaBitmap();

		if (resultBmp.Size.Width > originalBmp.Size.Width ||
		    resultBmp.Size.Height > originalBmp.Size.Height)
		{
			var scale = Math.Min(
				originalBmp.Size.Width / resultBmp.Size.Width,
				originalBmp.Size.Height / resultBmp.Size.Height);

			using var scaled = resultBmp.CreateScaledBitmap(new PixelSize(
				(int)(resultBmp.Size.Width * scale),
				(int)(resultBmp.Size.Height * scale)));

			ProcessedImage.Source = scaled;
		}
		else
		{
			ProcessedImage.Source = resultBmp;
		}
	}
}