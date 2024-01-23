namespace RayTracingInOneWeekend.Hit
{
    internal interface IHittable
    {
        bool Hit(Ray ray, Interval<float> t, out HitRecord? rec);
    }
}
