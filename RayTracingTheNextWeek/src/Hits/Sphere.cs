using System.Numerics;
using RayTracingTheNextWeek.Materials;
using RayTracingTheNextWeek.Common;

namespace RayTracingTheNextWeek.Hits
{
    internal class Sphere : Hittable
    {
        private readonly Vector3 center;
        private readonly float radius;
        private readonly Material material;

        private bool isMoving;
        private Vector3 centerVector;

        public override AABB BoundingBox { get; set; }

        public Sphere(Vector3 center, float radius, Material material)
        {
            this.center = center;
            this.radius = radius;
            this.material = material;
            isMoving = false;

            var rv = new Vector3(radius);
            BoundingBox = new AABB(center - rv, center + rv);
        }

        public Sphere(Vector3 center1, Vector3 center2, float radius, Material material)
        {
            center = center1;
            this.radius = radius;
            this.material = material;
            isMoving = true;
            centerVector = center2 - center1;

            var rv = new Vector3(radius);
            var box1 = new AABB(center1 - rv, center1 + rv);
            var box2 = new AABB(center2 - rv, center2 + rv);
            BoundingBox = new AABB(box1, box2);
        }

        public override bool Hit(in Ray ray, Interval<float> t, ref HitRecord rec)
        {
            var trueCenter = isMoving ? CenterAtTime(ray.Time) : center;

            var oc = ray.Origin - trueCenter;
            var a = Vector3.Dot(ray.Direction, ray.Direction);
            var b = Vector3.Dot(oc, ray.Direction);
            var c = Vector3.Dot(oc, oc) - radius * radius;
            
            var discriminant = b * b - a * c;
            if (discriminant < 0)
            {
                return false;
            }

            var sqrtd = MathF.Sqrt(discriminant);
            var root = (-b - sqrtd) / a;
            if (!t.Surrounds(root))
            {
                root = (-b + sqrtd) / a;
                if (!t.Surrounds(root))
                {
                    return false;
                }
            }

            rec.T = root;
            rec.P = ray.At(rec.T);
            var outwardNormal = (rec.P - trueCenter) / radius;
            rec.SetFaceNormal(ray, outwardNormal);
            (rec.U, rec.V) = GetSphereUV(outwardNormal);
            rec.Material = material;

            return true;
        }

        private Vector3 CenterAtTime(float time)
        {
            return center + time * centerVector;
        }

        private (float, float) GetSphereUV(in Vector3 p)
        {
            var theta = MathF.Acos(-p.Y);
            var phi = MathF.Atan2(-p.Z, p.X) + MathF.PI;

            var u = phi / (2 * MathF.PI);
            var v = theta / MathF.PI;

            return (u, v);
        }
    }
}
