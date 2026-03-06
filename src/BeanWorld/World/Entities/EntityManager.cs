using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BeanWorld.World.Entities;

/// <summary>
/// Manages the lifecycle and update/draw order of all active entities.
/// Dead entities (IsAlive == false) are purged at the end of each Update().
/// </summary>
public class EntityManager
{
    private readonly List<Entity> _entities = new();
    private readonly List<Entity> _pendingAdd = new();

    public IReadOnlyList<Entity> Entities => _entities;

    public void Add(Entity entity)
    {
        entity.Initialize();
        entity.LoadContent();
        _pendingAdd.Add(entity);
    }

    public void Update(GameTime gameTime)
    {
        // Flush pending additions before updating so newly-added
        // entities don't update on their first frame
        _entities.AddRange(_pendingAdd);
        _pendingAdd.Clear();

        foreach (var entity in _entities)
        {
            if (entity.IsAlive)
                entity.Update(gameTime);
        }

        // Remove dead entities
        _entities.RemoveAll(e => !e.IsAlive);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        // Draw all entities. For Y-sorted depth, sort by Position.Y + Bounds.Bottom.
        foreach (var entity in _entities)
        {
            if (entity.IsAlive)
                entity.Draw(spriteBatch);
        }
    }

    public void Clear()
    {
        _entities.Clear();
        _pendingAdd.Clear();
    }

    public void RemoveAll(Predicate<Entity> match)
    {
        _entities.RemoveAll(match);
        _pendingAdd.RemoveAll(match);
    }
}
