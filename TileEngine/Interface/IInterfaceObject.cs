using Microsoft.Xna.Framework.Graphics;
using TileEngine.Sprite;

namespace TileEngine.Interface
{
    public interface IInterfaceObject : IGameObject
    {
        SimpleSprite Sprite { get; }
        
        void Draw(SpriteBatch spriteBatch);
    }
}
