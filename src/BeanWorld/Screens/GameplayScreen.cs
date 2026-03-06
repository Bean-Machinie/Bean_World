using BeanWorld.Assets;
using BeanWorld.Camera;
using BeanWorld.Core.Screen;
using BeanWorld.Core.Services;
using BeanWorld.Input;
using BeanWorld.World.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BeanWorld.Screens;

/// <summary>
/// The primary gameplay screen. Owns the Camera2D, EntityManager, and TileMap for the current area.
/// The bulk of game logic will live here and in the entity/tile systems.
/// </summary>
public class GameplayScreen : Screen
{
    private Camera2D _camera = null!;
    private EntityManager _entityManager = null!;
    private Player _player = null!;

    public GameplayScreen(ScreenManager screenManager, AssetManager assets)
        : base(screenManager, assets) { }

    public override void LoadContent()
    {
        var graphicsDevice = ServiceLocator.Get<GraphicsDevice>();
        var input = ServiceLocator.Get<InputManager>();

        _camera = new Camera2D(graphicsDevice.Viewport);
        _entityManager = new EntityManager();

        _player = new Player(new Vector2(100, 100), input);
        _entityManager.Add(_player);

        _camera.CenterOn(_player.Position);

        // TODO: load TileRegistry from Content/Data/tiles.json
        // TODO: construct TileMap for the starting area
    }

    public override void Update(GameTime gameTime, bool isTopScreen)
    {
        _entityManager.Update(gameTime);
        _camera.CenterOn(_player.Position);

        // TODO: _camera.Clamp(tileMap.Bounds)
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        spriteBatch.Begin(transformMatrix: _camera.GetTransform(), samplerState: SamplerState.PointClamp);

        // TODO: tileMapRenderer.Draw(spriteBatch, tileMap, camera)  -- ground layers
        _entityManager.Draw(spriteBatch);
        // TODO: tileMapRenderer.Draw(spriteBatch, tileMap, camera)  -- overlay layers

        spriteBatch.End();
    }
}
