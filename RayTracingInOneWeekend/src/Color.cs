using System.Numerics;

namespace RayTracingInOneWeekend
{
    internal class Color
    {
        static readonly Interval<float> intensity = new Interval<float>(0.0f, 0.999f);

        private Vector3 _color;

        public float R 
        {
            get => _color.X; 
            set => _color.X = value; 
        }

        public float G 
        { 
            get => _color.Y; 
            set => _color.Y = value; 
        }
        
        public float B { 
            get => _color.Z; 
            set => _color.Z = value; 
        }

        public static Color Black => new Color(0, 0, 0);

        public static Color White => new Color(1, 1, 1);

        public Color(float r, float g, float b)
        {
            _color = new Vector3(r, g, b);
        }

        public Color(Vector3 color)
        {
            _color = color;
        }

        public static Color operator +(in Color a, in Color b)
        {
            return new Color(a._color + b._color);
        }

        public static Color operator -(in Color a, in Color b)
        {
            return new Color(a._color - b._color);
        }

        public static Color operator *(in Color a, in Color b)
        {
            return new Color(a._color * b._color);
        }

        public static Color operator *(in Color a, float b)
        {
            return new Color(a._color * b);
        }

        public static Color operator *(float a, in Color b)
        {
            return new Color(a * b._color);
        }

        public static bool operator ==(in Color a, in Color b)
        {
            return a._color == b._color;
        }

        public static bool operator !=(in Color a, in Color b)
        {
            return a._color != b._color;
        }

        public override bool Equals(object? obj)
        {
            if (obj is Color other)
            {
                return _color.Equals(other._color);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return _color.GetHashCode();
        }

        public static float LinearToGamma(float linear)
        {
            return (float)Math.Sqrt(linear);
        }

        public static void WriteColor(StreamWriter streamWriter, Color pixelColor, int samplesPerPixel)
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
            Func<float, float> linearToGamma = linear => (float)Math.Sqrt(linear);
            r = linearToGamma(r);
            g = linearToGamma(g);
            b = linearToGamma(b);

            // Write the translated [0,255] value of each color component
            streamWriter.Write($"{(int)(256f * intensity.Clamp(r))} " +
                               $"{(int)(256f * intensity.Clamp(g))} " +
                               $"{(int)(256f * intensity.Clamp(b))}\n");
        }

        public static implicit operator Color(Vector3 v)
        {
            return new Color(v);
        }

        public static Color Random(float min = 0f, float max = 1f)
        {
            return new Color(Utilities.RandomFloat(min, max), Utilities.RandomFloat(min, max), Utilities.RandomFloat(min, max));
        }
    }
}
