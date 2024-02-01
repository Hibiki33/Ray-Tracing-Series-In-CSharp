using RayTracingTheNextWeek.Common;

namespace RayTracingTheNextWeek.Hits
{
    internal abstract class Hittable
    {
        public abstract bool Hit(in Ray ray, Interval<float> t, ref HitRecord rec);

        public abstract AABB BoundingBox { get; set; }
    }
}
