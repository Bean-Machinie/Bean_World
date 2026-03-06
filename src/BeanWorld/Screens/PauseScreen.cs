using BeanWorld.Assets;
using BeanWorld.Core.Screen;
using BeanWorld.Core.Services;
using BeanWorld.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BeanWorld.Screens;

/// <summary>
/// Pause overlay pushed on top of GameplayScreen.
/// The gameplay screen still draws beneath it (DrawBelowThis = true),
/// but it does not update (UpdateBelowThis = false).
/// </summary>
public class PauseScreen : Screen
{
    public override bool DrawBelowThis => true;   // gameplay screen draws beneath
    public override bool UpdateBelowThis => false; // gameplay screen is frozen

    private SpriteFont _font = null!;
    private Texture2D _overlayTexture = null!;

    public PauseScreen(ScreenManager screenManager, AssetManager assets)
        : base(screenManager, assets) { }

    public override void LoadContent()
    {
        _font = Assets.Load<SpriteFont>(FontAssets.Default);

        var graphicsDevice = ServiceLocator.Get<GraphicsDevice>();
        _overlayTexture = new Texture2D(graphicsDevice, 1, 1);
        _overlayTexture.SetData(new[] { Color.White });
    }

    public override void Update(GameTime gameTime, bool isTopScreen)
    {
        var input = ServiceLocator.Get<InputManager>();
        if (input.IsActionPressed(GameAction.Pause) || input.IsActionPressed(GameAction.Cancel))
            ScreenManager.Pop();
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        var graphicsDevice = ServiceLocator.Get<GraphicsDevice>();
        var viewport = graphicsDevice.Viewport;

        spriteBatch.Begin();

        // Semi-transparent dark overlay covering the whole screen
        spriteBatch.Draw(_overlayTexture,
            new Rectangle(0, 0, viewport.Width, viewport.Height),
            Color.Black * 0.5f);

        // Centered "PAUSED" text
        var text = "PAUSED";
        var textSize = _font.MeasureString(text);
        var textPos = new Vector2(
            (viewport.Width  - textSize.X) * 0.5f,
            (viewport.Height - textSize.Y) * 0.5f);

        spriteBatch.DrawString(_font, text, textPos + Vector2.One, Color.Black); // shadow
        spriteBatch.DrawString(_font, text, textPos, Color.White);

        var hint = "Press Escape to resume";
        var hintSize = _font.MeasureString(hint);
        var hintPos = new Vector2(
            (viewport.Width  - hintSize.X) * 0.5f,
            textPos.Y + textSize.Y + 12);

        spriteBatch.DrawString(_font, hint, hintPos, Color.LightGray);

        spriteBatch.End();
    }
}
