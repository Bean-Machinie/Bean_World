namespace BeanWorld.Settings;

/// <summary>
/// All user-configurable game settings. Loaded from JSON at startup.
/// Never hardcode these values elsewhere in the codebase — always read from here.
/// </summary>
public class GameSettings
{
    // Display
    public int ResolutionWidth { get; set; } = 1280;
    public int ResolutionHeight { get; set; } = 720;
    public bool IsFullscreen { get; set; } = false;
    public bool ShowFps { get; set; } = false;

    // Audio
    public float MasterVolume { get; set; } = 1.0f;
    public float MusicVolume { get; set; } = 0.8f;
    public float SfxVolume { get; set; } = 1.0f;

    // World
    public int TargetTileSize { get; set; } = 16; // base tile size in pixels
}
