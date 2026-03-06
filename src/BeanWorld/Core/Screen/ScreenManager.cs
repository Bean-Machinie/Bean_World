using BeanWorld.Core.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BeanWorld.Core.Screen;

/// <summary>
/// Manages a stack of game screens (pushdown automaton).
/// Push/Pop/Replace operations are deferred to the start of the next frame
/// to prevent mid-update collection modification.
/// </summary>
public class ScreenManager
{
    private readonly Stack<IScreen> _screens = new();
    private readonly List<Action> _pendingOperations = new();

    public IScreen? Current => _screens.Count > 0 ? _screens.Peek() : null;

    public void Push(IScreen screen)
    {
        _pendingOperations.Add(() =>
        {
            screen.Initialize();
            screen.LoadContent();
            _screens.Push(screen);
        });
    }

    public void Pop()
    {
        _pendingOperations.Add(() =>
        {
            if (_screens.Count > 0)
            {
                _screens.Pop().UnloadContent();
            }
        });
    }

    public void Replace(IScreen screen)
    {
        _pendingOperations.Add(() =>
        {
            if (_screens.Count > 0)
                _screens.Pop().UnloadContent();

            screen.Initialize();
            screen.LoadContent();
            _screens.Push(screen);
        });
    }

    public void Update(GameTime gameTime)
    {
        FlushPendingOperations();

        // Walk the stack: update from top down, stopping when UpdateBelowThis is false
        var screensToUpdate = GetScreensToProcess(s => s.UpdateBelowThis);
        foreach (var screen in screensToUpdate)
            screen.Update(gameTime, screen == _screens.Peek());
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        // Walk the stack: draw from bottom up (so top screen draws last / on top)
        var screensToDraw = GetScreensToProcess(s => s.DrawBelowThis);
        for (int i = screensToDraw.Count - 1; i >= 0; i--)
            screensToDraw[i].Draw(gameTime, spriteBatch);
    }

    private void FlushPendingOperations()
    {
        if (_pendingOperations.Count == 0)
            return;

        foreach (var operation in _pendingOperations)
            operation();

        _pendingOperations.Clear();
    }

    /// <summary>
    /// Builds an ordered list of screens starting at the top of the stack,
    /// continuing downward as long as the predicate is true for each screen.
    /// Always includes at least the top screen.
    /// </summary>
    private List<IScreen> GetScreensToProcess(Func<IScreen, bool> continueBelow)
    {
        var result = new List<IScreen>();
        foreach (var screen in _screens)
        {
            result.Add(screen);
            if (!continueBelow(screen))
                break;
        }
        return result;
    }
}
