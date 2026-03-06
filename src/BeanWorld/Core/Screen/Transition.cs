using Microsoft.Xna.Framework;

namespace BeanWorld.Core.Screen;

/// <summary>
/// Tracks the progress of a screen fade transition (in or out).
/// Alpha goes from 0 (fully visible) to 1 (fully obscured).
/// </summary>
public class Transition
{
    public float Alpha { get; private set; }
    public bool IsComplete => Alpha is <= 0f or >= 1f;

    private readonly float _direction; // -1 = fade in, +1 = fade out
    private readonly float _durationSeconds;

    public static Transition FadeIn(float durationSeconds) =>
        new(startAlpha: 1f, direction: -1f, durationSeconds);

    public static Transition FadeOut(float durationSeconds) =>
        new(startAlpha: 0f, direction: +1f, durationSeconds);

    private Transition(float startAlpha, float direction, float durationSeconds)
    {
        Alpha = startAlpha;
        _direction = direction;
        _durationSeconds = durationSeconds;
    }

    public void Update(GameTime gameTime)
    {
        if (IsComplete)
            return;

        float delta = (float)gameTime.ElapsedGameTime.TotalSeconds / _durationSeconds;
        Alpha = Math.Clamp(Alpha + _direction * delta, 0f, 1f);
    }
}
