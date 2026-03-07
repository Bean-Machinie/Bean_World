using BeanWorld.Assets;
using BeanWorld.Camera;
using BeanWorld.Core.Screen;
using BeanWorld.Core.Services;
using BeanWorld.Input;
using BeanWorld.Rendering;
using BeanWorld.World.Entities;
using BeanWorld.World.Rooms;
using BeanWorld.World.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BeanWorld.Screens;

/// <summary>
/// The primary gameplay screen. One room/map is active at a time.
/// Clearing the room allows transition to the next room in the run.
/// </summary>
public class GameplayScreen : Screen
{
    private Camera2D _camera = null!;
    private EntityManager _entityManager = null!;
    private Player _player = null!;
    private SpriteFont _font = null!;
    private Texture2D _pixel = null!;
    private TileRegistry _registry = null!;
    private TileMapRenderer _tileMapRenderer = null!;

    private TileMap _tileMap = null!;
    private RoomRun _roomRun = null!;
    private bool _runWon;

    public GameplayScreen(ScreenManager screenManager, AssetManager assets)
        : base(screenManager, assets) { }

    public override void LoadContent()
    {
        var graphicsDevice = ServiceLocator.Get<GraphicsDevice>();
        var input = ServiceLocator.Get<InputManager>();

        _font = Assets.Load<SpriteFont>(FontAssets.Default);

        _pixel = new Texture2D(graphicsDevice, 1, 1);
        _pixel.SetData(new[] { Color.White });

        _camera = new Camera2D(graphicsDevice.Viewport);
        _entityManager = new EntityManager();

        _registry = new TileRegistry();
        _registry.LoadFromJson(Path.Combine(AppContext.BaseDirectory, "Content", "Data", "tiles.json"));

        var tilesetTexture = BuildDebugTileset(graphicsDevice);
        _tileMapRenderer = new TileMapRenderer(_registry, tilesetTexture);

        _roomRun = new RoomRun(BuildRooms());
        _roomRun.Start();

        _player = new Player(Vector2.Zero, input, Assets, rect => _tileMap.OverlapsSolid(rect, _registry));
        _entityManager.Add(_player);

        LoadCurrentRoom();
        _runWon = false;
    }

    public override void Update(GameTime gameTime, bool isTopScreen)
    {
        var input = ServiceLocator.Get<InputManager>();
        if (input.IsActionPressed(GameAction.Pause))
        {
            ScreenManager.Push(new PauseScreen(ScreenManager, Assets));
            return;
        }

        int healthBefore = _player.Health;
        _entityManager.Update(gameTime);

        if (_player.Health < healthBefore)
            _camera.Shake(0.45f);

        if (_player.AttackBounds is Rectangle attackBounds)
        {
            foreach (var enemy in _entityManager.Entities.OfType<Enemy>())
            {
                if (enemy.IsAlive && attackBounds.Intersects(enemy.Bounds))
                {
                    enemy.TakeDamage(1);
                    var knockDir = enemy.Position - _player.Position;
                    enemy.ApplyKnockback(knockDir, 180f, 0.15f);
                    _camera.Shake(0.15f);
                }
            }
        }

        if (!_player.IsAlive)
        {
            ScreenManager.Push(new GameOverScreen(ScreenManager, Assets));
            return;
        }

        int previousRoomIndex = _roomRun.CurrentRoomIndex;
        bool advanced = _roomRun.TryAdvance(_player);
        if (advanced)
        {
            if (_roomRun.IsCompleted && !_runWon)
            {
                _runWon = true;
                ScreenManager.Push(new WinScreen(ScreenManager, Assets));
                return;
            }

            if (_roomRun.CurrentRoomIndex != previousRoomIndex)
            {
                LoadCurrentRoom();
                return;
            }
        }

        _camera.CenterOn(_player.Position);
        _camera.Clamp(_tileMap.Bounds);
        _camera.UpdateShake(gameTime);
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        var room = _roomRun.CurrentRoom;
        var doorwayColor = room.State == RoomState.Cleared ? Color.Green : Color.DarkRed;

        spriteBatch.Begin(transformMatrix: _camera.GetTransform(), samplerState: SamplerState.PointClamp);

        _tileMapRenderer.Draw(spriteBatch, _tileMap, _camera);
        _entityManager.Draw(spriteBatch);
        spriteBatch.Draw(_pixel, room.ExitTrigger, doorwayColor * 0.35f);

        spriteBatch.End();

        spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        spriteBatch.DrawString(_font, $"HP: {_player.Health}/{_player.MaxHealth}", new Vector2(12, 10), Color.White);
        spriteBatch.DrawString(_font, $"Room {_roomRun.CurrentRoomIndex + 1}/{_roomRun.TotalRooms}: {room.State}", new Vector2(12, 30), Color.White);

        var hintColor = room.State == RoomState.Cleared ? Color.LightGreen : Color.Gray;
        var hint = room.State == RoomState.Cleared
            ? "Doorway open: move into green zone"
            : "Defeat all enemies to open doorway";
        spriteBatch.DrawString(_font, hint, new Vector2(12, 50), hintColor);

        spriteBatch.End();
    }

