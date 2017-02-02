using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Penumbra;
using TileEngine.Pathfinding;
using System;

namespace TileEngine.LayerMap
{
    public class Map : IOPFMap
    {
        #region Fields
        //bacground music
        private SoundEffectInstance musicInstance;
        //player
        public IMapObject Player;
        //krypton light engine
        public PenumbraComponent Penumbra;
        //mapped gameobjects
        public List<IMapObject> GameObjectsEarly = new List<IMapObject>();
        //mapped gameobjects to be drawn late
        public List<IMapObject> GameObjectsLate = new List<IMapObject>();
        //collection of the map layers
        public List<TileLayer> Layers = new List<TileLayer>();
        //collision layer of the map
        public CollisionLayer CollisionLayer = null;
        //tactical layer
        public TileLayer TacticalLayer = null;
        //water layer
        public TileLayer WaterLayer = null;
        //background image
        public Texture2D Background;
        //clear color
        public Color BackBufferColor = Color.Black;
        //ambient color
        public Color AmbientColor = new Color(50, 50, 50, 255);
        //public Color AmbientColor = Color.White;
        #endregion

        #region Properties
        public bool UseTacticalLayer { get; set; }
        public bool UseBackground { get; set; }

        public SoundEffect MusicInstance
        {
            set
            {
                if (musicInstance != null)
                    musicInstance.Stop();
                musicInstance = value.CreateInstance();
                musicInstance.IsLooped = true;
                musicInstance.Play();
            }
        }
        #endregion
        public ushort Width
        {
            get
            {
                return (ushort)CollisionLayer.Width;
            }
        }

        public ushort Height
        {
            get
            {
                return (ushort)CollisionLayer.Height;
            }
        }

        public uint MaxPathlength
        {
            get
            {
                return (uint)(Height * Width);
            }
        }

        public byte NodeBaseCost
        {
            get
            {
                return 1;
            }
        }

        #region Constructors
        public Map(Game game, bool useLightingSystem = true)
        {
            if (useLightingSystem)
            {
                //create krypton, link to this game, use shaders
                Penumbra = new PenumbraComponent(game);
                this.InitializeMapLight();
            }
        }
        #endregion

        #region InitializeMapLight
        private void InitializeMapLight()
        {
            //make sure to initialize krypton, unless it has been added to the Game's list of Components
            Penumbra.Initialize();
            Penumbra.AmbientColor = AmbientColor;
            Penumbra.Game.GraphicsDevice.RasterizerState = RasterizerState.CullClockwise;
        }
        #endregion

        #region MusicToggle
        public void MusicToggle()
        {
            if (musicInstance.State == SoundState.Playing)
                musicInstance.Pause();
            else if (musicInstance.State == SoundState.Paused)
                musicInstance.Resume();
        }
        #endregion

        #region Create Light/Hull
        public SpotLight CreateLight(float radius, Color color, float intensity, float rotation, Vector2 position, ShadowType st)
        {
            SpotLight light = new SpotLight()
            {
                Radius = radius,
                Color = color,
                Intensity = intensity,
                Rotation = rotation,
                Position = position,
                ShadowType = st
            };
            Penumbra.Lights.Add(light);
            return light;
        }

        public Hull CreateHull(float rotation, Vector2 position, Vector2 scale, params Vector2[] points)
        {
            Hull hull;
            if (points.Length <= 3)
                hull = new Hull(points);
            else
                hull = Hull.CreateRectangle(Vector2.One);
            
            hull.Position = position;
            hull.Scale = scale;
            hull.Rotation = rotation;
            
            Penumbra.Hulls.Add(hull);
            return hull;
        }
        #endregion

        #region Add/Remove Light/Hull
        public void AddLight(Light light)
        {
            Penumbra.Lights.Add(light);
        }
        public void RemoveLight(Light light)
        {
            Penumbra.Lights.Remove(light);
        }
        public void AddHull(Hull hull)
        {
            Penumbra.Hulls.Add(hull);
        }
        public void RemoveHull(Hull hull)
        {
            Penumbra.Hulls.Remove(hull);
        }
        #endregion

