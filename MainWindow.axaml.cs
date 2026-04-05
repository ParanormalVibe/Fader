using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Immutable;

namespace Fader;

public partial class MainWindow : Window
{
    private double _gradientStart = 0.0;
    private double _gradientEnd = 1.0;
    private bool _isResizable;
    private bool _isDraggable;
    private SettingsWindow? _settingsWindow;
    private ProfilesWindow? _profilesWindow;
    private AppSettings _settings;

    private const int ResizeGripSize = 8;

    public MainWindow()
    {
        InitializeComponent();

        _settings = AppSettings.Load();
        ApplyProfile(_settings.ActiveProfile);

        if (!double.IsNaN(_settings.WindowWidth) && !double.IsNaN(_settings.WindowHeight))
        {
            Width = _settings.WindowWidth;
            Height = _settings.WindowHeight;
        }
        if (!double.IsNaN(_settings.WindowX) && !double.IsNaN(_settings.WindowY))
        {
            WindowStartupLocation = WindowStartupLocation.Manual;
            Position = new PixelPoint((int)_settings.WindowX, (int)_settings.WindowY);
        }

        GradientPanel.PointerPressed += OnPanelPointerPressed;
        GradientPanel.PointerMoved += OnPanelPointerMoved;

        Closing += (_, _) =>
        {
            _settings.WindowX = Position.X;
            _settings.WindowY = Position.Y;
            _settings.WindowWidth = Bounds.Width;
            _settings.WindowHeight = Bounds.Height;
            _settings.Save();
        };
    }

    private void ApplyProfile(Profile profile)
    {
        _gradientStart = profile.GradientStart;
        _gradientEnd = profile.GradientEnd;
        _isResizable = profile.Resizable;
        _isDraggable = profile.Draggable;
        Topmost = profile.Topmost;
        CanResize = profile.Resizable;
        UpdateDecorations();
        UpdateGradient();

        if (_settingsWindow is not null && !_settingsWindow.IsClosed)
        {
            _settingsWindow.UpdateValues(
                profile.GradientStart, profile.GradientEnd,
                profile.Topmost, profile.Resizable, profile.Draggable);
        }
    }

    private void OnPanelPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        var point = e.GetCurrentPoint(this);

        if (point.Properties.IsRightButtonPressed)
        {
            var menu = new ContextMenu
            {
                Items =
                {
                    new MenuItem { Header = "Settings" },
                    new MenuItem { Header = "Manage Profiles" },
                    new MenuItem { Header = "Close" },
                }
            };

            ((MenuItem)menu.Items[0]!).Click += (_, _) => OpenSettings();
            ((MenuItem)menu.Items[1]!).Click += (_, _) => OpenProfiles();
            ((MenuItem)menu.Items[2]!).Click += (_, _) => Close();

            menu.Open(GradientPanel);
        }
        else if (point.Properties.IsLeftButtonPressed)
        {
            if (_isResizable)
            {
                var edge = GetResizeEdge(point.Position);
                if (edge != null)
                {
                    BeginResizeDrag(edge.Value, e);
                    return;
                }
            }

            if (_isDraggable)
            {
                BeginMoveDrag(e);
            }
        }
    }

    private void OnPanelPointerMoved(object? sender, PointerEventArgs e)
    {
        if (_isResizable)
        {
            var pos = e.GetPosition(this);
            var edge = GetResizeEdge(pos);
            Cursor = edge switch
            {
                WindowEdge.North or WindowEdge.South =>
                    new Cursor(StandardCursorType.SizeNorthSouth),
                WindowEdge.West or WindowEdge.East =>
                    new Cursor(StandardCursorType.SizeWestEast),
                WindowEdge.NorthWest => new Cursor(StandardCursorType.TopLeftCorner),
                WindowEdge.SouthEast => new Cursor(StandardCursorType.BottomRightCorner),
                WindowEdge.NorthEast => new Cursor(StandardCursorType.TopRightCorner),
                WindowEdge.SouthWest => new Cursor(StandardCursorType.BottomLeftCorner),
                _ => Cursor.Default
            };
        }
        else
        {
            Cursor = Cursor.Default;
        }
    }

    private WindowEdge? GetResizeEdge(Point position)
    {
        var w = Bounds.Width;
        var h = Bounds.Height;
        var top = position.Y < ResizeGripSize;
        var bottom = position.Y > h - ResizeGripSize;
        var left = position.X < ResizeGripSize;
        var right = position.X > w - ResizeGripSize;

        if (top && left) return WindowEdge.NorthWest;
        if (top && right) return WindowEdge.NorthEast;
        if (bottom && left) return WindowEdge.SouthWest;
        if (bottom && right) return WindowEdge.SouthEast;
        if (top) return WindowEdge.North;
        if (bottom) return WindowEdge.South;
        if (left) return WindowEdge.West;
        if (right) return WindowEdge.East;
        return null;
    }

    private void OpenSettings()
    {
        if (_settingsWindow is null || _settingsWindow.IsClosed)
        {
            _settingsWindow = new SettingsWindow(
                _gradientStart, _gradientEnd, Topmost, _isResizable, _isDraggable);
            _settingsWindow.GradientChanged += (start, end) =>
            {
                _gradientStart = start;
                _gradientEnd = end;
                UpdateGradient();
                _settings.ActiveProfile.GradientStart = start;
                _settings.ActiveProfile.GradientEnd = end;
            };
            _settingsWindow.TopmostChanged += topmost =>
            {
                Topmost = topmost;
                _settings.ActiveProfile.Topmost = topmost;
            };
            _settingsWindow.ResizableChanged += resizable =>
            {
                _isResizable = resizable;
                CanResize = resizable;
                UpdateDecorations();
                _settings.ActiveProfile.Resizable = resizable;
            };
            _settingsWindow.DraggableChanged += draggable =>
            {
                _isDraggable = draggable;
                UpdateDecorations();
                _settings.ActiveProfile.Draggable = draggable;
            };
            _settingsWindow.Show(this);
        }
        else
        {
            _settingsWindow.Activate();
        }
    }

    private void OpenProfiles()
    {
        if (_profilesWindow is null || _profilesWindow.IsClosed)
        {
            _profilesWindow = new ProfilesWindow(_settings.Profiles, _settings.ActiveProfileName);
            _profilesWindow.ProfileSwitched += name =>
            {
                _settings.ActiveProfileName = name;
                ApplyProfile(_settings.ActiveProfile);
            };
            _profilesWindow.Show(this);
        }
        else
        {
            _profilesWindow.Activate();
        }
    }

    private void UpdateDecorations()
    {
        if (_isDraggable)
            SystemDecorations = SystemDecorations.Full;
        else if (_isResizable)
            SystemDecorations = SystemDecorations.BorderOnly;
        else
            SystemDecorations = SystemDecorations.None;
    }

    private void UpdateGradient()
    {
        GradientPanel.Background = new ImmutableLinearGradientBrush(
            new[]
            {
                new ImmutableGradientStop(_gradientStart, Color.FromArgb(0, 0, 0, 0)),
                new ImmutableGradientStop(_gradientEnd, Colors.Black),
            },
            startPoint: new RelativePoint(0, 0, RelativeUnit.Relative),
            endPoint: new RelativePoint(0, 1, RelativeUnit.Relative));
    }
}
