using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Calcifer.Engine.Graphics;
using Calcifer.Engine.Graphics.Buffers;
using OpenTK.Graphics.OpenGL;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;

namespace Calcifer.Engine.Content.Pipeline
{
    public class GdiTextureLoader: ResourceLoader<Texture>
    {
        private readonly float maxAnisotropic;

        public GdiTextureLoader(ContentManager parent) : base(parent)
        {
            GL.GetFloat((GetPName)ExtTextureFilterAnisotropic.MaxTextureMaxAnisotropyExt, out maxAnisotropic);
        }

        public override Texture Load(string name, Stream stream)
        {
            var bmap = new Bitmap(stream);
            var rect = new Rectangle(0, 0, bmap.Width, bmap.Height);
            var bmapdata = bmap.LockBits(rect, ImageLockMode.ReadOnly,
                                         System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            if (bmapdata.Stride > 0)
            {
                // OpenGL hates top-down bitmaps, so we flip it.
                bmap.UnlockBits(bmapdata);
                bmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
                bmapdata = bmap.LockBits(rect, ImageLockMode.ReadOnly,
                              System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            }
            var texture = new Texture2D(name);
            texture.Bind();
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS,
                            (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT,
                            (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                            (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                            (int)TextureMagFilter.Linear);
            var anisotropicHint = RenderHints<float>.GetHint("AnisotropicFiltering");
            GL.TexParameter(TextureTarget.Texture2D, (TextureParameterName) ExtTextureFilterAnisotropic.TextureMaxAnisotropyExt,
                            anisotropicHint > maxAnisotropic ? maxAnisotropic : anisotropicHint);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.CompressedRgba,
                              bmapdata.Width, bmapdata.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte,
                              bmapdata.Scan0);
            bmap.UnlockBits(bmapdata);
            return texture;
        }

        public override bool Supports(string name, Stream stream)
        {
            if (Path.GetExtension(name) == ".png")
            {
                // TODO: When APNG support is implemented, drop PNG files in favor of APNG loader.
            }
            return true;
        }
    }
}
