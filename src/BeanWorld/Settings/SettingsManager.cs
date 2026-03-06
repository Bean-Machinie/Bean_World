using Microsoft.Xna.Framework;
using System.Text.Json;

namespace BeanWorld.Settings;

/// <summary>
/// Loads and saves GameSettings as JSON.
/// On first run (no user file exists), falls back to the shipped defaults in Content/Data/.
/// User overrides are stored in %AppData%\BeanWorld\ so they survive game updates.
/// </summary>
public class SettingsManager
{
    private static readonly string UserSavePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "BeanWorld",
        "settings.json");

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
    };

    public GameSettings Current { get; private set; } = new();

    /// <summary>
    /// Loads settings from the user save path.
    /// Falls back to the shipped defaults if no user file exists yet.
    /// </summary>
    public void Load(string? shippedDefaultsPath = null)
    {
        if (File.Exists(UserSavePath))
        {
            TryLoadFrom(UserSavePath);
            return;
        }

        if (shippedDefaultsPath is not null && File.Exists(shippedDefaultsPath))
        {
            TryLoadFrom(shippedDefaultsPath);
            return;
        }

        // No file found at all — use code defaults
        Current = new GameSettings();
    }

    /// <summary>Persists the current settings to the user save path.</summary>
    public void Save()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(UserSavePath)!);
        var json = JsonSerializer.Serialize(Current, SerializerOptions);
        File.WriteAllText(UserSavePath, json);
    }

    /// <summary>Applies resolution and fullscreen settings to the graphics device manager.</summary>
    public void ApplyToGraphics(GraphicsDeviceManager graphics)
    {
        graphics.PreferredBackBufferWidth = Current.ResolutionWidth;
        graphics.PreferredBackBufferHeight = Current.ResolutionHeight;
        graphics.IsFullScreen = Current.IsFullscreen;
        graphics.ApplyChanges();
    }

    private void TryLoadFrom(string path)
    {
        try
        {
            var json = File.ReadAllText(path);
            Current = JsonSerializer.Deserialize<GameSettings>(json, SerializerOptions)
                      ?? new GameSettings();
        }
        catch
        {
            Current = new GameSettings();
        }
    }
}
