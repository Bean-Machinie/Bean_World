using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BeanWorld.Rendering;

/// <summary>
/// Thin helpers for common sprite drawing patterns.
/// All methods assume SpriteBatch.Begin() has already been called.
/// </summary>
public static class SpriteRenderer
{
    /// <summary>Draws a texture centered on the given world position.</summary>
    public static void DrawCentered(
        SpriteBatch spriteBatch,
        Texture2D texture,
        Vector2 worldPosition,
        Color color,
        float scale = 1f,
        float rotation = 0f,
        float layerDepth = 0f)
    {
        var origin = new Vector2(texture.Width * 0.5f, texture.Height * 0.5f);
        spriteBatch.Draw(texture, worldPosition, null, color, rotation, origin, scale, SpriteEffects.None, layerDepth);
    }

    /// <summary>Draws a region of a texture (sprite sheet frame) centered on the given position.</summary>
    public static void DrawFrameCentered(
        SpriteBatch spriteBatch,
        Texture2D texture,
        Rectangle sourceRect,
        Vector2 worldPosition,
        Color color,
        float scale = 1f,
        float rotation = 0f,
        SpriteEffects effects = SpriteEffects.None,
        float layerDepth = 0f)
    {
        var origin = new Vector2(sourceRect.Width * 0.5f, sourceRect.Height * 0.5f);
        spriteBatch.Draw(texture, worldPosition, sourceRect, color, rotation, origin, scale, effects, layerDepth);
    }
}
