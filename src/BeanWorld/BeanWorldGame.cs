using BeanWorld.Assets;
using BeanWorld.Core.Screen;
using BeanWorld.Core.Services;
using BeanWorld.Input;
using BeanWorld.Screens;
using BeanWorld.Settings;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BeanWorld;

/// <summary>
/// Main game class. Responsibilities:
///   - Create and wire up all core systems
///   - Register them with ServiceLocator
///   - Delegate Update/Draw to ScreenManager
///
/// No gameplay logic belongs here. This class should stay small.
/// </summary>
public class BeanWorldGame : Game
{
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch = null!;

    private SettingsManager _settingsManager = null!;
    private InputManager _inputManager = null!;
    private AssetManager _assetManager = null!;
    private ScreenManager _screenManager = null!;

    public BeanWorldGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // Settings must be loaded first so resolution is applied before the window is shown
        _settingsManager = new SettingsManager();
        _settingsManager.Load(shippedDefaultsPath: Path.Combine(Content.RootDirectory, "Data", "settings.json"));
        _settingsManager.ApplyToGraphics(_graphics);

        // Load input bindings (falls back to hardcoded defaults if file is missing)
        var bindingsPath = Path.Combine(Content.RootDirectory, "Data", "bindings.json");
        var bindings = File.Exists(bindingsPath)
            ? InputBindings.LoadFromJson(File.ReadAllText(bindingsPath))
            : InputBindings.CreateDefault();

        _inputManager  = new InputManager(bindings);
        _assetManager  = new AssetManager(Content);
        _screenManager = new ScreenManager();

        ServiceLocator.Register(_settingsManager);
        ServiceLocator.Register(_inputManager);
        ServiceLocator.Register(_assetManager);
        ServiceLocator.Register(_screenManager);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        ServiceLocator.Register(_spriteBatch);

        _screenManager.Push(new MainMenuScreen(_screenManager, _assetManager));
    }

    protected override void Update(GameTime gameTime)
    {
        _inputManager.Update();
        _screenManager.Update(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        _screenManager.Draw(gameTime, _spriteBatch);
        base.Draw(gameTime);
    }
}
