using Microsoft.Xna.Framework.Input;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BeanWorld.Input;

/// <summary>
/// Maps each GameAction to a keyboard key and an optional gamepad button.
/// Serialized to/from JSON so bindings can be user-configured.
/// </summary>
public class InputBindings
{
    public Dictionary<GameAction, Keys> Keyboard { get; set; } = new();
    public Dictionary<GameAction, Buttons> Gamepad { get; set; } = new();

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public static InputBindings CreateDefault() => new()
    {
        Keyboard = new Dictionary<GameAction, Keys>
        {
            [GameAction.MoveUp]        = Keys.W,
            [GameAction.MoveDown]      = Keys.S,
            [GameAction.MoveLeft]      = Keys.A,
            [GameAction.MoveRight]     = Keys.D,
            [GameAction.Confirm]       = Keys.Enter,
            [GameAction.Cancel]        = Keys.Escape,
            [GameAction.Interact]      = Keys.E,
            [GameAction.Pause]         = Keys.Escape,
            [GameAction.OpenInventory] = Keys.I,
            [GameAction.OpenMap]       = Keys.M,
        },
        Gamepad = new Dictionary<GameAction, Buttons>
        {
            [GameAction.MoveUp]        = Buttons.DPadUp,
            [GameAction.MoveDown]      = Buttons.DPadDown,
            [GameAction.MoveLeft]      = Buttons.DPadLeft,
            [GameAction.MoveRight]     = Buttons.DPadRight,
            [GameAction.Confirm]       = Buttons.A,
            [GameAction.Cancel]        = Buttons.B,
            [GameAction.Interact]      = Buttons.X,
            [GameAction.Pause]         = Buttons.Start,
            [GameAction.OpenInventory] = Buttons.Y,
            [GameAction.OpenMap]       = Buttons.Back,
        }
    };

    public static InputBindings LoadFromJson(string json)
    {
        return JsonSerializer.Deserialize<InputBindings>(json, SerializerOptions)
               ?? CreateDefault();
    }

    public string ToJson() => JsonSerializer.Serialize(this, SerializerOptions);
}
