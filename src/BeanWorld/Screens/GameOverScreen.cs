using BeanWorld.Assets;
using BeanWorld.Core.Screen;
using BeanWorld.Core.Services;
using BeanWorld.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BeanWorld.Screens;

/// <summary>
/// Shown when the player dies. Press Enter to return to the main menu.
/// </summary>
public class GameOverScreen : Screen
{
    private SpriteFont _font = null!;
    private Texture2D _overlayTexture = null!;

    public GameOverScreen(ScreenManager screenManager, AssetManager assets)
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
        if (input.IsActionPressed(GameAction.Confirm))
            ScreenManager.Replace(new MainMenuScreen(ScreenManager, Assets));
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        var graphicsDevice = ServiceLocator.Get<GraphicsDevice>();
        var viewport = graphicsDevice.Viewport;

        spriteBatch.Begin();

        spriteBatch.Draw(_overlayTexture,
            new Rectangle(0, 0, viewport.Width, viewport.Height),
            Color.Black * 0.75f);

        var title = "YOU DIED";
        var titleSize = _font.MeasureString(title);
        var titlePos = new Vector2(
            (viewport.Width  - titleSize.X) * 0.5f,
            (viewport.Height - titleSize.Y) * 0.5f - 20);

        spriteBatch.DrawString(_font, title, titlePos + Vector2.One, Color.Black);
        spriteBatch.DrawString(_font, title, titlePos, Color.DarkRed);

        var hint = "Press Enter to return to menu";
        var hintSize = _font.MeasureString(hint);
        var hintPos = new Vector2(
            (viewport.Width  - hintSize.X) * 0.5f,
            titlePos.Y + titleSize.Y + 12);

        spriteBatch.DrawString(_font, hint, hintPos, Color.Gray);

        spriteBatch.End();
    }
}
