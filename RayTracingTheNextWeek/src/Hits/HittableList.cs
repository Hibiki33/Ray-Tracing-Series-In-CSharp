using RayTracingTheNextWeek.Common;

namespace RayTracingTheNextWeek.Hits
{
    internal class HittableList : Hittable
    {
        private readonly List<Hittable> objects;

        public IReadOnlyList<Hittable> Objects => objects;
        
        public override AABB BoundingBox { get; set; }

        public HittableList()
        {
            objects = new List<Hittable>();
            BoundingBox = new AABB();
        }

        //public HittableList(IEnumerable<Hittable> objects)
        //{
        //    this.objects = objects.ToList();
        //    BoundingBox = new AABB();
        //}

        public void Add(in Hittable obj)
        {
            objects.Add(obj);
            BoundingBox = new AABB(BoundingBox, obj.BoundingBox);
        }

        public void Clear()
        {
            objects.Clear();
        }

        public override bool Hit(in Ray ray, Interval<float> t, ref HitRecord rec)
        {
            HitRecord tempRec = new HitRecord();
            var hitAnything = false;
            var closestSoFar = t.Max;

            foreach (var obj in objects)
            {
                if (obj.Hit(ray, new (t.Min, closestSoFar), ref tempRec))
                {
                    hitAnything = true;
                    closestSoFar = tempRec.T; // Ensure tempRec is not null
                    rec = tempRec;
                }
            }

            return hitAnything;
        }
    }
}
