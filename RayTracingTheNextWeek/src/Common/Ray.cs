using System.Numerics;

namespace RayTracingTheNextWeek.Common
{
    internal class Ray
    {
        public Vector3 Origin { get; }
        public Vector3 Direction { get; }
        public float Time { get; }

        public Ray() : this(Vector3.Zero, Vector3.Zero, 0f)
        {
        }

        public Ray(Vector3 origin, Vector3 direction, float time)
        {
            Origin = origin;
            Direction = direction;
            Time = time;
        }

        public Ray(Vector3 origin, Vector3 direction) : this(origin, direction, 0f)
        {
        }

        public Vector3 At(float t)
        {
            return Origin + t * Direction;
        }
    }
}
