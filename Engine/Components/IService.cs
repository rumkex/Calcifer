using System.Collections.Generic;
using ComponentKit;

namespace Calcifer.Engine.Components
{
    public interface IService
    {
        void Synchronize(IEnumerable<IComponent> components);
    }
}