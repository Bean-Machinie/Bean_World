using BeanWorld.Core.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BeanWorld.World.Entities;

public class Enemy : Entity
{
    private const float Speed       = 55f;
    private const float AgroRange   = 150f;
    private const float ContactCooldown = 1.0f; // seconds between contact damage ticks

    private readonly Player _player;
    private readonly Func<Rectangle, bool> _isSolid;
    private Texture2D _texture = null!;
    private float _contactTimer;
    private float _hitFlashTimer;

    public override Rectangle Bounds => new((int)Position.X, (int)Position.Y, 16, 16);

    public Enemy(Vector2 position, Player player, Func<Rectangle, bool> isSolid)
        : base(position)
    {
        _player  = player;
        _isSolid = isSolid;
        InitHealth(3);
    }

    public override void LoadContent()
    {
        var graphicsDevice = ServiceLocator.Get<GraphicsDevice>();
        _texture = new Texture2D(graphicsDevice, 1, 1);
        _texture.SetData(new[] { Color.White });
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime); // ticks HitCooldown

        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (_contactTimer > 0) _contactTimer -= dt;
        if (_hitFlashTimer > 0) _hitFlashTimer -= dt;

        // Move toward player when in agro range (skip if knocked back)
        var toPlayer = _player.Position - Position;
        if (!IsKnockedBack && toPlayer.LengthSquared() < AgroRange * AgroRange && toPlayer != Vector2.Zero)
        {
            var step = Vector2.Normalize(toPlayer) * Speed * dt;

            var newX = Position with { X = Position.X + step.X };
            if (!_isSolid(new Rectangle((int)newX.X, (int)newX.Y, 16, 16)))
                Position = newX;

            var newY = Position with { Y = Position.Y + step.Y };
            if (!_isSolid(new Rectangle((int)newY.X, (int)newY.Y, 16, 16)))
                Position = newY;
        }

        // Contact damage to player
        if (_contactTimer <= 0 && Bounds.Intersects(_player.Bounds))
        {
            _player.TakeDamage(1);
            var knockDir = _player.Position - Position;
            _player.ApplyKnockback(knockDir, 150f, 0.15f);
            _contactTimer = ContactCooldown;
        }
    }

    protected override void ApplyKnockbackStep(Vector2 delta)
    {
        var newX = Position with { X = Position.X + delta.X };
        if (!_isSolid(new Rectangle((int)newX.X, (int)newX.Y, 16, 16)))
            Position = newX;

        var newY = Position with { Y = Position.Y + delta.Y };
        if (!_isSolid(new Rectangle((int)newY.X, (int)newY.Y, 16, 16)))
            Position = newY;
    }

    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);
        if (IsAlive)
        {
            HitCooldown = 0.2f;
            _hitFlashTimer = 0.08f; // short bright flash, separate from i-frames
        }
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        var color = _hitFlashTimer > 0 ? Color.White : Color.Red;
        spriteBatch.Draw(_texture, Bounds, color);
    }
}
