using BeanWorld.Core.Services;
using BeanWorld.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BeanWorld.World.Entities;

public class Player : Entity
{
    private const float Speed = 120f; // pixels per second
    private const int Size = 16;      // matches default TargetTileSize

    private readonly InputManager _input;
    private readonly Func<Rectangle, bool> _isSolid;
    private Texture2D _texture = null!;

    public override Rectangle Bounds => new((int)Position.X, (int)Position.Y, Size, Size);

    public Player(Vector2 startPosition, InputManager input, Func<Rectangle, bool> isSolid)
        : base(startPosition)
    {
        _input = input;
        _isSolid = isSolid;
    }

    public override void LoadContent()
    {
        var graphicsDevice = ServiceLocator.Get<GraphicsDevice>();
        _texture = new Texture2D(graphicsDevice, 1, 1);
        _texture.SetData(new[] { Color.White });
    }

    public override void Update(GameTime gameTime)
    {
        var delta = _input.GetMovementVector() * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (delta == Vector2.Zero) return;

        // Axis-separated collision so the player can slide along walls
        var newX = Position with { X = Position.X + delta.X };
        if (!_isSolid(new Rectangle((int)newX.X, (int)newX.Y, Size, Size)))
            Position = newX;

        var newY = Position with { Y = Position.Y + delta.Y };
        if (!_isSolid(new Rectangle((int)newY.X, (int)newY.Y, Size, Size)))
            Position = newY;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_texture, Bounds, Color.Cyan);
    }
}
