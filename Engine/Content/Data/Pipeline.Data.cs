using System.Collections.Generic;

namespace Calcifer.Engine.Content.Data
{
    internal class PriorityComparer : IComparer<int>
    {
        public int Compare(int x, int y)
        {
            return (x <= y) ? ((x >= y) ? 0 : 1) : -1;
        }
    }
}