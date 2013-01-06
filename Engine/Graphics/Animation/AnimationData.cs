using System.Collections.Generic;
using Calcifer.Engine.Content;

namespace Calcifer.Engine.Graphics.Animation
{
    public class AnimationData : IResource
    {
        public string Name { get; private set; }
        public float Speed { get; private set; } // Frames per second
        public List<Pose> Frames { get; private set; }
        public int BoneCount { get; private set; }
        public AnimationData(string name, float speed, IEnumerable<Pose> anim): this(name, speed, new List<Pose>())
        {
            foreach (var frame in anim)
                Frames.Add(frame);
            BoneCount = Frames[0].BoneCount;
        }

        protected AnimationData(string name, float speed, List<Pose> frames)
        {
            Name = name;
            Speed = speed;
            Frames = frames;
        }

	    public object Clone()
	    {
			return new AnimationData(Name, Speed, Frames) {BoneCount = BoneCount};
	    }
    }
}
