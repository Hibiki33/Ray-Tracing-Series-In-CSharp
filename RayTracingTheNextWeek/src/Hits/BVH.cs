using RayTracingTheNextWeek.Common;
using System.Diagnostics;

namespace RayTracingTheNextWeek.Hits
{
    // Bounding Volume Hierarchy
    internal class BVHNode : Hittable
    {
        private Hittable left;
        private Hittable right;

        public override AABB BoundingBox { get; set; }

        public BVHNode(HittableList srcObjects) : this(srcObjects.Objects, 0, srcObjects.Objects.Count)
        {
        }

        public BVHNode(IEnumerable<Hittable> srcObjects, int start, int end)
        {
            var objects = srcObjects.ToList();

            var axis = Utilities.RandomInt(0, 3);
            Comparison<Hittable> comparator = axis switch
            {
                0 => BoxXCompare,
                1 => BoxYCompare,
                _ => BoxZCompare
            };

            var objectSpan = end - start;

            if (objectSpan == 1)
            {
                left = right = objects[start];
            }
            else if (objectSpan == 2)
            {
                if (comparator(objects[start], objects[start + 1]) < 0)
                {
                    left = objects[start];
                    right = objects[start + 1];
                }
                else
                {
                    left = objects[start + 1];
                    right = objects[start];
                }
            }
            else
            {
                objects.Sort(start, objectSpan, Comparer<Hittable>.Create(comparator));

                var mid = start + objectSpan / 2;
                left = new BVHNode(objects, start, mid);
                right = new BVHNode(objects, mid, end);
            }

            BoundingBox = new AABB(left.BoundingBox, right.BoundingBox);
        }

        public override bool Hit(in Ray ray, Interval<float> t, ref HitRecord rec)
        {
            if (!BoundingBox.Hit(ray, t))
            {
                return false;
            }

            var hitLeft = left.Hit(ray, t, ref rec);
            var hitRight = right.Hit(ray, new(t.Min, hitLeft ? rec.T : t.Max), ref rec);

            return hitLeft || hitRight;
        }

        private static int BoxCompare(Hittable a, Hittable b, int axis)
        {
            var boxA = a.BoundingBox;
            var boxB = b.BoundingBox;

            return boxA.GetElement(axis).Min.CompareTo(boxB.GetElement(axis).Min);
        }

        private static int BoxXCompare(Hittable a, Hittable b)
        {
            return BoxCompare(a, b, 0);
        }

        private static int BoxYCompare(Hittable a, Hittable b)
        {
            return BoxCompare(a, b, 1);
        }

        private static int BoxZCompare(Hittable a, Hittable b)
        {
            return BoxCompare(a, b, 2);
        }
    }
}
