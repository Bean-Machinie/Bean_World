using BeanWorld.Assets;
using BeanWorld.Core.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BeanWorld.Core.Screen;

/// <summary>
/// Abstract base class for all game screens. Subclasses override the lifecycle
/// methods they need; only LoadContent and Draw are mandatory.
/// </summary>
public abstract class Screen : IScreen
{
    public virtual bool DrawBelowThis => false;
    public virtual bool UpdateBelowThis => false;

    protected ScreenManager ScreenManager { get; }
    protected AssetManager Assets { get; }

    protected Screen(ScreenManager screenManager, AssetManager assets)
    {
        ScreenManager = screenManager;
        Assets = assets;
    }

    public virtual void Initialize() { }
    public abstract void LoadContent();
    public virtual void UnloadContent() { }
    public virtual void Update(GameTime gameTime, bool isTopScreen) { }
    public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);
}
