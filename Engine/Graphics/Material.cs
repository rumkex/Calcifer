using System;
using Calcifer.Engine.Graphics.Buffers;
using OpenTK.Graphics;

namespace Calcifer.Engine.Graphics
{
    public class Material: IDisposable
    {
        public string Name;
        public Color4 Ambient, Diffuse, Specular;
        public float Shininess, Refraction;
        public Texture DiffuseMap, SpecularMap, NormalMap;

        private static Material deflt;

        public Material(string name)
        {
            Name = name;
        }

        public static Material DefaultMaterial
        {
            get
            {
                return deflt ?? (deflt = new Material("default")
                                             {
                                                 Ambient = new Color4(0.3f, 0.3f, 0.3f, 1.0f),
                                                 Diffuse = new Color4(0.5f, 0.5f, 0.5f, 1.0f),
                                                 Specular = new Color4(0.6f, 0.6f, 0.6f, 1.0f),
                                                 Shininess = 50.0f
                                             });
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool manual)
        {
            if (!manual) return;
            if (DiffuseMap != null) DiffuseMap.Dispose();
        }

        ~Material()
        {
            Dispose(false);
        }
    }
}
