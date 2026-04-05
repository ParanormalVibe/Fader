using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fader;

public class AppSettings
{
    public double WindowX { get; set; } = double.NaN;
    public double WindowY { get; set; } = double.NaN;
    public double WindowWidth { get; set; } = double.NaN;
    public double WindowHeight { get; set; } = double.NaN;
    public string ActiveProfileName { get; set; } = "Default";
    public List<Profile> Profiles { get; set; } = new() { new Profile() };

    [JsonIgnore]
    public Profile ActiveProfile =>
        Profiles.FirstOrDefault(p => p.Name == ActiveProfileName) ?? Profiles[0];

    private static string SettingsPath => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "Fader", "settings.json");

    public static AppSettings Load()
    {
        try
        {
            if (File.Exists(SettingsPath))
            {
                var json = File.ReadAllText(SettingsPath);
                var settings = JsonSerializer.Deserialize<AppSettings>(json) ?? new();
                if (settings.Profiles == null || settings.Profiles.Count == 0)
                    settings.Profiles = new() { new Profile() };
                return settings;
            }
        }
        catch { }
        return new();
    }

    public void Save()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath)!);
            File.WriteAllText(SettingsPath, JsonSerializer.Serialize(this));
        }
        catch { }
    }
}
