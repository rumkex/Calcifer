namespace Calcifer.Engine.Graphics.Animation
{
    public abstract class Sequence
    {
        public abstract string Name { get; }
        public abstract float Time { get; set; }
        public abstract float Speed { get; set; }
        public abstract float Length { get; }
        public abstract bool Finished { get; }
        public abstract Pose Pose { get; }
        public abstract void Update(float timedelta);
    }
}