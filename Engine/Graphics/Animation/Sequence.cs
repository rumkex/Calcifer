namespace Calcifer.Engine.Graphics.Animation
{
    public abstract class Sequence
    {
        public abstract string Name
        {
            get;
        }
        public abstract float Time
        {
            get;
        }
        public abstract float Speed
        {
            get;
        }
        public abstract float Length
        {
            get;
        }
        public abstract bool Finished
        {
            get;
        }
        public abstract Pose Pose
        {
            get;
        }
        public abstract void Update(float timedelta);
    }
}