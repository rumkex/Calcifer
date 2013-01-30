namespace Calcifer.Engine.Particles
{
    public interface IEmitter
    {
        void SetPool(ParticlePool p);
        void Update(float time);
    }
}