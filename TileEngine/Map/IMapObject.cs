using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TileEngine.Sprite;

namespace TileEngine.LayerMap
{
    public interface IMapObject : IGameObject
    {
        ISprite Sprite { get; }
        Map CurrentMap { get; set;}

        void HandleObjectCollision(IMapObject collidee);
        void HandleMapCollision();
        void Draw(SpriteBatch spriteBatch, Camera camera);
        void Update(GameTime gameTime);
    }
}
