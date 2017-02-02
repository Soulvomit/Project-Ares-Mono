using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using TileEngine.Sprite;

namespace TileEngine.LayerMap
{
    public class MovableObject : MapObject
    {
        protected float currentSpeed;
        protected float maxSpeed;
        protected float minSpeed;
        protected float acceleration;
        protected float decceleration;
        protected Vector2 velocity;
        protected float rotationSpeed;
        protected float dt = 0;

        public MovableObject(string name, Vector2 startingPos, AnimatedSprite sprite): base(name, startingPos, sprite)
        {
            this.velocity = Vector2.Zero;
            this.acceleration = 0.0003f;
            this.decceleration = 0.0005f;
            this.rotationSpeed = 0.003f;
            this.currentSpeed = 0f;
            this.maxSpeed = 0.5f;
            this.minSpeed = 0.05f;
        }

        public AnimatedSprite AnimatedSprite
        {
            get
            {
                return (sprite as AnimatedSprite);
            }
        }

        public float CurrentSpeed
        {
            get { return currentSpeed; }
            set { currentSpeed = value; }
        }
        public float MinimalSpeed
        {
            get { return minSpeed; }
            set { minSpeed = value; }
        }

        public virtual void Move(float offset = 0)
        {
            Vector2 faceDirection = base.GetDirection(offset);
            velocity = faceDirection * (currentSpeed * dt);
            sprite.Position += velocity;
        }

        public void Accelerate()
        {
            if (currentSpeed < maxSpeed)
            {
                currentSpeed = currentSpeed + (acceleration * dt);
            }
            else
            {
                currentSpeed = maxSpeed;
            }
        }
        public void Deccelerate()
        {
            if (currentSpeed > 0)
            {
                currentSpeed = currentSpeed - (decceleration * dt);
            }
            else
            {
                currentSpeed = 0;
            }
        }

        public virtual void Rotate(Vector2 direction, float offset)
        {
            //sprite.Angle =
            //    MathHelper.Lerp(sprite.Angle, (float)(Math.Atan2(direction.Y, direction.X)) + offset, rotationSpeed * dt);
            sprite.Angle = Helper.CurveAngle(sprite.Angle, (float)(Math.Atan2(direction.Y, direction.X)) + offset, rotationSpeed * dt);
        }

        public override void HandleObjectCollision(IMapObject collidee)
        {
            throw new NotImplementedException();
        }

        public override void HandleMapCollision()
        {
            currentMap.CollisionLayer.HandleBlockedCells(this);
        }

        public override void Update(GameTime gameTime)
        {
            dt = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            HandleMapCollision();

            sprite.ClampToArea(currentMap.CollisionLayer.WidthInPixels, currentMap.CollisionLayer.HeightInPixels);

            if (base.sprite.GetType() == typeof(AnimatedSprite))
            {
                (base.sprite as AnimatedSprite).Update(gameTime);
            }
            base.Update(gameTime);
        }
        public override void Draw(SpriteBatch spriteBatch, Camera camera)
        {
            base.Draw(spriteBatch, camera);
        }
    }
}
