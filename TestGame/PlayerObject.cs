using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Penumbra;
using System;
using TileEngine;
using TileEngine.LayerMap;
using TileEngine.ParticleSys;
using TileEngine.Sprite;

namespace TestGame
{
    public enum Movement: byte
    {
        Accelerating,
        Deccelerating,
        Stopped
    }
    public class PlayerObject : MovableObject
    {
        protected Movement currentMovement;
        protected float angleOffset;
        protected Vector2 mouseLoc;

        public PlayerObject(string name, Vector2 startingPos, AnimatedSprite sprite, Texture2D particleTex, Texture2D lightTex) 
            : base(name, startingPos, sprite)
        {
            base.sprite.Position = startingPos;
            base.sprite.CollisionRange = 32;
            this.currentMovement = Movement.Stopped;
            this.angleOffset = MathHelper.PiOver2;
            this.mouseLoc = Vector2.Zero;

            //create ambient light
            TexturedLight ambient =   new TexturedLight
            {
                Texture = lightTex,
                Intensity = 1.5f,
                Scale = new Vector2(384, 384),
                Rotation = Sprite.Angle,
                Color = Color.White,
                //Radius = 8,
                //ConeDecay = 0.2f,
                Enabled = true
            };
            ambient.Position = Position;
            //ambient.Origin = new Vector2(0.1f, 0.5f);
            base.lights.Add("Ambient", ambient);

            //create engine particle system
            ParticleSystem engine = new ParticleSystem(Position);
            engine.AddEmitter(
                new Vector2(0.030f, 0.005f), 
                new Vector2(0, -1), 
                new Vector2(0.001f * MathHelper.Pi, 0.001f * MathHelper.Pi),
                new Vector2(0.85f, 1.05f), new Vector2(120 / 2, 140 / 2), 
                new Vector2(60 / 4, 70 / 4), 
                Color.Blue, 
                Color.LightBlue, 
                Color.Red, 
                Color.Crimson,
                new Vector2(400 / 3, 500 / 3), 
                new Vector2(100 / 3, 120 / 3), 
                1000, 
                Vector2.Zero, 
                particleTex);
            engine.Emitters[0].StopEmmiting = true;
            base.particleSystems.Add("Engine", engine);
        }
        protected override void SwitchMap(Map map)
        {
            if (currentMap != null)
            {
                currentMap.Player = null;
            }
            map.Player = this;
            currentMap = map;
        }

        public override void HandleObjectCollision(IMapObject collidee)
        {
            throw new NotImplementedException();
        }

        private void MoveAmbientLight()
        {
            if (currentMovement != Movement.Stopped)
            {
                lights["Ambient"].Position = sprite.Center;
                lights["Ambient"].Rotation = sprite.Angle - angleOffset;
            }
        }
        private void MoveEngine(int offset)
        {
            if (currentMovement == Movement.Accelerating)
            {
                particleSystems["Engine"].Emitters[0].StopEmmiting = false;
                Vector2 veloNormalized = Vector2.Normalize(velocity);
                particleSystems["Engine"].Emitters[0].SpawnDirection = -veloNormalized;
                particleSystems["Engine"].Position = sprite.Center + (-veloNormalized * offset);
            }
            else if (currentMovement == Movement.Deccelerating || currentMovement == Movement.Stopped)
            {
                particleSystems["Engine"].Emitters[0].StopEmmiting = true;
            }
        }
        private void MoveAnimated()
        {
            if (currentMovement == Movement.Accelerating || currentMovement == Movement.Deccelerating)
            {
                base.AnimatedSprite.Animations["Idle"].FramesPerSecond = (int)(40 * currentSpeed);
            }
            else
            {
                base.AnimatedSprite.Animations["Idle"].FramesPerSecond = 0;
            }
        }
        private void RotateToCell(Vector2 direction)
        {
            if (currentMovement == Movement.Accelerating || currentMovement == Movement.Deccelerating)
            {
                Point pointToCell = Helper.VPointToCell(direction);
                Vector2 centerOfCell = Helper.CellToCellCenterPoint(pointToCell.X, pointToCell.Y, Vector2.Zero);
                Vector2 mouseDirection = centerOfCell - sprite.Center;
                mouseDirection.Normalize();
                base.Rotate(mouseDirection, angleOffset);
            }
        }
        public void UpdateControl()
        {
            MouseState curMouse = Mouse.GetState();
            mouseLoc = new Vector2(curMouse.X, curMouse.Y) + GameEngine.Camera.Position;

            if (curMouse.LeftButton == ButtonState.Pressed && !Sprite.IsColliding(mouseLoc))
            {
                currentMovement = Movement.Accelerating;
            }
            else if (currentSpeed == 0.0f)
            {
                currentMovement = Movement.Stopped;
            }
            else
            {
                currentMovement = Movement.Deccelerating;
            }
            if (GameEngine.CurrentGameState == GameState.Debugging)
            {
                TestGame.DebugMsg = "Current Movemenet: " + currentMovement;
            }
        }

        public override void Update(GameTime gameTime)
        {
            this.UpdateControl();

            if (currentMovement == Movement.Accelerating)
            {
                //accelerate
                base.Accelerate();
                //move
                base.Move(angleOffset);
            }
            else if(currentMovement == Movement.Deccelerating)
            {
                //deccelerate
                base.Deccelerate();
                //move
                base.Move(angleOffset);
            }
            //rotate to cell
            this.RotateToCell(mouseLoc);
            //move engine
            this.MoveEngine(60);
            //move ambient light
            this.MoveAmbientLight();
            //animate movement
            this.MoveAnimated();
            //base update
            base.Update(gameTime);

            if (GameEngine.CurrentGameState == GameState.Debugging)
            {
                //TestGame.DebugMsg = "Mouse Angle: " + Math.Atan2(mouseDirection.Y, mouseDirection.X) + 
                //    " - Face Angle: " + (sprite.Angle - MathHelper.PiOver2);
            }
        }
        public override void Draw(SpriteBatch spriteBatch, Camera camera)
        {
            base.Draw(spriteBatch, camera);
        }
    }
}
