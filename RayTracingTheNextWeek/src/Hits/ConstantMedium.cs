using System.Numerics;
using RayTracingTheNextWeek.Common;
using RayTracingTheNextWeek.Materials;
using RayTracingTheNextWeek.Textures;

namespace RayTracingTheNextWeek.Hits
{
    internal class ConstantMedium : Hittable
    {
        private readonly Hittable boundary;
        private readonly float negInvDensity;
        private readonly Material phaseFunction;

        public override AABB BoundingBox { get; set; }

        public ConstantMedium(Hittable boundary, float density, Texture texture)
        {
            this.boundary = boundary;
            negInvDensity = -1.0f / density;
            phaseFunction = new Isotropic(texture);

            BoundingBox = boundary.BoundingBox;
        }

        public ConstantMedium(Hittable boundary, float density, Color color)
        {
            this.boundary = boundary;
            negInvDensity = -1.0f / density;
            phaseFunction = new Isotropic(color);

            BoundingBox = boundary.BoundingBox;
        }

        public override bool Hit(in Ray ray, Interval<float> t, ref HitRecord rec)
        {
            var db = new HitRecord();
            var dc = new HitRecord();

            if (!boundary.Hit(ray, new Interval<float>(float.MinValue, float.MaxValue), ref db))
            {
                return false;
            }

            if (!boundary.Hit(ray, new Interval<float>(db.T + 0.0001f, float.MaxValue), ref dc))
            {
                return false;
            }

            if (db.T < t.Min)
            {
                db.T = t.Min;
            }

            if (dc.T > t.Max)
            {
                dc.T = t.Max;
            }

            if (db.T >= dc.T)
            {
                return false;
            }

            if (db.T < 0)
            {
                db.T = 0;
            }

            var rayLength = ray.Direction.Length();
            var distanceInsideBoundary = (dc.T - db.T) * rayLength;
            var hitDistance = negInvDensity * MathF.Log(Utilities.RandomFloat());

            if (hitDistance > distanceInsideBoundary)
            {
                return false;
            }

            rec.T = db.T + hitDistance / rayLength;
            rec.P = ray.At(rec.T);

            rec.Normal = new Vector3(1, 0, 0); 
            rec.FrontFace = true; 
            rec.Material = phaseFunction;

            return true;
        }
    }
}
