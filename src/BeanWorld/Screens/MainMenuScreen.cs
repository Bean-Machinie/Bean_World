using BeanWorld.Assets;
using BeanWorld.Core.Screen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BeanWorld.Screens;

/// <summary>
/// The first screen shown when the game starts.
/// Currently a placeholder — replace with actual menu UI when ready.
/// </summary>
public class MainMenuScreen : Screen
{
    public MainMenuScreen(ScreenManager screenManager, AssetManager assets)
        : base(screenManager, assets) { }

    public override void LoadContent()
    {
        // TODO: load font — Assets.Load<SpriteFont>(FontAssets.Default)
    }

    public override void Update(GameTime gameTime, bool isTopScreen)
    {
        // TODO: handle menu navigation via InputManager
        // var input = ServiceLocator.Get<InputManager>();
        // if (input.IsActionPressed(GameAction.Confirm)) ScreenManager.Replace(new GameplayScreen(...));
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        spriteBatch.Begin();
        // TODO: draw title text and menu options once a font is loaded
        spriteBatch.End();
    }
}
