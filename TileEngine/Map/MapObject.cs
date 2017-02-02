using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Penumbra;
using System;
using System.Collections.Generic;
using TileEngine.ParticleSys;
using TileEngine.Sprite;

namespace TileEngine.LayerMap
{
    public class MapObject : IMapObject
    {
        protected string name;
        protected Map currentMap;
        protected ISprite sprite;
        protected Dictionary<string, Light> lights;
        protected Dictionary<string, Hull> hulls;
        protected Dictionary<string, ParticleSystem> particleSystems;

        public MapObject(string name, Vector2 startingPos, ISprite sprite)
        {
            this.name = name;
            this.sprite = sprite;
            this.sprite.Position = startingPos;

            lights = new Dictionary<string, Light>();
            hulls = new Dictionary<string, Hull>();
            particleSystems = new Dictionary<string, ParticleSystem>(); 
        }

        public Map CurrentMap
        {
            get
            {
                if (currentMap == null)
                {
                    throw new Exception(name + " has no map!");
                }
                return CurrentMap;
            }

            set
            {
                if (currentMap != null)
                {
                    foreach (KeyValuePair<string, Light> kvp in lights)
                    {
                        currentMap.RemoveLight(kvp.Value);
                    }
                    foreach (KeyValuePair<string, Hull> kvp in hulls)
                    {
                        currentMap.RemoveHull(kvp.Value);
                    }
                }
                foreach (KeyValuePair<string, Light> kvp in lights)
                {
                    value.AddLight(kvp.Value);
                }
                foreach (KeyValuePair<string, Hull> kvp in hulls)
                {
                    value.AddHull(kvp.Value);
                }
                
                SwitchMap(value);
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public ISprite Sprite
        {
            get
            {
                return sprite;
            }
        }

        public Vector2 Position
        {
            get
            {
                return sprite.Position;
            }
            set
            {
                sprite.Position = value;
            }
        }

        public float Angle
        {
            set
            {
                this.sprite.Angle = value;
            }
        }

        public Vector2 GetDirection(float offset = 0)
        {
            Vector2 direction = Vector2.Zero;
            direction.X = (float)Math.Cos(sprite.Angle - offset);
            direction.Y = (float)Math.Sin(sprite.Angle - offset);
            return direction;
        }

        public float Scale
        {
            set
            {
                this.sprite.Scale = value;
            }
        }

        public virtual void HandleObjectCollision(IMapObject collidee)
        {
            throw new NotImplementedException();
        }
        public virtual void HandleMapCollision()
        {
            
        }

        public virtual void Update(GameTime gameTime)
        {
            foreach (KeyValuePair<string, ParticleSystem> kvp in particleSystems)
            {
                kvp.Value.Update((float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000f);
            }
        }
        public virtual void Draw(SpriteBatch spriteBatch, Camera camera)
        {
            foreach (KeyValuePair<string, ParticleSystem> kvp in particleSystems)
            {
                spriteBatch.Begin(SpriteSortMode.Immediate, kvp.Value.BlendState, null, null, null, null, camera.TransformMatrix);
                kvp.Value.Draw(spriteBatch, 1, Vector2.Zero);
                spriteBatch.End();
            }
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, null, null, null, null, camera.TransformMatrix);
            sprite.Draw(spriteBatch, SpriteEffects.None);
            spriteBatch.End();
        }

        protected virtual void SwitchMap(Map map)
        {
            if (currentMap != null)
            {
                if (currentMap.GameObjectsEarly.Contains(this))
                {
                    currentMap.GameObjectsEarly.Remove(this);
                    map.GameObjectsEarly.Add(this);
                }
                else if (currentMap.GameObjectsLate.Contains(this))
                {
                    currentMap.GameObjectsLate.Remove(this);
                    map.GameObjectsLate.Add(this);
                }
            }
            else
            {
                map.GameObjectsEarly.Add(this);
            }
            currentMap = map;
        }
    }
}
