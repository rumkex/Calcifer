using OpenTK;

namespace Calcifer.Engine.Particles
{
    public class Particle
    {
        private readonly ParticlePool owner;
        internal readonly int ID;
        internal bool Destroyed;
        public Vector3 Position, Velocity, Force;
        public float Rotation, AngularVelocity, TTL;

        public bool IsActive { get { return TTL > 0; } }
        public IBehavior Behavior { get; internal set; }

        internal Particle(ParticlePool owner, int id)
        {
            Destroyed = true;
            this.owner = owner;
            ID = id;
        }

        public void Update(float time)
        {
            if (Destroyed) return;
            if (IsActive)
            {
                TTL -= time;
                Behavior.Update(this, time);
                Vector3 temp;
                Vector3.Multiply(ref Force, time, out temp);
                Vector3.Add(ref Velocity, ref temp, out Velocity);
                Vector3.Multiply(ref Velocity, time, out temp);
                Vector3.Add(ref Position, ref temp, out Position);
                Rotation += time * AngularVelocity;
            }
            else 
                Destroy();
        }

        public void Destroy()
        {
            owner.Destroy(this);
        }
    }
}
