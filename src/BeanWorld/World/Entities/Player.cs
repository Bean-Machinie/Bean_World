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
    private Texture2D _texture = null!;

    public override Rectangle Bounds => new((int)Position.X, (int)Position.Y, Size, Size);

    public Player(Vector2 startPosition, InputManager input) : base(startPosition)
    {
        _input = input;
    }

    public override void LoadContent()
    {
        var graphicsDevice = ServiceLocator.Get<GraphicsDevice>();
        _texture = new Texture2D(graphicsDevice, 1, 1);
        _texture.SetData(new[] { Color.White });
    }

    public override void Update(GameTime gameTime)
    {
        var movement = _input.GetMovementVector();
        if (movement != Vector2.Zero)
            Position += movement * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_texture, Bounds, Color.Cyan);
    }
}
