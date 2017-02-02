using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TileEngine.Sprite
{
    public class AnimatedSprite: SimpleSprite
    {
        #region Fields
        private string currentAnimation = null;
        private bool animating = true;
        #endregion

        #region Properties
        public Dictionary<string, Animation> Animations = new Dictionary<string, Animation>();

        public override Vector2 OriginOffset
        {
            get { return new Vector2(CurrentAnimation.CurrentRect.Width / 2, CurrentAnimation.CurrentRect.Height); }
            set { OriginOffset = value; }
        }

        public override Vector2 Center
        {
            get { return Position + new Vector2(CurrentAnimation.CurrentRect.Width / 2, CurrentAnimation.CurrentRect.Height / 2); }
        }

        public override Rectangle Bounds
        {
            get
            {
                Rectangle rect = new Rectangle(0, 0, CurrentAnimation.CurrentRect.Width, (int)(CurrentAnimation.CurrentRect.Height*(((scale-1)/2)+1)));
                rect.X = (int)(Position.X);
                rect.Y = (int)(Position.Y);

                return rect;
            }
            //get { return new Rectangle((int)base.Position.X, (int)Position.Y, CurrentAnimation.CurrentRect.Width, CurrentAnimation.CurrentRect.Height); } 
        }

        public bool IsAnimating 
        {
            get { return animating; }
            set { animating = value; }
        }

        public Animation CurrentAnimation 
        {
            get 
            {
                if (!string.IsNullOrEmpty(currentAnimation))
                    return Animations[currentAnimation];
                else
                    return null;
            }
        }

        public string CurrentAnimationName
        {
            get { return currentAnimation; }
            set 
            {
                if (Animations.ContainsKey(value))
                    currentAnimation = value;
            }
        }
        #endregion

        #region Constructor
        public AnimatedSprite(Texture2D texture, Texture2D boundsTexture): base(texture, boundsTexture)
        {
        }
        #endregion

        #region ClampToArea
        public override void ClampToArea(int width, int height)
        {
            if (Position.X < 0)
                position.X = 0;

            if (Position.Y < 0)
                position.Y = 0;

            if (Position.X > width - CurrentAnimation.CurrentRect.Width)
                position.X = width - CurrentAnimation.CurrentRect.Width;

            if (Position.Y > height - CurrentAnimation.CurrentRect.Height)
                position.Y = height - CurrentAnimation.CurrentRect.Height;
        }
        #endregion

        #region AddAnimation
        public void AddAnimation(int numberOfFrames, int frameWidth, int frameHeight, int offsetX, int offsetY, int fps, string animationName)
        {
            Animation a = new Animation(numberOfFrames, frameWidth, frameHeight, offsetX, offsetY);
            a.FramesPerSecond = fps;
            Animations.Add(animationName, a);
        }
        public void AddAnimation(int numberOfColums, int numberOfRows, int frameWidth, int frameHeight, int offsetX, int offsetY, int fps, string animationName)
        {
            Animation a = new Animation(numberOfColums, numberOfRows, frameWidth, frameHeight, offsetX, offsetY);
            a.FramesPerSecond = fps;
            Animations.Add(animationName, a);
        }
        #endregion

        #region Update
        public void Update(GameTime gameTime)
        {
            if (!IsAnimating)
                return;

            Animation animation = CurrentAnimation;

            if (animation == null)
            {
                if (Animations.Count > 0)
                {
                    string[] keys = new string[Animations.Count];
                    Animations.Keys.CopyTo(keys, 0);

                    currentAnimation = keys[0];

                    animation = CurrentAnimation;
                }
                else return;
            }

            animation.Update(gameTime);
        }
        #endregion

        #region Draw
        public override void Draw(SpriteBatch spriteBatch)
        {
            Animation animation = CurrentAnimation;

            if (animation != null)
            {
                spriteBatch.Draw(texture, Position, animation.CurrentRect, new Color(new Vector4(1f, 1f, 1f, alpha)));
                if (GameEngine.CurrentGameState == GameState.Debugging)
                {
                    //spriteBatch.Draw(boundsTexture, Bounds,  new Color(new Vector4(1f, 1f, 1f, alpha)));
                }
            }
        }
        public override void Draw(SpriteBatch spriteBatch, SpriteEffects spriteEffect)
        {
            Animation animation = CurrentAnimation;

            if (animation != null)
            {
                //spriteBatch.Draw(texture, Position, 
                //    new Rectangle(animation.CurrentRect.X, animation.CurrentRect.Y, (int)(animation.CurrentRect.Width), (int)(animation.CurrentRect.Height)), 
                //    new Color(new Vector4(1f, 1f, 1f, alpha)), rotation, new Vector2(0f, 0f), this.scale, SpriteEffects.None, 0f);
                spriteBatch.Draw(texture, 
                                 Position + new Vector2(animation.CurrentRect.Width / 2, animation.CurrentRect.Height / 2),
                                 new Rectangle(animation.CurrentRect.X, animation.CurrentRect.Y, (int)(animation.CurrentRect.Width), (int)(animation.CurrentRect.Height)),
                                 new Color(new Vector4(1f, 1f, 1f, alpha)), 
                                 angle, 
                                 new Vector2(animation.CurrentRect.Width / 2, animation.CurrentRect.Height / 2), 
                                 scale,
                                 spriteEffect, 
                                 0);
                if (GameEngine.CurrentGameState == GameState.Debugging)
                {
                    //spriteBatch.Draw(boundsTexture, Bounds, new Color(new Vector4(1f, 1f, 1f, alpha)));
                }
            }
        }
        #endregion
    }
}
