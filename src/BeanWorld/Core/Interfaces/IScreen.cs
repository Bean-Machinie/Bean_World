using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BeanWorld.Core.Interfaces;

public interface IScreen
{
    /// <summary>When true, the screen below this one in the stack is also drawn.</summary>
    bool DrawBelowThis { get; }

    /// <summary>When true, the screen below this one in the stack is also updated.</summary>
    bool UpdateBelowThis { get; }

    void Initialize();
    void LoadContent();
    void UnloadContent();
    void Update(GameTime gameTime, bool isTopScreen);
    void Draw(GameTime gameTime, SpriteBatch spriteBatch);
}
