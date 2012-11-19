using OpenTK;

namespace Calcifer.Engine.Scenegraph
{
    public interface ICamera
    {
        Matrix4 Matrix { get; }
    }

    public class Camera : ICamera
    {
        public Matrix4 Matrix { get; set; }

        public Camera(Matrix4 matrix)
        {
            Matrix = matrix;
        }
    }
}
