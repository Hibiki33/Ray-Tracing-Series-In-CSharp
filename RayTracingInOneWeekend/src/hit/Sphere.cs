using System.Numerics;
using RayTracingInOneWeekend.Material;

namespace RayTracingInOneWeekend.Hit
{
    internal class Sphere : IHittable
    {
        private readonly Vector3 _center;
        private readonly float _radius;
        private readonly IMaterial _material;

        public Sphere(Vector3 center, float radius, IMaterial material)
        {
            _center = center;
            _radius = radius;
            _material = material;
        }

        public bool Hit(Ray ray, Interval<float> t, out HitRecord? rec)
        {
            var oc = ray.Origin - _center;
            var a = Vector3.Dot(ray.Direction, ray.Direction);
            var b = Vector3.Dot(oc, ray.Direction);
            var c = Vector3.Dot(oc, oc) - _radius * _radius;
            
            var discriminant = b * b - a * c;
            if (discriminant < 0)
            {
                rec = default;
                return false;
            }

            var sqrtd = MathF.Sqrt(discriminant);
            var root = (-b - sqrtd) / a;
            if (!t.Surrounds(root))
            {
                root = (-b + sqrtd) / a;
                if (!t.Surrounds(root))
                {
                    rec = default;
                    return false;
                }
            }

            rec = new HitRecord();
            rec.T = root;
            rec.P = ray.At(rec.T);
            var outwardNormal = (rec.P - _center) / _radius;
            rec.SetFaceNormal(ray, outwardNormal);
            rec.Material = _material;

            return true;
        }
    }
}
