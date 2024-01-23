namespace RayTracingInOneWeekend.Hit
{
    internal class HittableList : IHittable
    {
        private readonly List<IHittable> _objects;

        public HittableList()
        {
            _objects = new List<IHittable>();
        }

        public HittableList(IEnumerable<IHittable> objects)
        {
            _objects = objects.ToList();
        }

        public void Add(IHittable obj)
        {
            _objects.Add(obj);
        }

        public void Clear()
        {
            _objects.Clear();
        }

        public bool Hit(Ray ray, Interval<float> t, out HitRecord? rec)
        {
            var hitAnything = false;
            var closestSoFar = t.Max;

            rec = default;

            foreach (var obj in _objects)
            {
                if (obj.Hit(ray, new (t.Min, closestSoFar), out var tempRec))
                {
                    hitAnything = true;
                    closestSoFar = tempRec!.T; // Ensure tempRec is not null
                    rec = tempRec;
                }
            }

            return hitAnything;
        }
    }
}
