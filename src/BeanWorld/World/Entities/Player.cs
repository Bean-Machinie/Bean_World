using BeanWorld.Assets;
using BeanWorld.Core.Services;
using BeanWorld.Input;
using BeanWorld.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BeanWorld.World.Entities;

public class Player : Entity
{
    private const float Speed = 180f;
    private const int   Size    = 24;  // collision hitbox
    private const int   HitSize = 28;  // attack hitbox
    private const float HitPush = 36f; // offset from player center to attack hitbox center

    // ── Attack phase timings (seconds) ───────────────────────────────────────
    private const float StartupDuration  = 0.05f;
    private const float ActiveDuration   = 0.07f;
    private const float RecoveryDuration = 0.08f;
    private const float LungeSpeed       = 200f;

    // ── Sprite sheet layout ───────────────────────────────────────────────────
    // Each animation is a separate PNG. All use 64×64 frames at ~6.67 fps (150 ms/frame).
    // 4 direction rows per sheet: 0=Down, 1=Left, 2=Right, 3=Up
    private const int   FrameW = 64;
    private const int   FrameH = 64;
    private const float AnimFps = 6.67f;

    private enum PlayerAnim { Idle, Walk, Attack, WalkAttack, Hurt, Death }
    private enum FacingDir  { Down = 0, Left = 1, Right = 2, Up = 3 }

    // Frames per direction for each animation (from sprite sheet widths ÷ 64)
    private static readonly int[] FrameCount = new int[(int)PlayerAnim.Death + 1];
    static Player()
    {
        FrameCount[(int)PlayerAnim.Idle]       = 12; // 768px / 64
        FrameCount[(int)PlayerAnim.Walk]       =  6; // 384px / 64
        FrameCount[(int)PlayerAnim.Attack]     =  8; // 512px / 64
        FrameCount[(int)PlayerAnim.WalkAttack] =  6; // 384px / 64
        FrameCount[(int)PlayerAnim.Hurt]       =  5; // 320px / 64
        FrameCount[(int)PlayerAnim.Death]      =  7; // 448px / 64
    }

    private static readonly string[] AssetKey =
    [
        TextureAssets.PlayerIdle,
        TextureAssets.PlayerWalk,
        TextureAssets.PlayerAttack,
        TextureAssets.PlayerWalkAttack,
        TextureAssets.PlayerHurt,
        TextureAssets.PlayerDeath,
    ];

    private enum AttackPhase { None, Startup, Active, Recovery }

    private readonly InputManager _input;
    private readonly AssetManager _assets;
    private readonly Func<Rectangle, bool> _isSolid;

    private readonly Texture2D?[] _textures = new Texture2D?[AssetKey.Length];
    private Texture2D  _pixel = null!;
    private readonly AnimationController _animator = new();

    private Vector2    _facing    = Vector2.UnitY; // default: facing down
    private FacingDir  _facingDir = FacingDir.Down;
    private bool       _isMoving;
    private AttackPhase _phase    = AttackPhase.None;
    private float      _phaseTimer;
    private Vector2    _attackDir;
    private Rectangle  _attackVisualBounds;
    private float      _hurtTimer;

    public override Rectangle Bounds => new((int)Position.X, (int)Position.Y, Size, Size);
    public Rectangle? AttackBounds { get; private set; }

    public Player(Vector2 startPosition, InputManager input, AssetManager assets, Func<Rectangle, bool> isSolid)
        : base(startPosition)
    {
        _input   = input;
        _assets  = assets;
        _isSolid = isSolid;
        InitHealth(6);
    }

    public override void LoadContent()
    {
        var gd = ServiceLocator.Get<GraphicsDevice>();
        _pixel = new Texture2D(gd, 1, 1);
        _pixel.SetData(new[] { Color.White });

        for (int i = 0; i < AssetKey.Length; i++)
        {
            try   { _textures[i] = _assets.Load<Texture2D>(AssetKey[i]); }
            catch { _textures[i] = null; }
        }
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime); // ticks HitCooldown + knockback

        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (_hurtTimer > 0) _hurtTimer -= dt;

        TickPhase(dt);

        if (_phase == AttackPhase.None)
        {
            var moveVec = _input.GetMovementVector();
            _isMoving = moveVec != Vector2.Zero;

            if (_isMoving)
            {
                _facing    = moveVec;
                _facingDir = ToFacingDir(moveVec);
                MoveWithCollision(moveVec * Speed * dt);
            }

            if (_input.IsActionPressed(GameAction.Attack))
            {
                var inputVec = _input.GetMovementVector();
                _attackDir  = inputVec != Vector2.Zero ? inputVec : _facing;
                _facing     = _attackDir;
                _facingDir  = ToFacingDir(_attackDir);
                _phase      = AttackPhase.Startup;
                _phaseTimer = StartupDuration;
                _isMoving   = true;
            }
        }
        else
        {
            _isMoving = _phase is AttackPhase.Startup or AttackPhase.Active;
            if (_isMoving)
                MoveWithCollision(_attackDir * LungeSpeed * dt);
        }

        // Attack hitbox — live only during active window
        if (_phase == AttackPhase.Active)
        {
            var center    = new Vector2(Position.X + Size / 2f, Position.Y + Size / 2f);
            var atkCenter = center + _attackDir * HitPush;
            AttackBounds  = _attackVisualBounds = new Rectangle(
                (int)(atkCenter.X - HitSize / 2f),
                (int)(atkCenter.Y - HitSize / 2f),
                HitSize, HitSize);
        }
        else
        {
            AttackBounds = null;
        }

        // Drive animation
        var anim  = SelectAnim();
        int row   = (int)_facingDir;
        int count = FrameCount[(int)anim];
        _animator.Play((int)anim, row, FrameW, FrameH, AnimFps, count);
        _animator.Update(dt);
    }

    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);
        if (IsAlive)
        {
            HitCooldown = 1.0f;
            _hurtTimer  = 0.35f;
        }
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        var anim   = SelectAnim();
        var color  = HitCooldown > 0 ? Color.White * 0.5f : Color.White;
        var center = new Vector2(Position.X + Size / 2f, Position.Y + Size / 2f);
        var tex    = _textures[(int)anim];

        if (tex is not null)
            SpriteRenderer.DrawFrameCentered(spriteBatch, tex, _animator.GetSourceRect(), center, color);
        else
            spriteBatch.Draw(_pixel, Bounds, HitCooldown > 0 ? Color.White : Color.Cyan);

        if (_phase == AttackPhase.Active)
            spriteBatch.Draw(_pixel, _attackVisualBounds, Color.Yellow * 0.8f);
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private PlayerAnim SelectAnim()
    {
        if (!IsAlive)       return PlayerAnim.Death;
        if (_hurtTimer > 0) return PlayerAnim.Hurt;
        if (_phase is AttackPhase.Startup or AttackPhase.Active)
            return _isMoving ? PlayerAnim.WalkAttack : PlayerAnim.Attack;
        return _isMoving ? PlayerAnim.Walk : PlayerAnim.Idle;
    }

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

    private static FacingDir ToFacingDir(Vector2 v)
    {
        if (MathF.Abs(v.Y) >= MathF.Abs(v.X))
            return v.Y > 0 ? FacingDir.Down : FacingDir.Up;
        return v.X < 0 ? FacingDir.Left : FacingDir.Right;
    }
}
