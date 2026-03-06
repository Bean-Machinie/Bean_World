using BeanWorld.Assets;
using BeanWorld.Core.Screen;
using BeanWorld.Input;
using BeanWorld.Core.Services;
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

    public PauseScreen(ScreenManager screenManager, AssetManager assets)
        : base(screenManager, assets) { }

    public override void LoadContent()
    {
        // TODO: load font — Assets.Load<SpriteFont>(FontAssets.Default)
    }

    public override void Update(GameTime gameTime, bool isTopScreen)
    {
        var input = ServiceLocator.Get<InputManager>();

        if (input.IsActionPressed(GameAction.Pause) || input.IsActionPressed(GameAction.Cancel))
            ScreenManager.Pop();
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        spriteBatch.Begin();
        // TODO: draw pause overlay once a font is loaded
        spriteBatch.End();
    }
}
