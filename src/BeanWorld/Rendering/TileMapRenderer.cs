using BeanWorld.Camera;
using BeanWorld.World.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BeanWorld.Rendering;

/// <summary>
/// Draws a TileMap layer by layer using the TileRegistry to resolve source rectangles.
/// Always performs viewport culling — only tiles visible within the camera's view are drawn.
///
/// Usage:
///   spriteBatch.Begin(transformMatrix: camera.GetTransform(), samplerState: SamplerState.PointClamp);
///   renderer.Draw(spriteBatch, map, camera);
///   spriteBatch.End();
/// </summary>
public class TileMapRenderer
{
    private readonly TileRegistry _registry;
    private readonly Texture2D _tilesetTexture;

    public TileMapRenderer(TileRegistry registry, Texture2D tilesetTexture)
    {
        _registry = registry;
        _tilesetTexture = tilesetTexture;
    }

    public void Draw(SpriteBatch spriteBatch, TileMap map, Camera2D camera)
    {
        var visibleRange = GetVisibleTileRange(map, camera);

        foreach (var layer in map.Layers)
        {
            if (!layer.IsVisible)
                continue;

            DrawLayer(spriteBatch, map, layer, visibleRange);
        }
    }

    private void DrawLayer(
        SpriteBatch spriteBatch,
        TileMap map,
        TileLayer layer,
        Rectangle visibleRange)
    {
        for (int x = visibleRange.Left; x < visibleRange.Right; x++)
        {
            for (int y = visibleRange.Top; y < visibleRange.Bottom; y++)
            {
                int tileId = layer.GetTileId(x, y);
                if (tileId == 0)
                    continue; // 0 = empty, nothing to draw

                if (!_registry.TryGet(tileId, out var def) || def is null)
                    continue;

                var worldPos = map.TileToWorld(x, y);
                var destRect = new Rectangle(
                    (int)worldPos.X,
                    (int)worldPos.Y,
                    map.TileWidth,
                    map.TileHeight);

                spriteBatch.Draw(
                    _tilesetTexture,
                    destRect,
                    def.SourceRect,
                    Color.White);
            }
        }
    }

    /// <summary>
    /// Returns the range of tile coordinates (in tile-space) that are currently
    /// visible within the camera's viewport. Clamped to the map bounds.
    /// </summary>
    private static Rectangle GetVisibleTileRange(TileMap map, Camera2D camera)
    {
        var worldBounds = camera.GetVisibleWorldBounds();

        int minX = Math.Max(0, worldBounds.Left / map.TileWidth);
        int minY = Math.Max(0, worldBounds.Top / map.TileHeight);

        // Add 1 to account for partially visible tiles at the edge
        int maxX = Math.Min(map.MapWidth,  (worldBounds.Right  / map.TileWidth)  + 1);
        int maxY = Math.Min(map.MapHeight, (worldBounds.Bottom / map.TileHeight) + 1);

        return new Rectangle(minX, minY, maxX - minX, maxY - minY);
    }
}
