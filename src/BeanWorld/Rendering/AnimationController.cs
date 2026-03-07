using Microsoft.Xna.Framework;

namespace BeanWorld.Rendering;

/// <summary>
/// Tracks the current animation row, frame, and timer for a sprite sheet.
/// Supports per-animation frame dimensions and FPS.
/// Changing the animKey resets the frame; changing only the row mid-animation
/// keeps the current frame index (so direction changes look smooth).
/// </summary>
public class AnimationController
{
    private int _animKey = -1;
    private int _row;
    private int _frameWidth;
    private int _frameHeight;
    private float _fps;
    private int _frameCount;
    private int _currentFrame;
    private float _timer;

    /// <summary>
    /// Switch to a new animation and direction.
    /// <paramref name="animKey"/> identifies the animation state — the frame only resets
    /// when this changes, so calling Play() every frame with the same key is safe.
    /// <paramref name="row"/> is the direction row and can change freely without resetting the frame.
    /// </summary>
    public void Play(int animKey, int row, int frameWidth, int frameHeight, float fps, int frameCount)
    {
        _row = row;
        if (_animKey == animKey) return;
        (_animKey, _frameWidth, _frameHeight, _fps, _frameCount, _currentFrame, _timer)
            = (animKey, frameWidth, frameHeight, fps, frameCount, 0, 0f);
    }

    /// <summary>Advance the frame timer. Call once per Update().</summary>
    public void Update(float dt)
    {
        if (_frameCount <= 1) return;
        _timer += dt;
        float frameDuration = 1f / _fps;
        while (_timer >= frameDuration)
        {
            _timer        -= frameDuration;
            _currentFrame  = (_currentFrame + 1) % _frameCount;
        }
    }

    /// <summary>Returns the source rectangle for the current frame on the sprite sheet.</summary>
    public Rectangle GetSourceRect() =>
        new(_currentFrame * _frameWidth, _row * _frameHeight, _frameWidth, _frameHeight);
}
