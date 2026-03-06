using Microsoft.Xna.Framework.Content;

namespace BeanWorld.Assets;

/// <summary>
/// Typed wrapper around ContentManager with a simple in-memory cache.
/// Avoids double-loading the same asset and centralizes all content access.
/// Use the string constants in TextureAssets / FontAssets instead of raw strings.
/// </summary>
public class AssetManager
{
    private readonly ContentManager _content;
    private readonly Dictionary<string, object> _cache = new();

    public AssetManager(ContentManager content)
    {
        _content = content;
    }

    /// <summary>
    /// Loads an asset by name, returning the cached version if already loaded.
    /// Equivalent to ContentManager.Load&lt;T&gt;(assetName) but cached.
    /// </summary>
    public T Load<T>(string assetName)
    {
        if (_cache.TryGetValue(assetName, out var cached))
            return (T)cached;

        var asset = _content.Load<T>(assetName);
        _cache[assetName] = asset!;
        return asset;
    }

    /// <summary>Removes a single asset from the cache. The next Load call will re-load it.</summary>
    public void Unload(string assetName)
    {
        _cache.Remove(assetName);
    }

    /// <summary>Clears the cache and unloads all content from the ContentManager.</summary>
    public void UnloadAll()
    {
        _cache.Clear();
        _content.Unload();
    }
}
