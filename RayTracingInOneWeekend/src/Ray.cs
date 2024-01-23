using System.Numerics;

namespace RayTracingInOneWeekend
{
    internal class Ray
    {
        public Vector3 Origin { get; }
        public Vector3 Direction { get; }

        public Ray(in Vector3 origin, in Vector3 direction)
        {
            Origin = origin;
            Direction = direction;
        }

        public Vector3 At(float t)
        {
            return Origin + t * Direction;
        }
    }
}
