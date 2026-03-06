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

        // Move toward player when in agro range
        var toPlayer = _player.Position - Position;
        if (toPlayer.LengthSquared() < AgroRange * AgroRange && toPlayer != Vector2.Zero)
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
            _contactTimer = ContactCooldown;
        }
    }

    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);
        if (IsAlive) HitCooldown = 0.2f; // brief flash on hit
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        // Flash white briefly when hit, otherwise red
        var color = HitCooldown > 0 ? Color.White : Color.Red;
        spriteBatch.Draw(_texture, Bounds, color);
    }
}
