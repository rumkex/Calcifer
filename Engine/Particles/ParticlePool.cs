using System;
using System.Collections.Generic;

namespace Calcifer.Engine.Particles
{
    public class ParticlePool
    {
        private readonly List<Particle> pool;
        private readonly Queue<int> freeIds;
        private readonly int initialSize;

        public ParticlePool() : this(512)
        {}

        public ParticlePool(int size)
        {
            initialSize = size;
            freeIds = new Queue<int>();
            pool = new List<Particle>(size);
            for (var i = 0; i < size; i++)
            {
                pool.Add(new Particle(this, i));
                freeIds.Enqueue(i);
            }
        }

        internal IList<Particle> Particles
        {
            get { return pool; }
        }

        public Particle GetNew()
        {
            if (freeIds.Count == 0)
            {
                // Extending pool
                pool.Capacity += pool.Count/2 + 1;
                var extendSize = pool.Capacity - pool.Count;
                for (var n = 0; n < extendSize; n++)
                {
                    var p = new Particle(this, pool.Count);
                    pool.Add(p);
                    freeIds.Enqueue(p.ID);
                }
#if DEBUG
                Console.WriteLine("Particle pool upsized to {0}", pool.Capacity);
#endif
            }
            var index = freeIds.Dequeue();
            pool[index].Destroyed = false;
            return pool[index];
        }

        internal void Destroy(Particle particle)
        {
            particle.Destroyed = true;
            freeIds.Enqueue(particle.ID);
        }

        internal void Update(float time)
        {
            foreach (var p in pool) p.Update(time);
        }
    }
}