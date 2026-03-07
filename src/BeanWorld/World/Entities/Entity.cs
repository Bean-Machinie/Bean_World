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

    // ── Health ───────────────────────────────────────────────────────────────
    public int MaxHealth { get; protected set; }
    public int Health { get; protected set; }

    /// <summary>
    /// When > 0 the entity cannot take damage. Ticked down in Update().
    /// Set this after a hit to create brief invincibility frames.
    /// </summary>
    protected float HitCooldown;

    private Vector2 _knockbackVelocity;
    private float _knockbackTimer;
    protected bool IsKnockedBack => _knockbackTimer > 0;

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

    /// <summary>
    /// Ticks HitCooldown down. Subclasses must call base.Update() to keep i-frames working.
    /// </summary>
    public virtual void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (HitCooldown > 0) HitCooldown -= dt;
        if (_knockbackTimer > 0)
        {
            ApplyKnockbackStep(_knockbackVelocity * dt);
            _knockbackTimer -= dt;
            if (_knockbackTimer <= 0) _knockbackVelocity = Vector2.Zero;
        }
    }

    /// <summary>
    /// Applies a single frame of knockback displacement.
    /// Override in subclasses to add collision checking.
    /// </summary>
    protected virtual void ApplyKnockbackStep(Vector2 delta) => Position += delta;

    public void ApplyKnockback(Vector2 direction, float force, float duration = 0.15f)
    {
        if (direction == Vector2.Zero) return;
        _knockbackVelocity = Vector2.Normalize(direction) * force;
        _knockbackTimer = duration;
    }

    public abstract void Draw(SpriteBatch spriteBatch);

    /// <summary>
    /// Reduces health by <paramref name="amount"/>. Ignored if HitCooldown > 0.
    /// Calls Destroy() when health reaches zero.
    /// </summary>
    public virtual void TakeDamage(int amount)
    {
        if (HitCooldown > 0 || !IsAlive) return;
        Health = Math.Max(0, Health - amount);
        if (Health <= 0) Destroy();
    }

    /// <summary>Sets MaxHealth and Health to <paramref name="maxHealth"/>.</summary>
    protected void InitHealth(int maxHealth)
    {
        MaxHealth = maxHealth;
        Health    = maxHealth;
    }

    /// <summary>Marks this entity for removal at the end of the current frame.</summary>
    protected void Destroy() => IsAlive = false;
}
