using System.Collections.Generic;

namespace Calcifer.Engine.Particles
{
    public interface IRenderer
    {
        void Update(IList<Particle> particles);
    }
}