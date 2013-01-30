using System;
using OpenTK;

namespace Calcifer.Engine.Particles.Emitters
{
    public class LineEmitter: IEmitter
    {
        private class Behavior: IBehavior
        {
            private LineEmitter parent;

            public Behavior(LineEmitter parent)
            {
                this.parent = parent;
            }

            public void Update(Particle p, float time)
            {
                p.Velocity = parent.MaxVelocity * (1f - p.TTL / parent.Lifetime);
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
        public Vector3 MaxVelocity { get; set; }
        /// <summary>
        /// Emitter line start position
        /// </summary>
        public Vector3 Start { get; set; }
        /// <summary>
        /// Emitter line end position
        /// </summary>
        public Vector3 End { get; set; }

        private ParticlePool pool;

        public void SetPool(ParticlePool p)
        {
            pool = p;
        }

        private float cachedTime;

        public LineEmitter()
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
                p.Position = Start + (float)random.NextDouble() * (End - Start);
                p.Behavior = behavior;
                p.TTL = (float) (Lifetime * (1f - 0.5f * random.NextDouble()));
                p.Rotation = 0f;
                p.AngularVelocity = 0f;
            }
        }
    }
}
