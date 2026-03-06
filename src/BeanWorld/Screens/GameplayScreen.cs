using BeanWorld.Assets;
using BeanWorld.Core.Screen;
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
    private EntityManager _entityManager = null!;

    public GameplayScreen(ScreenManager screenManager, AssetManager assets)
        : base(screenManager, assets) { }

    public override void LoadContent()
    {
        _entityManager = new EntityManager();

        // TODO: load TileRegistry from Content/Data/tiles.json
        // TODO: construct TileMap for the starting area
        // TODO: create Camera2D(GraphicsDevice.Viewport) and CenterOn map center
        // TODO: create and add player entity via _entityManager.Add(...)
    }

    public override void Update(GameTime gameTime, bool isTopScreen)
    {
        _entityManager.Update(gameTime);

        // TODO: camera.CenterOn(player.Position)
        // TODO: camera.Clamp(tileMap.Bounds)
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        // TODO: spriteBatch.Begin(transformMatrix: camera.GetTransform(), samplerState: SamplerState.PointClamp)
        spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        // TODO: tileMapRenderer.Draw(spriteBatch, tileMap, camera)  -- ground layers
        _entityManager.Draw(spriteBatch);
        // TODO: tileMapRenderer.Draw(spriteBatch, tileMap, camera)  -- overlay layers

        spriteBatch.End();
    }
}
