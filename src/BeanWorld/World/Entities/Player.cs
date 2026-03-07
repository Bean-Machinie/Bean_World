using BeanWorld.Core.Services;
using BeanWorld.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BeanWorld.World.Entities;

public class Player : Entity
{
    private const float Speed = 120f;
    private const int   Size  = 16;
    private const int   HitSize = 20;
    private const float HitPush = 26f;

    // ── Attack phase timings (seconds) ───────────────────────────────────────
    private const float StartupDuration  = 0.05f;
    private const float ActiveDuration   = 0.07f;
    private const float RecoveryDuration = 0.08f;
    private const float LungeSpeed       = 140f;   // px/s during startup + active

    private enum AttackPhase { None, Startup, Active, Recovery }

    private readonly InputManager _input;
    private readonly Func<Rectangle, bool> _isSolid;
    private Texture2D _texture = null!;

    private Vector2 _facing = Vector2.UnitX; // last movement/attack direction
    private AttackPhase _phase = AttackPhase.None;
    private float _phaseTimer;
    private Vector2 _attackDir;             // locked when attack starts
    private Rectangle _attackVisualBounds;

    public override Rectangle Bounds => new((int)Position.X, (int)Position.Y, Size, Size);

    /// <summary>Non-null only during the Active window. GameplayScreen reads this to resolve hits.</summary>
    public Rectangle? AttackBounds { get; private set; }

    public Player(Vector2 startPosition, InputManager input, Func<Rectangle, bool> isSolid)
        : base(startPosition)
    {
        _input   = input;
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
        base.Update(gameTime); // ticks HitCooldown + knockback

        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        TickPhase(dt);

        if (_phase == AttackPhase.None)
        {
            // Normal movement
            var moveVec = _input.GetMovementVector();
            if (moveVec != Vector2.Zero)
            {
                _facing = moveVec;
                MoveWithCollision(moveVec * Speed * dt);
            }

            // Start attack — lock direction to current input or last facing
            if (_input.IsActionPressed(GameAction.Attack))
            {
                var inputVec = _input.GetMovementVector();
                _attackDir  = inputVec != Vector2.Zero ? inputVec : _facing;
                _facing     = _attackDir;
                _phase      = AttackPhase.Startup;
                _phaseTimer = StartupDuration;
            }
        }
        else
        {
            // Attack overrides movement: lunge during startup + active, hold during recovery
            if (_phase is AttackPhase.Startup or AttackPhase.Active)
                MoveWithCollision(_attackDir * LungeSpeed * dt);
        }

        // Hitbox is only live during the active window
        if (_phase == AttackPhase.Active)
        {
            var center    = new Vector2(Position.X + Size / 2f, Position.Y + Size / 2f);
            var atkCenter = center + _attackDir * HitPush;
            AttackBounds = _attackVisualBounds = new Rectangle(
                (int)(atkCenter.X - HitSize / 2f),
                (int)(atkCenter.Y - HitSize / 2f),
                HitSize, HitSize);
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
        var color = HitCooldown > 0 ? Color.White : Color.Cyan;
        spriteBatch.Draw(_texture, Bounds, color);

        if (_phase == AttackPhase.Active)
            spriteBatch.Draw(_texture, _attackVisualBounds, Color.Yellow * 0.8f);
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private void TickPhase(float dt)
    {
        if (_phase == AttackPhase.None) return;
        _phaseTimer -= dt;
        if (_phaseTimer > 0) return;

        (_phase, _phaseTimer) = _phase switch
        {
            AttackPhase.Startup  => (AttackPhase.Active,   ActiveDuration),
            AttackPhase.Active   => (AttackPhase.Recovery, RecoveryDuration),
            _                    => (AttackPhase.None,      0f)
        };
    }

    private void MoveWithCollision(Vector2 delta)
    {
        var newX = Position with { X = Position.X + delta.X };
        if (!_isSolid(new Rectangle((int)newX.X, (int)newX.Y, Size, Size)))
            Position = newX;

        var newY = Position with { Y = Position.Y + delta.Y };
        if (!_isSolid(new Rectangle((int)newY.X, (int)newY.Y, Size, Size)))
            Position = newY;
    }

    protected override void ApplyKnockbackStep(Vector2 delta) => MoveWithCollision(delta);
}
