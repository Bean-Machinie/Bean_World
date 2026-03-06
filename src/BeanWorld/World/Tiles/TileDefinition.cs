using Microsoft.Xna.Framework;
using System.Text.Json.Serialization;

namespace BeanWorld.World.Tiles;

/// <summary>
/// Pure data definition of a tile type. No rendering logic here.
/// Loaded from tiles.json via TileRegistry at startup.
/// </summary>
public class TileDefinition
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // Source rectangle on the tileset texture (stored as components for clean JSON)
    public int SourceX { get; set; }
    public int SourceY { get; set; }
    public int SourceWidth { get; set; }
    public int SourceHeight { get; set; }

    [JsonIgnore]
    public Rectangle SourceRect => new(SourceX, SourceY, SourceWidth, SourceHeight);

    // Collision / behavior flags
    public bool IsSolid { get; set; }
    public bool IsWater { get; set; }
    public bool IsHazard { get; set; }

    // Animation (only relevant when IsAnimated is true)
    public bool IsAnimated { get; set; }
    public int AnimationFrameCount { get; set; } = 1;
    public float AnimationFrameDuration { get; set; } = 0.1f; // seconds per frame

    // For extensibility: arbitrary key-value properties defined in JSON
    public Dictionary<string, string> Properties { get; set; } = new();
}
