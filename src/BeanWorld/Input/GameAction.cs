namespace BeanWorld.Input;

/// <summary>
/// All bindable game actions. No code outside InputManager should ever check
/// raw keyboard/gamepad state — always use these actions instead.
/// Add new actions here as gameplay features are added.
/// </summary>
public enum GameAction
{
    // Movement
    MoveUp,
    MoveDown,
    MoveLeft,
    MoveRight,

    // Interaction
    Confirm,
    Cancel,
    Interact,
    Attack,

    // Meta
    Pause,
    OpenInventory,
    OpenMap,

#if DEBUG
    // Debug actions (stripped in Release builds)
    DebugToggleCollision,
    DebugReloadMap,
    DebugToggleFps,
#endif
}
