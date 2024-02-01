using System.Numerics;
using RayTracingTheNextWeek.Common;

namespace RayTracingTheNextWeek.Hits
{
    internal class Translate : Hittable
    {
        private readonly Hittable obj;
        private readonly Vector3 offset;

        public override AABB BoundingBox { get; set; }

        public Translate(Hittable obj, Vector3 offset)
        {
            this.obj = obj;
            this.offset = offset;
            BoundingBox = obj.BoundingBox + offset;
        }

        public override bool Hit(in Ray ray, Interval<float> t, ref HitRecord rec)
        {
            var movedRay = new Ray(ray.Origin - offset, ray.Direction, ray.Time);
            if (!obj.Hit(movedRay, t, ref rec))
            {
                return false;
            }

            rec.P += offset;

            return true;
        }
    }

    internal class RotateY : Hittable
    {
        private readonly Hittable obj;
        private readonly float sinTheta;
        private readonly float cosTheta;

        public override AABB BoundingBox { get; set; }

        public RotateY(Hittable obj, float angle)
        {
            this.obj = obj;
            var radians = Utilities.DegreesToRadians(angle);
            sinTheta = MathF.Sin(radians);
            cosTheta = MathF.Cos(radians);
            
            BoundingBox = obj.BoundingBox;

            var min = new Vector3(float.PositiveInfinity);
            var max = new Vector3(float.NegativeInfinity);

            for (var i = 0; i < 2; i++)
            {
                for (var j = 0; j < 2; j++)
                {
                    for (var k = 0; k < 2; k++)
                    {
                        var x = i * BoundingBox.X.Max + (1 - i) * BoundingBox.X.Min;
                        var y = j * BoundingBox.Y.Max + (1 - j) * BoundingBox.Y.Min;
                        var z = k * BoundingBox.Z.Max + (1 - k) * BoundingBox.Z.Min;

                        var newX = cosTheta * x + sinTheta * z;
                        var newZ = -sinTheta * x + cosTheta * z;

                        var tester = new Vector3(newX, y, newZ);

                        for (var c = 0; c < 3; c++)
                        {
                            min[c] = MathF.Min(min[c], tester[c]);
                            max[c] = MathF.Max(max[c], tester[c]);
                        }
                    }
                }
            }

            BoundingBox = new AABB(min, max);
        }

        public override bool Hit(in Ray ray, Interval<float> t, ref HitRecord rec)
        {
            var origin = ray.Origin;
            var direction = ray.Direction;

            origin.X = cosTheta * ray.Origin.X - sinTheta * ray.Origin.Z;
            origin.Z = sinTheta * ray.Origin.X + cosTheta * ray.Origin.Z;

            direction.X = cosTheta * ray.Direction.X - sinTheta * ray.Direction.Z;
            direction.Z = sinTheta * ray.Direction.X + cosTheta * ray.Direction.Z;

            var rotatedRay = new Ray(origin, direction, ray.Time);

            if (!obj.Hit(rotatedRay, t, ref rec))
            {
                return false;
            }

            var p = rec.P;
            var normal = rec.Normal;

            p.X = cosTheta * rec.P.X + sinTheta * rec.P.Z;
            p.Z = -sinTheta * rec.P.X + cosTheta * rec.P.Z;

            normal.X = cosTheta * rec.Normal.X + sinTheta * rec.Normal.Z;
            normal.Z = -sinTheta * rec.Normal.X + cosTheta * rec.Normal.Z;

            rec.P = p;
            rec.Normal = normal;

            return true;
        }
    }
}
