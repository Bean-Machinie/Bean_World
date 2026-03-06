using Microsoft.Xna.Framework.Input;

namespace BeanWorld.Input;

/// <summary>
/// Immutable snapshot of input hardware state for one frame.
/// Holding both current and previous states allows edge detection
/// (just pressed / just released) without heap allocation.
/// </summary>
public readonly struct InputState
{
    public KeyboardState CurrentKeyboard { get; init; }
    public KeyboardState PreviousKeyboard { get; init; }
    public GamePadState CurrentGamepad { get; init; }
    public GamePadState PreviousGamepad { get; init; }

    public bool KeyDown(Keys key) => CurrentKeyboard.IsKeyDown(key);
    public bool KeyPressed(Keys key) => CurrentKeyboard.IsKeyDown(key) && PreviousKeyboard.IsKeyUp(key);
    public bool KeyReleased(Keys key) => CurrentKeyboard.IsKeyUp(key) && PreviousKeyboard.IsKeyDown(key);

    public bool ButtonDown(Buttons button) => CurrentGamepad.IsButtonDown(button);
    public bool ButtonPressed(Buttons button) => CurrentGamepad.IsButtonDown(button) && PreviousGamepad.IsButtonUp(button);
    public bool ButtonReleased(Buttons button) => CurrentGamepad.IsButtonUp(button) && PreviousGamepad.IsButtonDown(button);
}
