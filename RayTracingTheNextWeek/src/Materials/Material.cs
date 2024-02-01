using System.Numerics;
using RayTracingTheNextWeek.Hits;
using RayTracingTheNextWeek.Common;

namespace RayTracingTheNextWeek.Materials
{
    internal abstract class Material
    {
        public virtual Color Emitted(float u, float v, in Vector3 p)
        {
            return Color.Black;
        }

        public abstract bool Scatter(in Ray ray, in HitRecord rec, out Color? attenuation, out Ray? scattered);
    }
}