        #region UnloadContent
        public void UnloadContent(bool lights, bool gameObjects, bool player)
        {
            if(lights)
            {
                foreach(Light light in Penumbra.Lights)
                    Penumbra.Lights.Remove(light);

                foreach (Hull hull in Penumbra.Hulls)
                    Penumbra.Hulls.Remove(hull);
            }
            if(gameObjects)
            {
                foreach (IMapObject go in this.GameObjectsEarly)
                    this.GameObjectsEarly.Remove(go);

                foreach (IMapObject gol in this.GameObjectsLate)
                    this.GameObjectsEarly.Remove(gol);
            }
            if(player)
            {
                Player = null;
            }
        }
        #endregion

        #region UpdateLights
        private void UpdateLights(GameTime gameTime)
        {
            foreach (Light light in Penumbra.Lights)
            {
                if (!light.Enabled && Player.Sprite.InLightingRange(light))
                {
                    light.Enabled = true;
                }
                else if (light.Enabled && !Player.Sprite.InLightingRange(light))
                {
                    light.Enabled = false;
                }
            }

            foreach (Hull hull in Penumbra.Hulls)
            {
                if (!hull.Enabled && Player.Sprite.InShadowRange(hull))
                {
                    hull.Enabled = true;
                }
                else if (hull.Enabled && !Player.Sprite.InShadowRange(hull))
                {
                    hull.Enabled = false;
                }
            }
        }
        #endregion

        #region Update
        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            UpdateLights(gameTime);
            //update early game objects
            foreach (IMapObject go in GameObjectsEarly)
            {
                //if (CurrentMap.Player.Sprite.InUpdateRange(go.Sprite))
                go.Update(gameTime);
            }
            //update player
            GameEngine.CurrentMap.Player.Update(gameTime);
            //EngineGame.CurrentMap.Player.Sprite.ClampToArea(EngineGame.CurrentMap.CollisionLayer.WidthInPixels - EngineGame.CurrentMap.Player.Sprite.Bounds.Width,
            //                                          EngineGame.CurrentMap.CollisionLayer.HeightInPixels - EngineGame.CurrentMap.Player.Sprite.Bounds.Height);
            //update late game objects
            foreach (IMapObject gol in GameObjectsLate)
            {
                //if (CurrentMap.Player.Sprite.InUpdateRange(gol.Sprite))
                gol.Update(gameTime);
            }
        }
        #endregion

        #region Draw
        /// <summary>
        /// Draws the map on a layer by layer basis, starting with the first layer in the Layers collection.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="camera"></param>
        public void Draw(SpriteBatch spriteBatch, Camera camera, GameTime gameTime, GraphicsDevice graphicsDevice)
        {
            Point min = Helper.VPointToCell(camera.Position);
            Point max = Helper.VPointToCell(camera.Position +
                new Vector2(spriteBatch.GraphicsDevice.Viewport.Width + Helper.TileWidth,
                            spriteBatch.GraphicsDevice.Viewport.Height + Helper.TileHeight));

            //assign the matrix and pre-render the lightmap each draw cycle
            //make sure not to change the position of any lights or shadow hulls after this call, as it won't take effect till the next frame
            this.Penumbra.Transform = camera.TransformMatrix;
            this.Penumbra.BeginDraw();
            //clear the backbuffer
            graphicsDevice.Clear(BackBufferColor);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, null, null, camera.TransformMatrix);
            
            if (UseBackground && Background != null)
                spriteBatch.Draw(Background, new Rectangle(0, 0, Layers[0].WidthInPixels, Layers[0].WidthInPixels), Color.White);

            foreach (TileLayer layer in Layers)
            {
                layer.Draw(spriteBatch, camera, min, max);
                //layer.Draw(spriteBatch, camera);
            }

            if (UseTacticalLayer && TacticalLayer != null)
                TacticalLayer.Draw(spriteBatch, camera, min, max);

            spriteBatch.End();

            //draw early game objects
            foreach (IMapObject go in this.GameObjectsEarly)
            {
                if(Player.Sprite.InDrawRange(go.Sprite))
                    go.Draw(spriteBatch, camera);
            }
            //draw player
            this.Player.Draw(spriteBatch, camera);
            //draw late game objects
            foreach (IMapObject gol in this.GameObjectsLate)
            {
                if (Player.Sprite.InDrawRange(gol.Sprite))
                    gol.Draw(spriteBatch, camera);
            }

            //2D lighting draw
            this.Penumbra.Draw(gameTime);
        }
        #endregion

        public byte[] GetLinearTopography()
        {
            throw new NotImplementedException();
        }
    }
}
