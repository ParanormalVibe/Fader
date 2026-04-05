using System;
using Avalonia.Controls;

namespace Fader;

public partial class SettingsWindow : Window
{
    public event Action<double, double>? GradientChanged;
    public event Action<bool>? TopmostChanged;
    public event Action<bool>? ResizableChanged;
    public event Action<bool>? DraggableChanged;
    public bool IsClosed { get; private set; }

    private bool _suppressEvents;

    public SettingsWindow() : this(0, 1, true, false, false) { }

    public SettingsWindow(double start, double end, bool topmost, bool resizable, bool draggable)
    {
        InitializeComponent();

        StartSlider.Value = start;
        EndSlider.Value = end;
        TopmostCheckBox.IsChecked = topmost;
        ResizableCheckBox.IsChecked = resizable;
        DraggableCheckBox.IsChecked = draggable;

        StartSlider.PropertyChanged += (_, e) =>
        {
            if (_suppressEvents) return;
            if (e.Property == Slider.ValueProperty)
                GradientChanged?.Invoke(StartSlider.Value, EndSlider.Value);
        };

        EndSlider.PropertyChanged += (_, e) =>
        {
            if (_suppressEvents) return;
            if (e.Property == Slider.ValueProperty)
                GradientChanged?.Invoke(StartSlider.Value, EndSlider.Value);
        };

        TopmostCheckBox.IsCheckedChanged += (_, _) =>
        {
            if (_suppressEvents) return;
            TopmostChanged?.Invoke(TopmostCheckBox.IsChecked == true);
        };

        ResizableCheckBox.IsCheckedChanged += (_, _) =>
        {
            if (_suppressEvents) return;
            ResizableChanged?.Invoke(ResizableCheckBox.IsChecked == true);
        };

        DraggableCheckBox.IsCheckedChanged += (_, _) =>
        {
            if (_suppressEvents) return;
            DraggableChanged?.Invoke(DraggableCheckBox.IsChecked == true);
        };

        Closed += (_, _) => IsClosed = true;
    }

    public void UpdateValues(double start, double end, bool topmost, bool resizable, bool draggable)
    {
        _suppressEvents = true;
        StartSlider.Value = start;
        EndSlider.Value = end;
        TopmostCheckBox.IsChecked = topmost;
        ResizableCheckBox.IsChecked = resizable;
        DraggableCheckBox.IsChecked = draggable;
        _suppressEvents = false;
    }
}
