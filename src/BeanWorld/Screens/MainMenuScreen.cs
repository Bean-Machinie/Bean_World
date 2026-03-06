using BeanWorld.Assets;
using BeanWorld.Core.Screen;
using BeanWorld.Core.Services;
using BeanWorld.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BeanWorld.Screens;

/// <summary>
/// The first screen shown when the game starts.
/// </summary>
public class MainMenuScreen : Screen
{
    private SpriteFont _font = null!;

    public MainMenuScreen(ScreenManager screenManager, AssetManager assets)
        : base(screenManager, assets) { }

    public override void LoadContent()
    {
        _font = Assets.Load<SpriteFont>(FontAssets.Default);
    }

    public override void Update(GameTime gameTime, bool isTopScreen)
    {
        var input = ServiceLocator.Get<InputManager>();
        if (input.IsActionPressed(GameAction.Confirm))
            ScreenManager.Replace(new GameplayScreen(ScreenManager, Assets));
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        spriteBatch.Begin();
        spriteBatch.DrawString(_font, "Bean World", new Vector2(100, 100), Color.White);
        spriteBatch.DrawString(_font, "Press Enter to start", new Vector2(100, 130), Color.Gray);
        spriteBatch.End();
    }
}
