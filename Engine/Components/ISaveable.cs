using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ComponentKit;

namespace Calcifer.Engine.Components
{
    public interface ISaveable: IComponent
    {
        void SaveState(BinaryWriter writer);
        void RestoreState(BinaryReader reader);
    }
}
