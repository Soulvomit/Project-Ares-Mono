using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Penumbra;

namespace TileEngine.Sprite
{
    public class SimpleSprite: ISprite
    {
        #region Fields
        protected float alpha = 1.0f;
        protected float scale = 1.0f;
        protected Texture2D texture;
        protected Texture2D boundsTexture;
        protected Vector2 position = Vector2.Zero;
        protected float collisionRange = 8f;
        protected float interactionRange = 40f;
        protected float combatRange = 400f;
        protected float detectionRange = 700f;
        protected float lightingRange = 1280f;
        protected float updateRange = 1920f;
        protected float drawRange = 1920f;
        protected float shadowRange = 1024f;
        protected float angle = 0f;
        #endregion

        #region Properties
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public float PositionX
        {
            get { return position.X; }
            set { position.X = value; }
        }

        public float PositionY
        {
            get { return position.Y; }
            set { position.Y = value; }
        }

        public float Angle
        {
            get { return angle; }
            set { angle = value; }
        }

        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }

        public virtual Rectangle Bounds
        {
            get { return new Rectangle((int)Position.X, (int)Position.Y, (int)(texture.Width * scale), (int)(texture.Height * scale)); }
        }

        public Vector2 Origin
        {
            get { return Position + OriginOffset; }
        }

        public virtual Vector2 OriginOffset
        {
            get { return new Vector2(texture.Width / 2, texture.Height); }
            set { OriginOffset = value; }
        }

        public float Alpha
        {
            get { return alpha; }
            set
            {
                alpha = MathHelper.Clamp(value, 0f, 1f);
            }
        }

        public float Scale
        {
            get { return scale; }
            set
            {
                scale = value;
            }
        }

        public virtual Vector2 Center
        {
            get { return Position + new Vector2(texture.Width / 2, texture.Height / 2); }
        }

        public float CollisionRange
        {
            get { return collisionRange; }
            set { collisionRange = value; }
        }

        public float CombatRange
        {
            get { return combatRange; }
            set { combatRange = value; }
        }

        public float InteractionRange
        {
            get { return interactionRange; }
            set { interactionRange = value; }
        }

        public float DetectionRange
        {
            get { return detectionRange; }
            set { detectionRange = value; }
        }
        #endregion

        #region Constructor
        public SimpleSprite(Texture2D texture, Texture2D boundsTexture)
        {
            this.texture = texture;
            this.boundsTexture = boundsTexture;
        }
        #endregion

        #region InLightingRange
        public bool InLightingRange(Light light, SpriteOrigin so = SpriteOrigin.SpriteCenter)
        {
            Vector2 distance;

            if (so == SpriteOrigin.SpriteCenter)
                distance = light.Position - Center;
            else if (so == SpriteOrigin.SpritePosition)
                distance = light.Position - Position;
            else
                distance = light.Position - Origin;

            return (distance.Length() < this.lightingRange);
        }
        #endregion

        #region InShadowRange
        public bool InShadowRange(Hull hull, SpriteOrigin so = SpriteOrigin.SpriteCenter)
        {
            Vector2 distance;

            if (so == SpriteOrigin.SpriteCenter)
                distance = hull.Position - Center;
            else if (so == SpriteOrigin.SpritePosition)
                distance = hull.Position - Position;
            else
                distance = hull.Position - Origin;

            return (distance.Length() < this.lightingRange);
        }
        #endregion

        #region InUpdateRange
        public bool InUpdateRange(ISprite updateObject, SpriteOrigin so = SpriteOrigin.SpriteCenter)
        {
            Vector2 distance;

            if (so == SpriteOrigin.SpriteCenter)
                distance = updateObject.Center - Center;
            else if (so == SpriteOrigin.SpritePosition)
                distance = updateObject.Position - Position;
            else
                distance = updateObject.Origin - Origin;

            return (distance.Length() < this.updateRange);
        }
        #endregion

        #region InDrawRange
        public bool InDrawRange(ISprite drawObject, SpriteOrigin so = SpriteOrigin.SpriteCenter)
        {
            Vector2 distance;

            if (so == SpriteOrigin.SpriteCenter)
                distance = drawObject.Center - Center;
            else if (so == SpriteOrigin.SpritePosition)
                distance = drawObject.Position - Position;
            else
                distance = drawObject.Origin - Origin;

            return (distance.Length() < this.drawRange);
        }
        #endregion

