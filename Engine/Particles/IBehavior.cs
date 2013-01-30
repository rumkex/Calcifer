namespace Calcifer.Engine.Particles
{
    /// <summary>
    /// This class provides custom update instructions for particles
    /// and defines their look
    /// </summary>
    public interface IBehavior
    {
        void Update(Particle p, float time);
    }
}