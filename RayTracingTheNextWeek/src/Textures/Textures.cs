using System.Numerics;
using RayTracingTheNextWeek.Common;

using Image = System.Drawing.Image;
using Bitmap = System.Drawing.Bitmap;

namespace RayTracingTheNextWeek.Textures
{
    internal class SolidColor : Texture
    {
        private readonly Color color;

        public SolidColor(Color color)
        {
            this.color = color;
        }

        public SolidColor(float r, float g, float b) : this(new Vector3(r, g, b))
        {
        }

        public override Color Value(float u, float v, in Vector3 p)
        {
            return color;
        }
    }

    internal class CheckerTexture : Texture
    {
        private readonly Texture even;
        private readonly Texture odd;
        private readonly float invScale;

        public CheckerTexture(Texture even, Texture odd, float scale = 1f)
        {
            this.even = even;
            this.odd = odd;
            invScale = 1.0f / scale;
        }

        public CheckerTexture(Color even, Color odd, float scale = 1f) : 
            this(new SolidColor(even), new SolidColor(odd), scale)
        {
        }

        public override Color Value(float u, float v, in Vector3 p)
        {
            var x = (int)MathF.Floor(p.X * invScale);
            var y = (int)MathF.Floor(p.Y * invScale);
            var z = (int)MathF.Floor(p.Z * invScale);
            var isEven = (x + y + z) % 2 == 0;

            return isEven ? even.Value(u, v, p) : odd.Value(u, v, p);
        }
    }

    internal class NoiseTexture : Texture
    {
        private readonly Perlin noise;
        private readonly float scale;

        public NoiseTexture(float scale)
        {
            noise = new Perlin();
            this.scale = scale;
        }

        public override Color Value(float u, float v, in Vector3 p)
        {
            return Color.White * 0.5f * (1.0f + MathF.Sin(scale * p.Z + 10.0f * noise.Turb(p)));
        }
    }

#pragma warning disable CA1416
    internal class ImageTexture : Texture
    {
        private static readonly Interval<float> coordSpace = new Interval<float>(0.0f, 1.0f);
        private static readonly float inv255 = 1.0f / 255.0f;

        private readonly Image image;

        public ImageTexture(string filename)
        {
            image = Image.FromFile(filename);
        }

        public override Color Value(float u, float v, in Vector3 p)
        {
            if (image == null || image!.Height <= 0)
            {
                return new Color(0f, 1f, 1f);
            }

            var i = coordSpace.Clamp(u);
            var j = 1.0f - coordSpace.Clamp(v);

            i = (int)(i * image!.Width);
            j = (int)(j * image!.Height);

            i = Math.Max(0, Math.Min(i, image.Width - 1));
            j = Math.Max(0, Math.Min(j, image.Height - 1));

            var pixel = ((Bitmap)image!).GetPixel((int)i, (int)j);

            return new Color(pixel.R * inv255, pixel.G * inv255, pixel.B * inv255);
        }
    }
#pragma warning restore CA1416
}
