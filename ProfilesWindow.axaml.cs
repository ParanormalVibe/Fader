using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;

namespace Fader;

public partial class ProfilesWindow : Window
{
    public event Action<string>? ProfileSwitched;
    public bool IsClosed { get; private set; }

    private readonly List<Profile> _profiles;
    private string _activeProfileName;
    private bool _suppressSelection;

    public ProfilesWindow() : this(new List<Profile> { new() }, "Default") { }

    public ProfilesWindow(List<Profile> profiles, string activeProfileName)
    {
        InitializeComponent();

        _profiles = profiles;
        _activeProfileName = activeProfileName;

        RefreshList();

        ProfileList.SelectionChanged += (_, _) =>
        {
            if (_suppressSelection) return;
            UpdateButtons();
        };

        SelectButton.Click += (_, _) =>
        {
            var selected = GetSelectedProfileName();
            if (selected == null || selected == _activeProfileName) return;
            _activeProfileName = selected;
            RefreshList();
            ProfileSwitched?.Invoke(selected);
        };

        AddButton.Click += async (_, _) =>
        {
            var dialog = new InputDialog("Profile name:", "New Profile");
            var name = await dialog.ShowDialog<string?>(this);
            if (string.IsNullOrWhiteSpace(name)) return;
            if (_profiles.Any(p => p.Name == name)) return;

            var active = _profiles.FirstOrDefault(p => p.Name == _activeProfileName);
            var newProfile = new Profile
            {
                Name = name,
                GradientStart = active?.GradientStart ?? 0.0,
                GradientEnd = active?.GradientEnd ?? 1.0,
                Topmost = active?.Topmost ?? true,
                Resizable = active?.Resizable ?? false,
                Draggable = active?.Draggable ?? false,
            };
            _profiles.Add(newProfile);
            _activeProfileName = name;
            RefreshList();
            ProfileSwitched?.Invoke(name);
        };

        RenameButton.Click += async (_, _) =>
        {
            var currentName = GetSelectedProfileName();
            if (currentName == null || currentName == "Default") return;
            var dialog = new InputDialog("Rename profile:", currentName);
            var newName = await dialog.ShowDialog<string?>(this);
            if (string.IsNullOrWhiteSpace(newName) || newName == currentName) return;
            if (_profiles.Any(p => p.Name == newName)) return;

            var profile = _profiles.FirstOrDefault(p => p.Name == currentName);
            if (profile == null) return;
            profile.Name = newName;
            if (_activeProfileName == currentName)
                _activeProfileName = newName;
            RefreshList();
            ProfileSwitched?.Invoke(_activeProfileName);
        };

        RemoveButton.Click += (_, _) =>
        {
            var name = GetSelectedProfileName();
            if (name == null || name == "Default") return;
            var profile = _profiles.FirstOrDefault(p => p.Name == name);
            if (profile == null) return;
            _profiles.Remove(profile);
            if (_activeProfileName == name)
            {
                _activeProfileName = "Default";
                ProfileSwitched?.Invoke("Default");
            }
            RefreshList();
        };

        Closed += (_, _) => IsClosed = true;
    }

    private void RefreshList()
    {
        _suppressSelection = true;
        ProfileList.ItemsSource = _profiles.Select(p =>
            (p.Name == _activeProfileName ? "\u2713 " : "   ") + p.Name).ToList();
        var idx = _profiles.FindIndex(p => p.Name == _activeProfileName);
        ProfileList.SelectedIndex = idx >= 0 ? idx : 0;
        _suppressSelection = false;
        UpdateButtons();
    }

    private string? GetSelectedProfileName()
    {
        if (ProfileList.SelectedItem is not string display) return null;
        return display.TrimStart('\u2713', ' ');
    }

    private void UpdateButtons()
    {
        var name = GetSelectedProfileName();
        var isDefault = name == "Default";
        SelectButton.IsEnabled = name != null && name != _activeProfileName;
        RenameButton.IsEnabled = name != null && !isDefault;
        RemoveButton.IsEnabled = name != null && !isDefault;
    }
}
