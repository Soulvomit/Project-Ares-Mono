using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace TileEngine.ParticleSys
{
    public class Emitter
    {
        public Vector2 RelPosition;                     //position relative to collection
        public int Budget;                              //max number of alive particles
        private float nextSpawnIn;                      //this is a random number generated using the SecPerSpawn
        private float secPassed;                        //time pased since last spawn
        private LinkedList<Particle> activeParticles;   //a list of all the active particles
        public Texture2D ParticleSprite;                //this is what the particle looks like
        public Random Random;                           //pointer to a random object passed trough constructor

        public Vector2 SecPerSpawn;
        public Vector2 SpawnDirection;
        public Vector2 SpawnNoiseAngle;
        public Vector2 StartLife;
        public Vector2 StartScale;
        public Vector2 EndScale;
        public Color StartColor1;
        public Color StartColor2;
        public Color EndColor1;
        public Color EndColor2;
        public Vector2 StartSpeed;
        public Vector2 EndSpeed;
        public bool StopEmmiting;

        public ParticleSystem Parent;

        public Emitter(Vector2 SecPerSpawn, Vector2 SpawnDirection, Vector2 SpawnNoiseAngle, Vector2 StartLife, Vector2 StartScale,
                    Vector2 EndScale, Color StartColor1, Color StartColor2, Color EndColor1, Color EndColor2, Vector2 StartSpeed,
                    Vector2 EndSpeed, int Budget, Vector2 RelPosition, Texture2D ParticleSprite, Random random, ParticleSystem parent)
        {
            this.SecPerSpawn = SecPerSpawn;
            this.SpawnDirection = SpawnDirection;
            this.SpawnNoiseAngle = SpawnNoiseAngle;
            this.StartLife = StartLife;
            this.StartScale = StartScale;
            this.EndScale = EndScale;
            this.StartColor1 = StartColor1;
            this.StartColor2 = StartColor2;
            this.EndColor1 = EndColor1;
            this.EndColor2 = EndColor2;
            this.StartSpeed = StartSpeed;
            this.EndSpeed = EndSpeed;
            this.Budget = Budget;
            this.RelPosition = RelPosition;
            this.ParticleSprite = ParticleSprite;
            this.Random = random;
            this.Parent = parent;
            this.activeParticles = new LinkedList<Particle>();
            this.nextSpawnIn = Helper.LinearInterpolate(SecPerSpawn.X, SecPerSpawn.Y, random.NextDouble());
            this.secPassed = 0.0f;
            this.StopEmmiting = false;
        }

        public void Update(float dt)
        {
            if (StopEmmiting && activeParticles.Count == 0)
            {
                return;
            }
            secPassed += dt;
            while (secPassed > nextSpawnIn)
            {
                if (activeParticles.Count < Budget)
                {
                    // Spawn a particle
                    Vector2 StartDirection = Vector2.Transform(SpawnDirection, Matrix.CreateRotationZ(Helper.LinearInterpolate(SpawnNoiseAngle.X, SpawnNoiseAngle.Y, Random.NextDouble())));
                    StartDirection.Normalize();
                    Vector2 EndDirection = StartDirection * Helper.LinearInterpolate(EndSpeed.X, EndSpeed.Y, Random.NextDouble());
                    StartDirection *= Helper.LinearInterpolate(StartSpeed.X, StartSpeed.Y, Random.NextDouble());
                    if (!StopEmmiting)
                    {
                        activeParticles.AddLast(new Particle(
                            RelPosition + Helper.LinearInterpolate(Parent.LastPos, Parent.Position, secPassed / dt),
                            StartDirection,
                            EndDirection,
                            Helper.LinearInterpolate(StartLife.X, StartLife.Y, Random.NextDouble()),
                            Helper.LinearInterpolate(StartScale.X, StartScale.Y, Random.NextDouble()),
                            Helper.LinearInterpolate(EndScale.X, EndScale.Y, Random.NextDouble()),
                            Helper.LinearInterpolate(StartColor1, StartColor2, Random.NextDouble()),
                            Helper.LinearInterpolate(EndColor1, EndColor2, Random.NextDouble()),
                            this)
                        );
                    }
                    activeParticles.Last.Value.Update(secPassed);
                }
                secPassed -= nextSpawnIn;
                nextSpawnIn = Helper.LinearInterpolate(SecPerSpawn.X, SecPerSpawn.Y, Random.NextDouble());
            }

            LinkedListNode<Particle> node = activeParticles.First;
            while (node != null)
            {
                bool isAlive = node.Value.Update(dt);
                node = node.Next;
                if (!isAlive)
                {
                    if (node == null)
                    {
                        activeParticles.RemoveLast();
                    }
                    else
                    {
                        activeParticles.Remove(node.Previous);
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, int Scale, Vector2 Offset)
        {
            LinkedListNode<Particle> node = activeParticles.First;
            while (node != null)
            {
                node.Value.Draw(spriteBatch, Scale, Offset);
                node = node.Next;
            }
        }

        public void Clear()
        {
            activeParticles.Clear();
        }
    }
}