using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BeanWorld.Camera;

/// <summary>
/// 2D camera that produces a transform Matrix for use with SpriteBatch.Begin().
/// Usage: spriteBatch.Begin(transformMatrix: camera.GetTransform())
/// </summary>
public class Camera2D
{
    private readonly Viewport _viewport;

    public Vector2 Position { get; set; }
    public float Zoom { get; set; } = 1f;
    public float Rotation { get; set; } = 0f;

    public Camera2D(Viewport viewport)
    {
        _viewport = viewport;
    }

    /// <summary>
    /// Returns the transform matrix to pass to SpriteBatch.Begin().
    /// Translates world space to screen space, applying zoom and rotation
    /// centered on the viewport.
    /// </summary>
    public Matrix GetTransform()
    {
        return
            Matrix.CreateTranslation(-Position.X, -Position.Y, 0f) *
            Matrix.CreateRotationZ(Rotation) *
            Matrix.CreateScale(Zoom, Zoom, 1f) *
            Matrix.CreateTranslation(_viewport.Width * 0.5f, _viewport.Height * 0.5f, 0f);
    }

    /// <summary>Converts a screen-space position to world-space coordinates.</summary>
    public Vector2 ScreenToWorld(Vector2 screenPosition)
    {
        return Vector2.Transform(screenPosition, Matrix.Invert(GetTransform()));
    }

    /// <summary>Converts a world-space position to screen-space coordinates.</summary>
    public Vector2 WorldToScreen(Vector2 worldPosition)
    {
        return Vector2.Transform(worldPosition, GetTransform());
    }

    /// <summary>Centers the camera on the given world position.</summary>
    public void CenterOn(Vector2 worldPosition)
    {
        Position = worldPosition;
    }

    /// <summary>
    /// Clamps the camera so it never shows empty space outside worldBounds.
    /// Call this after moving the camera each frame.
    /// </summary>
    public void Clamp(Rectangle worldBounds)
    {
        float halfW = _viewport.Width * 0.5f / Zoom;
        float halfH = _viewport.Height * 0.5f / Zoom;

        float minX = worldBounds.Left + halfW;
        float maxX = worldBounds.Right - halfW;
        float minY = worldBounds.Top + halfH;
        float maxY = worldBounds.Bottom - halfH;

        // If the world is smaller than the viewport, center it instead of clamping
        float x = maxX > minX ? Math.Clamp(Position.X, minX, maxX) : worldBounds.Center.X;
        float y = maxY > minY ? Math.Clamp(Position.Y, minY, maxY) : worldBounds.Center.Y;

        Position = new Vector2(x, y);
    }

    /// <summary>
    /// Returns the visible world rectangle at the current camera position and zoom.
    /// Useful for frustum culling (e.g., in TileMapRenderer).
    /// </summary>
    public Rectangle GetVisibleWorldBounds()
    {
        float halfW = _viewport.Width * 0.5f / Zoom;
        float halfH = _viewport.Height * 0.5f / Zoom;

        return new Rectangle(
            (int)(Position.X - halfW),
            (int)(Position.Y - halfH),
            (int)(_viewport.Width / Zoom),
            (int)(_viewport.Height / Zoom)
        );
    }
}
