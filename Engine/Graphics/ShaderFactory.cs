using System.IO;
using OpenTK.Graphics.OpenGL;

namespace Calcifer.Engine.Graphics
{
    public static class ShaderFactory
    {

        public static Shader Create(Stream vert, Stream frag)
        {
            var vshader = new StreamReader(vert);
            var fshader = new StreamReader(frag);
            return Create(vshader.ReadToEnd(), fshader.ReadToEnd());
        }

        public static Shader Create(string vert, string frag)
        {
            var shader = Compile(vert, frag);
            Validate(shader);
            return shader;
        }

        private static void Validate(Shader shader)
        {
            int result;
            GL.GetProgram(shader.ID, ProgramParameter.ValidateStatus, out result);
            if (result == 1) return;
            string slog, vlog, flog;
            GL.GetProgramInfoLog(shader.ID, out slog);
            GL.GetShaderInfoLog(shader.VertexHandle, out vlog);
            GL.GetShaderInfoLog(shader.FragmentHandle, out flog);
            throw new EngineException(string.Format("Shader linking failed: {0}\n{1}\n{2}", slog, vlog, flog));
        }

        private static Shader Compile(string vsData, string fsData)
        {
            var shader = new Shader();
            GL.ShaderSource(shader.VertexHandle, vsData);
            GL.ShaderSource(shader.FragmentHandle, fsData);
            GL.CompileShader(shader.VertexHandle);
            GL.CompileShader(shader.FragmentHandle);
            GL.AttachShader(shader.ID, shader.VertexHandle);
            GL.AttachShader(shader.ID, shader.FragmentHandle);
            GL.LinkProgram(shader.ID);
            return shader;
        }
    }
}