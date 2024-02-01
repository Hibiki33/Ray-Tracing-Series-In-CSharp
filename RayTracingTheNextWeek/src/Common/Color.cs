using System.Numerics;

namespace RayTracingTheNextWeek.Common
{
    internal class Color
    {
        static readonly Interval<float> intensity = new Interval<float>(0.0f, 0.999f);

        private Vector3 color;

        public float R
        {
            get => color.X;
            set => color.X = value;
        }

        public float G
        {
            get => color.Y;
            set => color.Y = value;
        }

        public float B
        {
            get => color.Z;
            set => color.Z = value;
        }

        public Color(float r, float g, float b) : this(new Vector3(r, g, b))
        {
        }

        public Color(Vector3 color)
        {
            this.color = color;
        }

        public static implicit operator Color(Vector3 v)
        {
            return new Color(v);
        }

        public static implicit operator Vector3(Color c)
        {
            return c.color;
        }

        public static Color operator +(in Color a, in Color b)
        {
            return new Color(a.color + b.color);
        }

        public static Color operator -(in Color a, in Color b)
        {
            return new Color(a.color - b.color);
        }

        public static Color operator *(in Color a, in Color b)
        {
            return new Color(a.color * b.color);
        }

        public static Color operator *(in Color a, float b)
        {
            return new Color(a.color * b);
        }

        public static Color operator *(float a, in Color b)
        {
            return new Color(a * b.color);
        }

        public static bool operator ==(in Color a, in Color b)
        {
            return a.color == b.color;
        }

        public static bool operator !=(in Color a, in Color b)
        {
            return a.color != b.color;
        }

        public override bool Equals(object? obj)
        {
            if (obj is Color other)
            {
                return color.Equals(other.color);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return color.GetHashCode();
        }

        public override string ToString()
        {
            return $"({R}, {G}, {B})";
        }

        public static void WriteColor(StreamWriter streamWriter, in Color pixelColor, int samplesPerPixel)
        {
            var r = pixelColor.R;
            var g = pixelColor.G;
            var b = pixelColor.B;

            // Divide the color by the number of samples
            var scale = 1.0f / samplesPerPixel;
            r *= scale;
            g *= scale;
            b *= scale;

            // Apply a linear to gamma transform for gamma 2
            var LinearToGamma = Utilities.LinearToGamma;
            r = LinearToGamma(r);
            g = LinearToGamma(g);
            b = LinearToGamma(b);

            // Write the translated [0,255] value of each color component
            streamWriter.Write($"{(int)(256f * intensity.Clamp(r))} " +
                               $"{(int)(256f * intensity.Clamp(g))} " +
                               $"{(int)(256f * intensity.Clamp(b))}\n");
        }

        public static Color Random(float min = 0f, float max = 1f)
        {
            var RandomFloat = Utilities.RandomFloat;
            return new Color(RandomFloat(min, max), RandomFloat(min, max), RandomFloat(min, max));
        }

        public static Color Black => new Color(0, 0, 0);
        public static Color White => new Color(1, 1, 1);
        public static Color Red => new Color(1, 0, 0);
        public static Color Green => new Color(0, 1, 0);
        public static Color Blue => new Color(0, 0, 1);
    }
}
