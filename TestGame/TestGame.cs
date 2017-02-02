using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TileEngine;
using TileEngine.LayerMap;
using TileEngine.Sprite;

namespace TestGame
{
    public class TestGame : GameEngine
    {
        public static string DebugMsg = "No Message";

        public TestGame() : base(1280, 720)
        {
            base.GraphicsDeviceManager.IsFullScreen = false;
            GameEngine.CurrentGameState = GameState.Debugging;
            base.CurrentInputType = InputType.MouseKeyboard;
        }
        protected override void Initialize()
        {
            //create a new SpriteBatch, which can be used to draw textures.
            base.SpriteBatch = new SpriteBatch(GraphicsDevice);
            //create krypton, link to this game, use shaders
            base.Maps.Add(new Map(this, true));
            //set currentMap
            GameEngine.CurrentMap = base.Maps[0];

            base.Initialize();
        }
        protected override void LoadContent()
        {
            //use base.Content to load your game content here
            //load map0 layers and backbuffer
            base.Maps[0].CollisionLayer = CollisionLayer.FromFile("Content/layers/map1/collision.lyr");
            base.Maps[0].TacticalLayer = TileLayer.FromFile(Content, "Content/layers/map1/tactical_map.lyr");
            base.Maps[0].TacticalLayer.Alpha = 1.0f;
            base.Maps[0].Layers.Add(TileLayer.FromFile(Content, "Content/layers/map1/map1.lyr"));
            base.Maps[0].Layers[0].Alpha = 1f;

            //player setup
            AnimatedSprite pas = new AnimatedSprite(base.Content.Load<Texture2D>("sprites/ship_ani"), null);
            pas.AddAnimation(2, 64, 64, 0, 0, 10, "Idle");
            pas.Animations["Idle"].UpdateType = UpdateType.Looped;
            pas.CurrentAnimationName = "Idle";
            PlayerObject player = new PlayerObject
                (
                    "Player", 
                    new Vector2(64, 64), 
                    pas, 
                    base.Content.Load<Texture2D>("particles/particle1"), 
                    base.Content.Load<Texture2D>("particles/particle_base")
                );
            player.CurrentMap = CurrentMap;
        }
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            //input and paused user interface logic goes here
            GameEngine.NewKeyState = Keyboard.GetState();

            if (GameEngine.CurrentGameState != GameState.Paused)
            {
                //game logic goes here
                GameEngine.CurrentMap.Update(gameTime);
                //camera update
                Camera.LockToTarget(((AnimatedSprite)GameEngine.CurrentMap.Player.Sprite), base.Resolution.X, base.Resolution.Y);
                Camera.ClampToArea(GameEngine.CurrentMap.CollisionLayer.WidthInPixels - base.Resolution.X, GameEngine.CurrentMap.CollisionLayer.HeightInPixels - base.Resolution.Y);

                //handle gameobject collisions
                if (CurrentMap.GameObjectsEarly.Count != 0)
                {
                    for (int i = 0; i < CurrentMap.GameObjectsEarly.Count; i++)
                    {
                        //GameObject.HandleCollision((GameObject)CurrentMap.GameObjectsEarly[i], (GameObject)CurrentMap.Player);

                        //for (int j = 0; j < CurrentMap.GameObjectsEarly.Count; j++)
                        //{
                        //    GameObject.HandleCollision((GameObject)CurrentMap.GameObjectsEarly[i], (GameObject)CurrentMap.GameObjectsEarly[j]);
                        //}
                    }
                }
            }
            base.UpdateGameStateControls();
            //debugging code goes here
            if (GameEngine.CurrentGameState == GameState.Debugging)
                base.Maps[0].UseTacticalLayer = true;
            else
                base.Maps[0].UseTacticalLayer = false;
            base.TickFrameCounter(gameTime, " - " + DebugMsg);
            //update the base code
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {

            //draw layered tiles
            GameEngine.CurrentMap.Draw(base.SpriteBatch, Camera, gameTime, base.GraphicsDevice);

            base.Draw(gameTime);
        }
    }
}
