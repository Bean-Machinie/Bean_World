using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BeanWorld.World.Entities;

/// <summary>
/// Base class for all game entities (player, NPCs, items, etc.).
/// Extend this for each entity type. Keep gameplay logic in subclasses;
/// keep engine plumbing in EntityManager.
/// </summary>
public abstract class Entity
{
    public Vector2 Position { get; set; }
    public bool IsAlive { get; protected set; } = true;

    /// <summary>
    /// Axis-aligned bounding box used for collision and camera targeting.
    /// Override to return a meaningful bounds relative to Position.
    /// </summary>
    public virtual Rectangle Bounds => new((int)Position.X, (int)Position.Y, 0, 0);

    protected Entity(Vector2 position)
    {
        Position = position;
    }

    /// <summary>Called once after the entity is added to the EntityManager.</summary>
    public virtual void Initialize() { }

    /// <summary>Called once when content (textures, sounds) should be loaded.</summary>
    public virtual void LoadContent() { }

    public abstract void Update(GameTime gameTime);
    public abstract void Draw(SpriteBatch spriteBatch);

    /// <summary>Marks this entity for removal at the end of the current frame.</summary>
    protected void Destroy() => IsAlive = false;
}
