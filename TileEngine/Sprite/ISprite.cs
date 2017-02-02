using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Penumbra;

namespace TileEngine.Sprite
{
    public enum SpriteOrigin : byte
    {
        CustomOrigin,
        SpritePosition,
        SpriteCenter
    }
    public interface ISprite
    {
        Vector2 Center { get; }
        Vector2 Position { get; set; }
        float Alpha { get; set; }
        float Angle { get; set; }
        float Scale { get; set; }
        float PositionX { get; set; }
        float PositionY { get; set; }
        Vector2 Origin { get; }
        Texture2D Texture { get; set; }
        Rectangle Bounds { get; }
        float CollisionRange { get; set; }
        float CombatRange { get; set; }
        float InteractionRange { get; set; }
        float DetectionRange { get; set; }
        bool InLightingRange(Light light, SpriteOrigin so = SpriteOrigin.SpriteCenter);
        bool InShadowRange(Hull hull, SpriteOrigin so = SpriteOrigin.SpriteCenter);
        bool InUpdateRange(ISprite updateObject, SpriteOrigin so = SpriteOrigin.SpriteCenter);
        bool InDrawRange(ISprite drawObject, SpriteOrigin so = SpriteOrigin.SpriteCenter);
        bool IsColliding(ISprite collisionObject, SpriteOrigin so = SpriteOrigin.SpriteCenter);
        bool IsColliding(Vector2 vector, SpriteOrigin so = SpriteOrigin.SpriteCenter);
        bool InDectectionRange(ISprite detectableObject, SpriteOrigin so = SpriteOrigin.SpriteCenter);
        bool InInteractionRange(ISprite interactableObject, SpriteOrigin so = SpriteOrigin.SpriteCenter);
        bool InCombatRange(ISprite target, SpriteOrigin so = SpriteOrigin.SpriteCenter);
        float Distance(ISprite toSprite, SpriteOrigin so = SpriteOrigin.SpriteCenter);
        void Draw(SpriteBatch spriteBatch);
        void Draw(SpriteBatch spriteBatch, SpriteEffects spriteEffect);
        void ClampToArea(int width, int height);
    }
}
