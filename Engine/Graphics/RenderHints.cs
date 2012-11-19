using System.Collections.Generic;

namespace Calcifer.Engine.Graphics
{
    public static class RenderHints<T> where T: struct
    {
        static RenderHints()
        {
            values = new Dictionary<string, T>();
        }

        private static Dictionary<string, T> values;

        public static void SetHint(string key, T value)
        {
            if (!values.ContainsKey(key))
                values.Add(key, value);
            else values[key] = value;
        }

        public static T GetHint(string key)
        {
            T result;
            return values.TryGetValue(key, out result) ? result : default(T);
        }

    }
}
