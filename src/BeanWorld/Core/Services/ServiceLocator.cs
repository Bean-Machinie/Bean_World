namespace BeanWorld.Core.Services;

/// <summary>
/// Lightweight type-keyed service registry. All core systems are registered here
/// during BeanWorldGame.Initialize() and can be retrieved from anywhere.
///
/// Rule: Use ServiceLocator for cross-cutting infrastructure only (AssetManager,
/// InputManager, SettingsManager, etc.). Gameplay objects should receive their
/// dependencies via constructor injection from their owning manager.
/// </summary>
public static class ServiceLocator
{
    private static readonly Dictionary<Type, object> _services = new();

    public static void Register<T>(T service) where T : class
    {
        _services[typeof(T)] = service;
    }

    public static T Get<T>() where T : class
    {
        if (_services.TryGetValue(typeof(T), out var service))
            return (T)service;

        throw new InvalidOperationException(
            $"Service '{typeof(T).Name}' has not been registered. " +
            $"Ensure it is registered in BeanWorldGame.Initialize().");
    }

    public static bool TryGet<T>(out T? service) where T : class
    {
        if (_services.TryGetValue(typeof(T), out var raw))
        {
            service = (T)raw;
            return true;
        }

        service = null;
        return false;
    }

    /// <summary>Clears all registered services. Intended for testing only.</summary>
    internal static void Reset() => _services.Clear();
}
