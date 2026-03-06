using BeanWorld.Assets;
using BeanWorld.Camera;
using BeanWorld.Core.Screen;
using BeanWorld.Core.Services;
using BeanWorld.Input;
using BeanWorld.Rendering;
using BeanWorld.World.Entities;
using BeanWorld.World.Tiles;
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
    private TileMap _tileMap = null!;
    private TileMapRenderer _tileMapRenderer = null!;

    public GameplayScreen(ScreenManager screenManager, AssetManager assets)
        : base(screenManager, assets) { }

    public override void LoadContent()
    {
        var graphicsDevice = ServiceLocator.Get<GraphicsDevice>();
        var input = ServiceLocator.Get<InputManager>();

        _camera = new Camera2D(graphicsDevice.Viewport);
        _entityManager = new EntityManager();

        // ── Tile registry ────────────────────────────────────────────────────
        var registry = new TileRegistry();
        registry.LoadFromJson(Path.Combine(AppContext.BaseDirectory, "Content", "Data", "tiles.json"));

        // ── Procedural tileset texture (32×16: grass left, stone right) ─────
        // Swap this block for Assets.Load<Texture2D>(TextureAssets.Tileset) when real art is ready.
        var tilesetTexture = new Texture2D(graphicsDevice, 32, 16);
        var pixels = new Color[32 * 16];
        var grassColor = new Color(34, 139, 34);
        var stoneColor = new Color(100, 100, 100);
        for (int y = 0; y < 16; y++)
        {
            for (int x = 0; x < 32; x++)
                pixels[y * 32 + x] = x < 16 ? grassColor : stoneColor;
        }
        tilesetTexture.SetData(pixels);

        _tileMapRenderer = new TileMapRenderer(registry, tilesetTexture);

        // ── Test map: 100×60 grass field with a stone border ────────────────
        // 100×60 tiles × 16px = 1600×960px — larger than the 1280×720 viewport so the camera scrolls.
        _tileMap = new TileMap(mapWidth: 100, mapHeight: 60, tileWidth: 16, tileHeight: 16);
        var ground = _tileMap.AddLayer("Ground");
        for (int y = 0; y < _tileMap.MapHeight; y++)
        {
            for (int x = 0; x < _tileMap.MapWidth; x++)
            {
                bool isBorder = x == 0 || y == 0 || x == _tileMap.MapWidth - 1 || y == _tileMap.MapHeight - 1;
                ground.SetTileId(x, y, isBorder ? 2 : 1); // 2=Stone, 1=Grass
            }
        }

        // ── Player spawns at map center ──────────────────────────────────────
        var spawnPos = new Vector2(_tileMap.MapWidth * _tileMap.TileWidth / 2f,
                                   _tileMap.MapHeight * _tileMap.TileHeight / 2f);
        _player = new Player(spawnPos, input);
        _entityManager.Add(_player);

        _camera.CenterOn(_player.Position);
    }

    public override void Update(GameTime gameTime, bool isTopScreen)
    {
        _entityManager.Update(gameTime);
        _camera.CenterOn(_player.Position);
        _camera.Clamp(_tileMap.Bounds);
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        spriteBatch.Begin(transformMatrix: _camera.GetTransform(), samplerState: SamplerState.PointClamp);

        _tileMapRenderer.Draw(spriteBatch, _tileMap, _camera);
        _entityManager.Draw(spriteBatch);
        // TODO: overlay layer draw here (Step 4)

        spriteBatch.End();
    }
}
