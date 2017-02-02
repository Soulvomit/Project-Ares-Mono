using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace TileEngine.ParticleSys
{
    public class ParticleSystem
    {

        private Random random;
        private Vector2 position;
        private Vector2 offset = Vector2.Zero;
        private BlendState blendState = BlendState.Additive;

        public Vector2 LastPos;
        public List<Emitter> Emitters;


        public Vector2 Offset
        {
            get { return offset; }
            set { offset = value; }
        }
        public Vector2 Position
        {
            get { return position; }
            set
            {
                LastPos = position;
                position = value;
            }
        }
        public BlendState BlendState
        {
            get { return blendState; }
            set { blendState = value; }
        }

        public ParticleSystem(Vector2 Position)
        {
            this.Position = Position;
            this.LastPos = Position;
            random = new Random();
            Emitters = new List<Emitter>();
        }

        public void Update(float dt)
        {
            for (int i = 0; i < Emitters.Count; i++)
            {
                if (Emitters[i].Budget > 0)
                {
                    Emitters[i].Update(dt);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, int Scale, Vector2 Offset)
        {
            for (int i = 0; i < Emitters.Count; i++)
            {
                if (Emitters[i].Budget > 0)
                {
                    Emitters[i].Draw(spriteBatch, Scale, Offset);
                }
            }
        }

        public void Clear()
        {
            for (int i = 0; i < Emitters.Count; i++)
            {
                if (Emitters[i].Budget > 0)
                {
                    Emitters[i].Clear();
                }
            }
        }

        public void AddEmitter(Vector2 SecPerSpawn, Vector2 SpawnDirection, Vector2 SpawnNoiseAngle, Vector2 StartLife, Vector2 StartScale,
                    Vector2 EndScale, Color StartColor1, Color StartColor2, Color EndColor1, Color EndColor2, Vector2 StartSpeed,
                    Vector2 EndSpeed, int Budget, Vector2 RelPosition, Texture2D ParticleSprite)
        {
            Emitter emitter = new Emitter(SecPerSpawn, SpawnDirection, SpawnNoiseAngle,
                                        StartLife, StartScale, EndScale, StartColor1,
                                        StartColor2, EndColor1, EndColor2, StartSpeed,
                                        EndSpeed, Budget, RelPosition, ParticleSprite, this.random, this);
            Emitters.Add(emitter);
        }
    }
}