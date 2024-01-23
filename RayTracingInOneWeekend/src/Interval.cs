namespace RayTracingInOneWeekend
{
    internal struct Interval<T> where T : IComparable<T>
    {
        public T Min { get; }
        public T Max { get; }

        public Interval(T min, T max)
        {
            Min = min;
            Max = max;
        }

        public T Size()
        {
            return Max.CompareTo(Min) > 0 ? (dynamic)Max - (dynamic)Min : (dynamic)Min - (dynamic)Max;
        }

        public bool Contains(in Interval<T> other)
        {
            return other.Min.CompareTo(Min) >= 0 && other.Max.CompareTo(Max) <= 0;
        }

        public bool Contains(T value)
        {
            return value.CompareTo(Min) >= 0 && value.CompareTo(Max) <= 0;
        }

        public bool Surrounds(in Interval<T> other)
        {
            return other.Min.CompareTo(Min) > 0 && other.Max.CompareTo(Max) < 0;
        }

        public bool Surrounds(T value)
        {
            return value.CompareTo(Min) > 0 && value.CompareTo(Max) < 0;
        }

        public bool Overlaps(in Interval<T> other)
        {
            return other.Min.CompareTo(Max) < 0 && other.Max.CompareTo(Min) > 0;
        }

        public T Clamp(T value)
        {
            return value.CompareTo(Min) < 0 ? Min : value.CompareTo(Max) > 0 ? Max : value;
        }

        public Interval<T> Expand(T delta)
        {
            var padding = (dynamic)delta / 2;
            return new Interval<T>(Min - padding, Max + padding);
        }

        public static Interval<T> operator +(in Interval<T> a, in Interval<T> b)
        {
            return new Interval<T>(a.Min.CompareTo(b.Min) < 0 ? a.Min : b.Min, a.Max.CompareTo(b.Max) > 0 ? a.Max : b.Max);
        }

        public static Interval<T> operator -(in Interval<T> a, in Interval<T> b)
        {
            return new Interval<T>(a.Min.CompareTo(b.Min) > 0 ? a.Min : b.Min, a.Max.CompareTo(b.Max) < 0 ? a.Max : b.Max);
        }

        public static bool operator ==(in Interval<T> a, in Interval<T> b)
        {
            return a.Min.CompareTo(b.Min) == 0 && a.Max.CompareTo(b.Max) == 0;
        }

        public static bool operator !=(in Interval<T> a, in Interval<T> b)
        {
            return a.Min.CompareTo(b.Min) != 0 || a.Max.CompareTo(b.Max) != 0;
        }

        public override bool Equals(object? obj)
        {
            if (obj is Interval<T> other)
            {
                return Min.CompareTo(other.Min) == 0 && Max.CompareTo(other.Max) == 0;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Min, Max);
        }

        public override string ToString()
        {
            return $"[{Min}, {Max}]";
        }   
    }
}
