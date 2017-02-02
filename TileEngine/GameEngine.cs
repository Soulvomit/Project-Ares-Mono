using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Windows.Forms;
using System;
using Microsoft.Xna.Framework.Input;

namespace TileEngine
{
    public enum InputType
    {
        MouseKeyboard,
        Controller
    }
    public enum GameState
    {
        Paused,
        Unpaused,
        Debugging
    }
    public class GameEngine : Game
    {     
        //graphics device manager
        public GraphicsDeviceManager GraphicsDeviceManager;
        //spritebatch for drawing
        public SpriteBatch SpriteBatch;
        //the current map to be updated and drawn
        public static LayerMap.Map CurrentMap;
        //all maps loaded
        public List<LayerMap.Map> Maps = new List<LayerMap.Map>();
        //form handle for window
        public Form FormWindow;
        //settings
        //static camera
        public static Camera Camera = new Camera();
        //set input type
        public InputType CurrentInputType = InputType.MouseKeyboard;
        //set the gamestate
        public static GameState CurrentGameState = GameState.Unpaused;
        //set resolution
        public Point Resolution;
        //for calculating FPS
        public int FrameCounter = 0, FPS = 0;
        public double FrameTime = 0.0;
        //key states
        public static KeyboardState OldKeyState;
        public static KeyboardState NewKeyState;

        public GameEngine(int resolutionX = 1920, int resolutionY = 1080) 
        {
            //set graphics device mananger
            this.GraphicsDeviceManager = new GraphicsDeviceManager(this);
            //set contet root directory
            base.Content.RootDirectory = "Content";
            //create form for window
            this.FormWindow = (Form)Form.FromHandle(this.Window.Handle);
            this.FormWindow.MinimizeBox = true;
            this.FormWindow.Text = "Engine Game";
            //screen settings
            this.Resolution = new Point(resolutionX, resolutionY);
            this.GraphicsDeviceManager.PreferredBackBufferWidth = this.Resolution.X;
            this.GraphicsDeviceManager.PreferredBackBufferHeight = this.Resolution.Y;
            this.GraphicsDeviceManager.PreferMultiSampling = true;
            this.GraphicsDeviceManager.IsFullScreen = false;
            //vsync
            this.GraphicsDeviceManager.SynchronizeWithVerticalRetrace = false;
            //variable time step
            base.IsFixedTimeStep = true;
            //allow the window to be resized
            base.Window.AllowUserResizing = true;
            //set if mouse should be visable
            base.IsMouseVisible = true;
            //apply all
            this.GraphicsDeviceManager.ApplyChanges();
        }
        public void UpdateGameStateControls()
        {
            //hit escape to exit.
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == Microsoft.Xna.Framework.Input.ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape))
                Exit();

            //hit p to toggle pause.
            if (NewKeyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.P) && OldKeyState.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.P))
            {
                if (CurrentGameState == GameState.Paused)
                    CurrentGameState = GameState.Unpaused;
                else
                    CurrentGameState = GameState.Paused;
            }
            //hit p to toggle pause
            if (NewKeyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D) && OldKeyState.IsKeyUp(Microsoft.Xna.Framework.Input.Keys.D))
            {
                if (CurrentGameState == GameState.Debugging)
                    CurrentGameState = GameState.Unpaused;
                else
                    CurrentGameState = GameState.Debugging;
            }

            OldKeyState = NewKeyState;
        }
        public void TickFrameCounter(GameTime gameTime, string debugMessage)
        {
            if (CurrentGameState == GameState.Debugging)
            {
                //determine FPS
                FrameTime += gameTime.ElapsedGameTime.TotalSeconds;
                if (FrameTime > 1)
                {
                    FPS = FrameCounter;
                    FrameCounter = 0;
                    FrameTime = 0;

                }
                FrameCounter++;
                this.FormWindow.Text = "Current FPS: " + FPS + " - EGT: " + Math.Round((float)gameTime.ElapsedGameTime.TotalMilliseconds) + " - GameState: " + CurrentGameState
                + " - Position: " + CurrentMap.Player.Sprite.Position + debugMessage;
            }
            else
            {
                this.FormWindow.Text = "GameState: " + CurrentGameState;
            }
        }
    }
}
