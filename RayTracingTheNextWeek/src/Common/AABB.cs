using System.Numerics;

namespace RayTracingTheNextWeek.Common
{
    using IntervalF = Interval<float>;

    internal class AABB // Axis-aligned bounding box
    {
        public IntervalF X { get; }
        public IntervalF Y { get; }
        public IntervalF Z { get; }

        public AABB()
        {
            X = Y = Z = new IntervalF(float.PositiveInfinity, float.NegativeInfinity);
        }

        public AABB(IntervalF x, IntervalF y, IntervalF z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public AABB(in Vector3 a, in Vector3 b) :
            this(new IntervalF(MathF.Min(a.X, b.X), MathF.Max(a.X, b.X)),
                 new IntervalF(MathF.Min(a.Y, b.Y), MathF.Max(a.Y, b.Y)),
                 new IntervalF(MathF.Min(a.Z, b.Z), MathF.Max(a.Z, b.Z)))
        {
        }

        public AABB(AABB box0, AABB box1) :
            this(new IntervalF(box0.X, box1.X),
                 new IntervalF(box0.Y, box1.Y),
                 new IntervalF(box0.Z, box1.Z))
        {
        }

        public AABB Pad()
        {
            const float padding = 0.0001f;
            return new AABB(
                X.Size() >= padding ? X : X.Expand(padding),
                Y.Size() >= padding ? Y : Y.Expand(padding),
                Z.Size() >= padding ? Z : Z.Expand(padding));
        }

        public IntervalF GetElement(int i)
        {
            return i switch
            {
                0 => X,
                1 => Y,
                2 => Z,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public bool Hit(in Ray ray, IntervalF t)
        {
            for (var i = 0; i < 3; ++i)
            {
                var invD = 1.0f / ray.Direction.GetElement(i);
                var orig = ray.Origin.GetElement(i);

                var t0 = (GetElement(i).Min - orig) * invD;
                var t1 = (GetElement(i).Max - orig) * invD;

                if (invD < 0.0f)
                {
                    var tmp = t0;
                    t0 = t1;
                    t1 = tmp;
                }

                if (t0 > t.Min)
                {
                    t.Min = t0;
                }
                if (t1 < t.Max)
                {
                    t.Max = t1;
                }

                if (t.Max <= t.Min)
                {
                    return false;
                }
            }

            return true;
        }

        public static AABB operator +(in AABB box, in Vector3 offset)
        {
            return new AABB(box.X + offset.X, box.Y + offset.Y, box.Z + offset.Z);
        }

        public static AABB operator +(in Vector3 offset, in AABB box)
        {
            return box + offset;
        }

        public override string ToString()
        {
            return $"X: {X}, Y: {Y}, Z: {Z}";
        }
    }
}
