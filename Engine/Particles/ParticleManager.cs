using System.Collections.Generic;

namespace Calcifer.Engine.Particles
{
    /// <summary>
    /// This class is responsible for managing all particles
    /// </summary>
    public class ParticleManager
    {
        private readonly IRenderer renderer;
        private readonly List<IEmitter> emitters;
        private readonly ParticlePool pool;

        public ParticleManager(IRenderer renderer)
        {
            this.renderer = renderer;
            emitters = new List<IEmitter>();
            pool = new ParticlePool();
        }

        public void AddEmitter(IEmitter emitter)
        {
            emitter.SetPool(pool);
            emitters.Add(emitter);
        }

        public void RemoveEmitter(IEmitter emitter)
        {
            emitters.Remove(emitter);
        }

        public void Update(float time)
        {
            pool.Update(time);
            foreach (var emitter in emitters) emitter.Update(time);
            renderer.Update(pool.Particles);
        }
    }
}
