using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Calcifer.Engine.Graphics
{
    public class Shader: IDisposable
    {
        public int ID { get; private set; }
        public int VertexHandle, FragmentHandle;

        private Dictionary<string, int> attribCache, uniformCache;

		public static Shader Current { get; private set; }		
		
        public Shader()
        {
            VertexHandle = GL.CreateShader(ShaderType.VertexShader);
            FragmentHandle = GL.CreateShader(ShaderType.FragmentShader);
            attribCache = new Dictionary<string, int>();
            uniformCache = new Dictionary<string, int>();
            ID = GL.CreateProgram();
        }

        public void Enable()
		{
			Current = this;
            GL.UseProgram(ID);
        }

        public void Disable()
        {
            GL.UseProgram(0);
        }

        public int GetUniformLocation(string var)
        {
            if (!uniformCache.ContainsKey(var)) uniformCache.Add(var, GL.GetUniformLocation(ID, var));
            return uniformCache[var];
        }

        public int GetAttribLocation(string var)
        {
            if (!attribCache.ContainsKey(var)) attribCache.Add(var, GL.GetAttribLocation(ID, var));
            return attribCache[var];
        }

        public void SetUniform(string var, int i)
        {
            SetUniform(GetUniformLocation(var), i);
        }

        public void SetUniform(string var, float f)
        {
            SetUniform(GetUniformLocation(var), f);
        }

        public void SetUniform(string var, Vector3 f)
        {
            SetUniform(GetUniformLocation(var), f);
        }

        public void SetUniform(string var, Vector4 f)
        {
            SetUniform(GetUniformLocation(var), f);
        }

        public void SetUniform(string var, ref Matrix4 f)
        {
            SetUniform(GetUniformLocation(var), ref f);
        }

        public void SetUniform(int loc, int i)
        {
            GL.Uniform1(loc, i);
        }

        public void SetUniform(int loc, float f)
        {
            GL.Uniform1(loc, f);
        }

        public void SetUniform(int loc, Vector3 f)
        {
            GL.Uniform3(loc, f);
        }

        public void SetUniform(int loc, Vector4 f)
        {
            GL.Uniform4(loc, f);
        }

        public void SetUniform(int loc, ref Matrix4 f)
        {
            GL.UniformMatrix4(loc, true, ref f);
        }

        public void BindAttribLocation(int loc, string name)
        {
            Enable();
            GL.BindAttribLocation(ID, loc, name);
        }

        public void BindFragDataLocation(int loc, string name)
        {
            Enable();
            GL.BindFragDataLocation(ID, loc, name);
        }

        public void Link()
        {
            GL.LinkProgram(ID);
            uniformCache.Clear();
            attribCache.Clear();
        }

        public override string ToString()
        {
            return "Shader #" + ID;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        ~Shader()
        {
            Dispose(false);
        }

        protected void Dispose(bool manual)
        {
            if (!manual) return;
            GL.DetachShader(ID, FragmentHandle);
            GL.DetachShader(ID, VertexHandle);
            GL.DeleteShader(FragmentHandle);
            GL.DeleteShader(VertexHandle);
            GL.DeleteProgram(ID);
        }
    }
}
