using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BeanWorld.Input;

/// <summary>
/// Central input system. All game code checks input through this class
/// using GameAction values — never raw Keys or Buttons directly.
/// Call Update() once per frame before any screen/entity Update() calls.
/// </summary>
public class InputManager
{
    private InputState _state;
    private readonly InputBindings _bindings;

    public InputManager(InputBindings bindings)
    {
        _bindings = bindings;
    }

    public void Update()
    {
        _state = new InputState
        {
            PreviousKeyboard = _state.CurrentKeyboard,
            PreviousGamepad  = _state.CurrentGamepad,
            CurrentKeyboard  = Keyboard.GetState(),
            CurrentGamepad   = GamePad.GetState(PlayerIndex.One),
        };
    }

    /// <summary>True while the action's key/button is held down.</summary>
    public bool IsActionDown(GameAction action)
    {
        if (_bindings.Keyboard.TryGetValue(action, out var key) && _state.KeyDown(key))
            return true;
        if (_bindings.Gamepad.TryGetValue(action, out var button) && _state.ButtonDown(button))
            return true;
        return false;
    }

    /// <summary>True only on the frame the action's key/button was first pressed.</summary>
    public bool IsActionPressed(GameAction action)
    {
        if (_bindings.Keyboard.TryGetValue(action, out var key) && _state.KeyPressed(key))
            return true;
        if (_bindings.Gamepad.TryGetValue(action, out var button) && _state.ButtonPressed(button))
            return true;
        return false;
    }

    /// <summary>True only on the frame the action's key/button was released.</summary>
    public bool IsActionReleased(GameAction action)
    {
        if (_bindings.Keyboard.TryGetValue(action, out var key) && _state.KeyReleased(key))
            return true;
        if (_bindings.Gamepad.TryGetValue(action, out var button) && _state.ButtonReleased(button))
            return true;
        return false;
    }

    /// <summary>
    /// Returns a normalized movement vector from the four directional actions.
    /// Supports both keyboard (WASD/arrows) and gamepad left stick.
    /// </summary>
    public Vector2 GetMovementVector()
    {
        var direction = Vector2.Zero;

        if (IsActionDown(GameAction.MoveUp))    direction.Y -= 1f;
        if (IsActionDown(GameAction.MoveDown))  direction.Y += 1f;
        if (IsActionDown(GameAction.MoveLeft))  direction.X -= 1f;
        if (IsActionDown(GameAction.MoveRight)) direction.X += 1f;

        // Blend in gamepad left stick if connected
        var stick = _state.CurrentGamepad.ThumbSticks.Left;
        if (stick != Vector2.Zero)
        {
            direction.X += stick.X;
            direction.Y -= stick.Y; // MonoGame thumbstick Y is inverted vs screen Y
        }

        return direction == Vector2.Zero ? Vector2.Zero : Vector2.Normalize(direction);
    }
}
