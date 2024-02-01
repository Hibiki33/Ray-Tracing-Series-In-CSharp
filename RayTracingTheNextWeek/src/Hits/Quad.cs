using System.Numerics;
using RayTracingTheNextWeek.Common;
using RayTracingTheNextWeek.Hits;
using RayTracingTheNextWeek.Materials;

namespace RayTracingTheNextWeek.src.Hits
{
    internal class Quad : Hittable
    {
        private readonly Vector3 q;
        private readonly Vector3 u;
        private readonly Vector3 v;
        private readonly Material material;

        private readonly Vector3 normal;
        private readonly float d;
        private readonly Vector3 w;

        public override AABB BoundingBox { get; set; }

        public Quad(Vector3 q, Vector3 u, Vector3 v, Material material)
        {
            this.q = q;
            this.u = u;
            this.v = v;
            this.material = material;

            var n = Vector3.Cross(u, v);
            normal = Vector3.Normalize(n);
            d = Vector3.Dot(normal, q);
            w = n / Vector3.Dot(n, n);

            BoundingBox = new AABB(q, q + u + v).Pad();
        }

        public override bool Hit(in Ray ray, Interval<float> t, ref HitRecord rec)
        {
            var denom = Vector3.Dot(normal, ray.Direction);
            if (MathF.Abs(denom) < 1e-8f)
            {
                return false;
            }

            var tHit = (d - Vector3.Dot(normal, ray.Origin)) / denom;
            if (!t.Contains(tHit))
            {
                return false;
            }

            var intersection = ray.At(tHit);
            var planarHitptVec = intersection - q;
            var alpha = Vector3.Dot(w, Vector3.Cross(planarHitptVec, v));
            var beta = Vector3.Dot(w, Vector3.Cross(u, planarHitptVec));

            if (!IsInterior(alpha, beta, ref rec))
            {
                return false;
            }

            rec.T = tHit;
            rec.P = intersection;
            rec.Material = material;
            rec.SetFaceNormal(ray, normal);

            return true;
        }

        public void SetBoundingBox()
        {
            BoundingBox = new AABB(q, q + u + v).Pad();
        }

        public bool IsInterior(float a, float b, ref HitRecord rec)
        {
            if (a < 0 || a > 1 || b < 0 || b > 1)
            {
                return false;
            }

            rec.U = a;
            rec.V = b;
            return true;
        }

        public static HittableList Box(in Vector3 a, in Vector3 b, Material material)
        {
            var sides = new HittableList();

            var min = Vector3.Min(a, b);
            var max = Vector3.Max(a, b);

            var dx = Vector3.UnitX * (max.X - min.X);
            var dy = Vector3.UnitY * (max.Y - min.Y);
            var dz = Vector3.UnitZ * (max.Z - min.Z);

            sides.Add(new Quad(new Vector3(min.X, min.Y, max.Z),  dx,  dy, material));
            sides.Add(new Quad(new Vector3(max.X, min.Y, max.Z), -dz,  dy, material));
            sides.Add(new Quad(new Vector3(max.X, min.Y, min.Z), -dx,  dy, material));
            sides.Add(new Quad(new Vector3(min.X, min.Y, min.Z),  dz,  dy, material));
            sides.Add(new Quad(new Vector3(min.X, max.Y, max.Z),  dx, -dz, material));
            sides.Add(new Quad(new Vector3(min.X, min.Y, min.Z),  dx,  dz, material));
        
            return sides;
        }   
    }
}
