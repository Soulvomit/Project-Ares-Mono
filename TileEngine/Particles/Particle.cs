using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TileEngine.ParticleSys
{
    class Particle
    {
        public Vector2 Position;
        private Vector2 startDirection;
        private Vector2 endDirection;
        private float lifeLeft;
        private float startingLife;
        private float scaleBegin;
        private float scaleEnd;
        private Color startColor;
        private Color endColor;
        private Emitter parent;
        private float lifePhase;

        public Particle(Vector2 Position, Vector2 StartDirection, Vector2 EndDirection, float StartingLife, float ScaleBegin, float ScaleEnd, Color StartColor, Color EndColor, Emitter Yourself)
        {
            this.Position = Position;
            this.startDirection = StartDirection;
            this.endDirection = EndDirection;
            this.startingLife = StartingLife;
            this.lifeLeft = StartingLife;
            this.scaleBegin = ScaleBegin;
            this.scaleEnd = ScaleEnd;
            this.startColor = StartColor;
            this.endColor = EndColor;
            this.parent = Yourself;
        }

        public bool Update(float dt)
        {
            lifeLeft -= dt;
            if (lifeLeft <= 0)
                return false;
            lifePhase = lifeLeft / startingLife;      // 1 means newly created 0 means dead.
            Position += Helper.LinearInterpolate(endDirection, startDirection, lifePhase) * dt;
            return true;
        }

        public void Draw(SpriteBatch spriteBatch, int Scale, Vector2 Offset)
        {
            float currScale = Helper.LinearInterpolate(scaleEnd, scaleBegin, lifePhase);
            Color currCol = Helper.LinearInterpolate(endColor, startColor, lifePhase);
            spriteBatch.Draw(
                parent.ParticleSprite,
                new Rectangle(
                    (int)((Position.X - 0.5f * currScale) * Scale + Offset.X),
                    (int)((Position.Y - 0.5f * currScale) * Scale + Offset.Y),
                    (int)(currScale * Scale),
                    (int)(currScale * Scale)),
                null,
                currCol,
                0,
                Vector2.Zero,
                SpriteEffects.None,
                0);
        }
    }
}