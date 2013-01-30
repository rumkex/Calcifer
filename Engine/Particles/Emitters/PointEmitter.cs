using System;
using OpenTK;

namespace Calcifer.Engine.Particles.Emitters
{
    /// <summary>
    /// Basic point emitter, spreads particles randomly
    /// </summary>
    public class PointEmitter : IEmitter
    {
        private class Behavior : IBehavior
        {
            private PointEmitter parent;

            public Behavior(PointEmitter parent)
            {
                this.parent = parent;
            }


            public void Update(Particle p, float time)
            {
                p.Force = parent.Position - p.Position;
            }
        }

        private Random random = new Random();
        private IBehavior behavior;

        /// <summary>
        /// Particles lifetime
        /// </summary>
        public float Lifetime { get; set; }
        /// <summary>
        /// Particles per second output of this emitter
        /// </summary>
        public float Intensity { get; set; }
        /// <summary>
        /// Maximum particle velocity
        /// </summary>
        public float MaxVelocity { get; set; }
        /// <summary>
        /// Emitter position
        /// </summary>
        public Vector3 Position { get; set; }

        private ParticlePool pool;

        public void SetPool(ParticlePool p)
        {
            pool = p;
        }

        private float cachedTime;

        public PointEmitter()
        {
            behavior = new Behavior(this);
        }

        public void Update(float time)
        {
            cachedTime += time;
            var genCount = (int) (cachedTime*Intensity);
            if (genCount <= 0) return;
            cachedTime -= genCount/Intensity;

            for (var i = 0; i < genCount; i++)
            {
                var p = pool.GetNew();
                p.Position = Position;
                p.Behavior = behavior;
                p.TTL = (float) (Lifetime * (1f - 0.5f * random.NextDouble()));
                var phi = random.NextDouble() * 2f * Math.PI;
                var rho = (float)(1f - 0.5f * random.NextDouble()) * MaxVelocity;
                p.Velocity = new Vector3((float)Math.Sin(phi), (float)Math.Cos(phi), 0f) * rho;
                p.Rotation = 0f;
                p.AngularVelocity = 0f;
            }
        }
    }
}
