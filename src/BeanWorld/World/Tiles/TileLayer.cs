namespace BeanWorld.World.Tiles;

/// <summary>
/// A single layer of tile IDs in a tile map.
/// TileIds[x, y] stores the tile ID at that grid position.
/// ID 0 means "empty" — no tile is drawn or checked for that cell.
/// </summary>
public class TileLayer
{
    public string Name { get; set; } = string.Empty;
    public int[,] TileIds { get; private set; }

    public int Width { get; }
    public int Height { get; }

    public bool IsVisible { get; set; } = true;

    /// <summary>
    /// Parallax scroll factor. 1.0 = moves with the world (normal).
    /// Values less than 1.0 scroll slower (background effect).
    /// </summary>
    public float Parallax { get; set; } = 1f;

    public TileLayer(string name, int width, int height)
    {
        Name = name;
        Width = width;
        Height = height;
        TileIds = new int[width, height];
    }

    public int GetTileId(int x, int y) => TileIds[x, y];

    public void SetTileId(int x, int y, int id) => TileIds[x, y] = id;

    public bool IsInBounds(int x, int y) =>
        x >= 0 && x < Width && y >= 0 && y < Height;
}
