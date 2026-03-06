using Microsoft.Xna.Framework;

namespace BeanWorld.World.Tiles;

/// <summary>
/// Represents a complete tile map: a collection of named layers over a fixed grid.
/// Contains no rendering logic — use TileMapRenderer to draw.
/// </summary>
public class TileMap
{
    public int TileWidth { get; }   // pixels per tile column
    public int TileHeight { get; }  // pixels per tile row
    public int MapWidth { get; }    // tiles wide
    public int MapHeight { get; }   // tiles tall

    public IReadOnlyList<TileLayer> Layers => _layers;
    private readonly List<TileLayer> _layers = new();

    /// <summary>The full pixel bounds of the map, starting at world origin (0, 0).</summary>
    public Rectangle Bounds => new(0, 0, MapWidth * TileWidth, MapHeight * TileHeight);

    public TileMap(int mapWidth, int mapHeight, int tileWidth, int tileHeight)
    {
        MapWidth = mapWidth;
        MapHeight = mapHeight;
        TileWidth = tileWidth;
        TileHeight = tileHeight;
    }

    // ── Layer management ────────────────────────────────────────────────────

    public TileLayer AddLayer(string name)
    {
        var layer = new TileLayer(name, MapWidth, MapHeight);
        _layers.Add(layer);
        return layer;
    }

    public TileLayer? GetLayer(string name) =>
        _layers.FirstOrDefault(l => l.Name == name);

    public TileLayer GetLayer(int index) => _layers[index];

    // ── Tile access helpers ──────────────────────────────────────────────────

    public int GetTileId(int layerIndex, int tileX, int tileY) =>
        _layers[layerIndex].GetTileId(tileX, tileY);

    public void SetTileId(int layerIndex, int tileX, int tileY, int id) =>
        _layers[layerIndex].SetTileId(tileX, tileY, id);

    // ── Coordinate conversion ────────────────────────────────────────────────

    /// <summary>Returns the top-left world position of tile (tileX, tileY).</summary>
    public Vector2 TileToWorld(int tileX, int tileY) =>
        new(tileX * TileWidth, tileY * TileHeight);

    /// <summary>Returns the tile grid coordinates that contain the given world position.</summary>
    public Point WorldToTile(Vector2 worldPosition) =>
        new(
            (int)Math.Floor(worldPosition.X / TileWidth),
            (int)Math.Floor(worldPosition.Y / TileHeight)
        );

    /// <summary>Returns true if the tile coordinates are within the map grid.</summary>
    public bool IsInBounds(int tileX, int tileY) =>
        tileX >= 0 && tileX < MapWidth && tileY >= 0 && tileY < MapHeight;
}