    private void LoadCurrentRoom()
    {
        var room = _roomRun.CurrentRoom;

        _tileMap = room.TileMap;
        _entityManager.RemoveAll(entity => entity is Enemy);

        _player.Position = room.PlayerSpawn;
        room.Activate(_entityManager, CreateEnemy);

        _camera.CenterOn(_player.Position);
        _camera.Clamp(_tileMap.Bounds);
    }

    private Enemy CreateEnemy(Vector2 position) =>
        new(position, _player, rect => _tileMap.OverlapsSolid(rect, _registry));

    private static Texture2D BuildDebugTileset(GraphicsDevice graphicsDevice)
    {
        // Two 32×32 tiles side by side: grass (left) and stone (right) → 64×32 texture
        var tilesetTexture = new Texture2D(graphicsDevice, 64, 32);
        var pixels = new Color[64 * 32];
        var grassColor = new Color(34, 139, 34);
        var stoneColor = new Color(100, 100, 100);

        for (int y = 0; y < 32; y++)
        {
            for (int x = 0; x < 64; x++)
                pixels[y * 64 + x] = x < 32 ? grassColor : stoneColor;
        }

        tilesetTexture.SetData(pixels);
        return tilesetTexture;
    }

    private List<CombatRoom> BuildRooms() =>
    [
        CreateRoom(
            mapWidth: 30,
            mapHeight: 20,
            playerSpawn: new Vector2(56, 304),
            enemySpawns:
            [
                new Vector2(480, 240),
                new Vector2(600, 360)
            ],
            extraSolidTiles:
            [
                new Point(15, 8),
                new Point(15, 9),
                new Point(15, 10)
            ]),
        CreateRoom(
            mapWidth: 28,
            mapHeight: 18,
            playerSpawn: new Vector2(56, 272),
            enemySpawns:
            [
                new Vector2(420, 180),
                new Vector2(500, 260),
                new Vector2(440, 340)
            ],
            extraSolidTiles:
            [
                new Point(10, 6),
                new Point(17, 11)
            ]),
        CreateRoom(
            mapWidth: 32,
            mapHeight: 20,
            playerSpawn: new Vector2(56, 304),
            enemySpawns:
            [
                new Vector2(520, 190),
                new Vector2(640, 300),
                new Vector2(520, 410)
            ],
            extraSolidTiles:
            [
                new Point(13, 7),
                new Point(13, 12),
                new Point(19, 7),
                new Point(19, 12)
            ])
    ];

    private static CombatRoom CreateRoom(
        int mapWidth,
        int mapHeight,
        Vector2 playerSpawn,
        IEnumerable<Vector2> enemySpawns,
        IEnumerable<Point> extraSolidTiles)
    {
        var tileMap = new TileMap(mapWidth, mapHeight, tileWidth: 32, tileHeight: 32);
        var ground = tileMap.AddLayer("Ground");

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                bool isBorder = x == 0 || y == 0 || x == mapWidth - 1 || y == mapHeight - 1;
                ground.SetTileId(x, y, isBorder ? 2 : 1); // 2=Stone (solid), 1=Grass
            }
        }

        foreach (var tile in extraSolidTiles)
            ground.SetTileId(tile.X, tile.Y, 2);

        var exitTrigger = new Rectangle(tileMap.Bounds.Right - 96, tileMap.Bounds.Center.Y - 80, 48, 160);
        return new CombatRoom(tileMap, exitTrigger, playerSpawn, enemySpawns);
    }
}
