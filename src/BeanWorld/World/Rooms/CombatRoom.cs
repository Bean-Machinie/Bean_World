using BeanWorld.World.Entities;
using BeanWorld.World.Tiles;
using Microsoft.Xna.Framework;

namespace BeanWorld.World.Rooms;

/// <summary>
/// One room unit in the run. Owns map data + enemy setup + runtime state.
/// </summary>
public class CombatRoom
{
    private readonly List<Enemy> _spawnedEnemies = new();

    public TileMap TileMap { get; }
    public Rectangle ExitTrigger { get; }
    public Vector2 PlayerSpawn { get; }
    public IReadOnlyList<Vector2> EnemySpawns { get; }

    public RoomState State { get; private set; } = RoomState.NotStarted;
    public IReadOnlyList<Enemy> SpawnedEnemies => _spawnedEnemies;

    public CombatRoom(TileMap tileMap, Rectangle exitTrigger, Vector2 playerSpawn, IEnumerable<Vector2> enemySpawns)
    {
        TileMap = tileMap;
        ExitTrigger = exitTrigger;
        PlayerSpawn = playerSpawn;
        EnemySpawns = enemySpawns.ToList();
    }

    public void Reset()
    {
        State = RoomState.NotStarted;
        _spawnedEnemies.Clear();
    }

    public void Activate(EntityManager entityManager, Func<Vector2, Enemy> enemyFactory)
    {
        if (State != RoomState.NotStarted)
            return;

        foreach (var spawn in EnemySpawns)
        {
            var enemy = enemyFactory(spawn);
            _spawnedEnemies.Add(enemy);
            entityManager.Add(enemy);
        }

        State = RoomState.Active;
    }

    public void RefreshState()
    {
        if (State == RoomState.Active && _spawnedEnemies.All(enemy => !enemy.IsAlive))
            State = RoomState.Cleared;
    }

    public bool CanTransition(Player player) =>
        State == RoomState.Cleared && player.Bounds.Intersects(ExitTrigger);
}
