using Microsoft.Xna.Framework;
using TileEngine.Sprite;
using Microsoft.Xna.Framework.Input;

namespace TileEngine
{
    public class Camera
    {
        #region Fields
        //private float zoom = 1.0f;
        //private float rotation = 0.0f;
        public Vector2 Position = Vector2.Zero;
        #endregion

        #region Properties
        public Vector2 MouseLocationScreen
        {
            get
            {
                MouseState ms = Mouse.GetState();
                return new Vector2(ms.X, ms.Y);
            }
        }
        public Vector2 MouseLocationWorld
        {
            get
            {
                MouseState ms = Mouse.GetState();
                return new Vector2(ms.X, ms.Y) + Position;
            }
        }
        public Matrix TransformMatrix
        {
            get 
            { 
                return Matrix.CreateTranslation(new Vector3(-Position, 0f)); 
            }
        }
        #endregion


        #region LockToTarget
        public void LockToTarget(AnimatedSprite target, int screenWidth, int screenHeight)
        {
            this.Position.X = (int)(target.Position.X + (target.CurrentAnimation.CurrentRect.Width / 2) - (screenWidth / 2));
            this.Position.Y = (int)(target.Position.Y + (target.CurrentAnimation.CurrentRect.Height / 2) - (screenHeight / 2));
            //this.Position.X = target.Position.X + (128 / 2) - (screenWidth / 2);
            //this.Position.Y = target.Position.Y + (128 / 2) - (screenHeight / 2);
        }
        public void LockToTarget(SimpleSprite target, int screenWidth, int screenHeight)
        {
            this.Position.X = (int)(target.Position.X + (target.Texture.Width / 2) - (screenWidth / 2));
            this.Position.Y = (int)(target.Position.Y + (target.Texture.Height / 2) - (screenHeight / 2));
        }
        #endregion

        #region ClampCameraToArea
        public void ClampToArea(int width, int height)
        {
            if (Position.X > width)
                Position.X = width;

            if (Position.Y > height)
                Position.Y = height;

            if (Position.X < 0)
                Position.X = 0;

            if (Position.Y < 0)
                Position.Y = 0;
        }
        #endregion
    }
}
