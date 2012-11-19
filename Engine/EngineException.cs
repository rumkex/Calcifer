using System;

namespace Calcifer.Engine
{
    class EngineException: Exception
    {
        public EngineException(string message): base(message)
        {}
    }
}
