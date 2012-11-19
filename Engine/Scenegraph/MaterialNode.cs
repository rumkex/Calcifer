using Calcifer.Engine.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Calcifer.Engine.Scenegraph
{
    public class MaterialNode: SceneNode
    {
        protected Material Material { get; private set; }

        public MaterialNode(SceneNode parent, Material material) : base(parent)
        {
            Material = material;
        }

        public override void AcceptPass(RenderPass pass)
        {
            pass.Visit(this);
        }

        public override void BeginRender()
        {
            GL.Material(MaterialFace.Front, MaterialParameter.Diffuse, Material.Diffuse);
            GL.Material(MaterialFace.Front, MaterialParameter.Specular, Material.Specular);
            GL.Material(MaterialFace.Front, MaterialParameter.Ambient, Material.Ambient);
            GL.Material(MaterialFace.Front, MaterialParameter.Shininess, Material.Shininess);
            if (Material.DiffuseMap != null)
            {
                GL.ActiveTexture(TextureUnit.Texture0);
                Material.DiffuseMap.Bind();
            }
            if (Material.SpecularMap != null)
            {
                GL.ActiveTexture(TextureUnit.Texture1);
                Material.SpecularMap.Bind();
            }
            if (Material.NormalMap != null)
            {
                GL.ActiveTexture(TextureUnit.Texture6);
                Material.NormalMap.Bind();
            }
        }

        public override void EndRender()
        {
            Material.DiffuseMap.Unbind();
        }
    }
}
