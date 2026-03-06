using BeanWorld.Core.Services;
using BeanWorld.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BeanWorld.World.Entities;

public class Player : Entity
{
    private const float Speed = 120f;
    private const int Size = 16;
    private const float AttackDuration = 0.15f; // seconds the attack visual shows
    private const float AttackCooldown = 0.4f;  // seconds between attacks

    private readonly InputManager _input;
    private readonly Func<Rectangle, bool> _isSolid;
    private Texture2D _texture = null!;

    private Vector2 _facing = Vector2.UnitX; // last non-zero movement direction
    private float _attackVisualTimer;
    private float _attackCooldownTimer;
    private Rectangle _attackVisualBounds; // persists for the visual duration

    public override Rectangle Bounds => new((int)Position.X, (int)Position.Y, Size, Size);

    /// <summary>
    /// The hitbox of the current attack swing. Non-null only on the frame Space is pressed
    /// (while off cooldown). GameplayScreen reads this to resolve hits.
    /// </summary>
    public Rectangle? AttackBounds { get; private set; }

    public Player(Vector2 startPosition, InputManager input, Func<Rectangle, bool> isSolid)
        : base(startPosition)
    {
        _input = input;
        _isSolid = isSolid;
        InitHealth(6);
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
        if (_attackCooldownTimer > 0) _attackCooldownTimer -= dt;
        if (_attackVisualTimer  > 0) _attackVisualTimer  -= dt;

        // Movement
        var moveVec = _input.GetMovementVector();
        if (moveVec != Vector2.Zero)
        {
            _facing = moveVec;
            var delta = moveVec * Speed * dt;

            var newX = Position with { X = Position.X + delta.X };
            if (!_isSolid(new Rectangle((int)newX.X, (int)newX.Y, Size, Size)))
                Position = newX;

            var newY = Position with { Y = Position.Y + delta.Y };
            if (!_isSolid(new Rectangle((int)newY.X, (int)newY.Y, Size, Size)))
                Position = newY;
        }

        // Attack — hitbox is only set on the exact frame the button is pressed
        if (_input.IsActionPressed(GameAction.Attack) && _attackCooldownTimer <= 0)
        {
            var offset = _facing * Size;
            AttackBounds = new Rectangle(
                (int)(Position.X + offset.X),
                (int)(Position.Y + offset.Y),
                Size, Size);
            _attackVisualBounds  = AttackBounds.Value;
            _attackVisualTimer   = AttackDuration;
            _attackCooldownTimer = AttackCooldown;
        }
        else
        {
            AttackBounds = null;
        }
    }

    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);
        if (IsAlive) HitCooldown = 1.0f; // 1-second i-frames after being hit
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        // Flash white during i-frames
        var color = HitCooldown > 0 ? Color.White : Color.Cyan;
        spriteBatch.Draw(_texture, Bounds, color);

        // Draw attack hitbox visual (persists for the full visual duration)
        if (_attackVisualTimer > 0)
            spriteBatch.Draw(_texture, _attackVisualBounds, Color.Yellow * 0.8f);
    }
}