        #region InDetectionRange
        public bool InDectectionRange(ISprite detectableObject, SpriteOrigin so = SpriteOrigin.SpriteCenter)
        {
            Vector2 distance;

            if (so == SpriteOrigin.SpriteCenter)
                distance = detectableObject.Center - this.Center;
            else if (so == SpriteOrigin.SpritePosition)
                distance = detectableObject.Position - this.Position;
            else
                distance = detectableObject.Origin - this.Origin;

            return (distance.Length() < this.DetectionRange);
        }
        #endregion

        #region InInteractionRange
        public bool InInteractionRange(ISprite interactableObject, SpriteOrigin so = SpriteOrigin.SpriteCenter)
        {
            Vector2 distance;

            if (so == SpriteOrigin.SpriteCenter)
                distance = interactableObject.Center - this.Center;
            else if (so == SpriteOrigin.SpritePosition)
                distance = interactableObject.Position - this.Position;
            else
                distance = interactableObject.Origin - this.Origin;

            return (distance.Length() < interactableObject.InteractionRange);
        }
        #endregion

        #region InCombatRange
        public bool InCombatRange(ISprite target, SpriteOrigin so = SpriteOrigin.SpriteCenter)
        {
            Vector2 distance;

            if (so == SpriteOrigin.SpriteCenter)
                distance = target.Center - this.Center;
            else if (so == SpriteOrigin.SpritePosition)
                distance = target.Position - this.Position;
            else
                distance = target.Origin - this.Origin;

            return (distance.Length() < this.combatRange);
        }
        #endregion

        #region IsColliding
        public bool IsColliding(ISprite collisionObject, SpriteOrigin so = SpriteOrigin.SpriteCenter)
        {
            Vector2 distance;

            if (so == SpriteOrigin.SpriteCenter)
                distance = collisionObject.Center - this.Center;
            else if (so == SpriteOrigin.SpritePosition)
                distance = collisionObject.Position - this.Position;
            else
                distance = collisionObject.Origin - this.Origin;

            return (distance.Length() < collisionObject.CollisionRange + this.collisionRange);
        }
        public bool IsColliding(Vector2 vector, SpriteOrigin so = SpriteOrigin.SpriteCenter)
        {
            Vector2 distance;

            if (so == SpriteOrigin.SpriteCenter)
                distance = vector - this.Center;
            else if (so == SpriteOrigin.SpritePosition)
                distance = vector - this.Position;
            else
                distance = vector - this.Origin;

            return (distance.Length() < this.collisionRange);
        }
        #endregion

        #region Distance
        public float Distance(ISprite toSprite, SpriteOrigin so = SpriteOrigin.SpriteCenter)
        {
            Vector2 distance;

            if (so == SpriteOrigin.SpriteCenter)
                distance = toSprite.Center - this.Center;
            else if (so == SpriteOrigin.SpritePosition)
                distance = toSprite.Position - this.Position;
            else
                distance = toSprite.Origin - this.Origin;

            return distance.Length();
        }
        #endregion

        #region ClampToArea
        public virtual void ClampToArea(int width, int height)
        {
            if (this.Position.X > width)
                this.Position = new Vector2(width, this.Position.Y);

            if (this.Position.Y > height)
                this.Position = new Vector2(this.Position.X, height);

            if (this.Position.X < 0)
                this.Position = new Vector2(0, this.Position.Y);

            if (this.Position.Y < 0)
                this.Position = new Vector2(this.Position.X, 0);
        }
        #endregion

        #region Draw
        /// <summary>
        /// draw with different rotation and scale
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="angle"></param>
        public virtual void Draw(SpriteBatch spriteBatch, SpriteEffects spriteEffects)
        {
            if (texture != null)
            {
                spriteBatch.Draw(Texture,
                                 Position + new Vector2(texture.Width / 2, texture.Height / 2),
                                 null,
                                 new Color(new Vector4(1f, 1f, 1f, alpha)),
                                 angle,
                                 new Vector2(Texture.Width / 2, Texture.Height / 2),
                                 scale,
                                 spriteEffects,
                                 0);
                if (GameEngine.CurrentGameState == GameState.Debugging)
                {
                    spriteBatch.Draw(boundsTexture, Bounds, new Color(new Vector4(1f, 1f, 1f, alpha)));
                }
            }
        }
        /// <summary>
        /// standard draw
        /// </summary>
        /// <param name="spriteBatch"></param>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (texture != null)
            {
                spriteBatch.Draw(texture, Position, new Color(new Vector4(1f, 1f, 1f, alpha)));
                if (GameEngine.CurrentGameState == GameState.Debugging)
                {
                    spriteBatch.Draw(boundsTexture, Bounds, new Color(new Vector4(1f, 1f, 1f, alpha)));
                }
            }
        }
        #endregion
    }
}
