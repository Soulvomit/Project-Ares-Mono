using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace TileEngine.Sprite
{
    public class Animation : ICloneable
    {
        #region Fields
        Rectangle[] frames;
        int currentFrame = 0;
        float frameLength = 0.5f;
        float timer = .0f;
        UpdateType updateType = UpdateType.Looped;
        #endregion

        #region Properties
        public UpdateType UpdateType 
        {
            get { return updateType; }
            set { updateType = value; }
        }
        public Rectangle[] Frames
        {
            get { return frames; }
        }
        public int FramesPerSecond 
        {
            get { return (int)(1f / frameLength); }
            set { frameLength = (float)Math.Max(1f / (float)value, .001f); }
        }
        public Rectangle CurrentRect 
        {
            get  { return frames[currentFrame]; }
        }
        public int CurrentFrame 
        {
            get { return currentFrame; }
            set { currentFrame = (int)MathHelper.Clamp(value, 0, frames.Length -1); }
        }
        #endregion

        #region Constructors
        public Animation(int numberOfFrames, int frameWidth, int frameHeight, int xOffset, int yOffset)
        {
            frames = new Rectangle[numberOfFrames];

            for (int i = 0; i < numberOfFrames; i++)
            {
                Rectangle rect = new Rectangle();
                rect.Width = frameWidth;
                rect.Height = frameHeight;
                rect.X = xOffset + (i * frameWidth);
                rect.Y = yOffset;

                frames[i] = rect;
            }
        }
        public Animation(int numberOfColums, int numberOfRows,  int frameWidth, int frameHeight, int xOffset, int yOffset)
        {
            List<Rectangle> temp = new List<Rectangle>(); 
            //frames = new Rectangle[numberOfColums * numberOfRows];

            for (int y = 0; y < numberOfRows; y++)
            {
                for (int x = 0; x < numberOfColums; x++)
                {
                    Rectangle rect = new Rectangle();
                    rect.Width = frameWidth;
                    rect.Height = frameHeight;
                    rect.X = xOffset + (x * frameWidth);
                    rect.Y = yOffset + (y * frameHeight);

                    //frames[(x+1)*(y+1)] = rect;
                    temp.Add(rect);
                }
            }
            frames = temp.ToArray();
        }
        public Animation(int numberOfFrames, int frameWidth, int frameHeight)
        {
            frames = new Rectangle[numberOfFrames];

            for (int i = 0; i < numberOfFrames; i++)
            {
                Rectangle rect = new Rectangle();
                rect.Width = frameWidth;
                rect.Height = frameHeight;
                rect.X = 0;
                rect.Y = 0;

                frames[i] = rect;
            }
        }

        private Animation() { }
        #endregion

        #region Update
        public void Update(GameTime gameTime)
        {
            if(updateType == UpdateType.Looped)
            {
                this.UpdateLooped(gameTime);
            }
            else if(updateType == UpdateType.Forward)
            {
                this.UpdateForward(gameTime);
            }
            else if(updateType == UpdateType.Backward)
            {
                this.UpdateBackward(gameTime);
            }
            else
            {
                throw new Exception("Update type undefined.");
            }
        }
        private void UpdateLooped(GameTime gameTime)
        {
            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (timer >= frameLength)
            {
                timer = 0f;

                currentFrame++;

                if (currentFrame >= frames.Length)
                    currentFrame = 0;
            }
        }
        private void UpdateForward(GameTime gameTime)
        {
            if (currentFrame < frames.Length-1)
            {
                timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (timer >= frameLength)
                {
                    timer = 0f;

                    currentFrame++;
                }
            }
        }
        private void UpdateBackward(GameTime gameTime)
        {
            if (currentFrame >= 0)
            {
                timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (timer >= frameLength)
                {
                    timer = 0f;

                    currentFrame++;
                }
            }
        }
        #endregion

        #region Clone
        public object Clone()
        {
            Animation anim = new Animation();

            anim.frameLength = this.frameLength;
            anim.frames = this.frames;

            return anim;
        }
        #endregion
    }
    public enum UpdateType
    {
        Looped,
        Forward,
        Backward
    }
}
