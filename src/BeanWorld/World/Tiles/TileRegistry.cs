using System.Text.Json;

namespace BeanWorld.World.Tiles;

/// <summary>
/// Holds all tile definitions indexed by tile ID.
/// Loaded once at startup from tiles.json.
/// TileId 0 is reserved to mean "empty / no tile".
/// </summary>
public class TileRegistry
{
    private readonly Dictionary<int, TileDefinition> _tiles = new();

    public IReadOnlyDictionary<int, TileDefinition> All => _tiles;

    public void LoadFromJson(string jsonPath)
    {
        var json = File.ReadAllText(jsonPath);
        var definitions = JsonSerializer.Deserialize<List<TileDefinition>>(json)
                          ?? new List<TileDefinition>();

        _tiles.Clear();
        foreach (var def in definitions)
            _tiles[def.Id] = def;
    }

    public TileDefinition Get(int tileId)
    {
        if (_tiles.TryGetValue(tileId, out var def))
            return def;

        throw new KeyNotFoundException(
            $"Tile ID {tileId} is not registered. Check tiles.json.");
    }

    public bool TryGet(int tileId, out TileDefinition? definition) =>
        _tiles.TryGetValue(tileId, out definition);

    public bool IsDefined(int tileId) => _tiles.ContainsKey(tileId);
}
