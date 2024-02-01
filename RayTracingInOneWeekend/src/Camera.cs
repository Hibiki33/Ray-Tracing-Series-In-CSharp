using System.Numerics;
using System.Text;
using RayTracingInOneWeekend.Hit;

namespace RayTracingInOneWeekend
{
    internal class Camera
    {
        public int ImageWidth = 1080;
        public int ImageHeight = 720;

        public int SamplesPerPixel = 10;
        public int MaxDepth = 10;

        public float VFov = 90.0f;
        public Vector3 LookFrom = new Vector3(0, 0, -1);
        public Vector3 LookAt = new Vector3(0, 0, 0);
        public Vector3 VUp = new Vector3(0, 1, 0);

        public float DefocusAngle = 0.0f;
        public float FocusDistance = 10.0f;
        
        private float AspectRatio;

        private Vector3 _pixelOrigin;
        private Vector3 _pixelDeltaU;
        private Vector3 _pixelDeltaV;
        private Vector3 u, v, w;

        private Vector3 _defocusDiskU;
        private Vector3 _defocusDiskV;

        public void Render(in IHittable world, string? savePath = null)
        {
            initialize();
            
            var writer = savePath != null ? new StreamWriter(savePath, false, Encoding.ASCII) : 
                                            new StreamWriter(Console.OpenStandardOutput(), Encoding.ASCII);
            writer.AutoFlush = true;

            writer.Write($"P3\n{ImageWidth} {ImageHeight}\n255\n");

            for (var j = 0; j < ImageHeight; ++j)
            {
                for (var i = 0; i < ImageWidth; ++i)
                {
                    var pixelColor = Color.Black;
                    for (var s = 0; s < SamplesPerPixel; ++s)
                    { 
                        var ray = GetRay(i, j);
                        pixelColor += RayColor(ray, MaxDepth, world);
                    }

                    Color.WriteColor(writer, pixelColor, SamplesPerPixel);
                }
            }

            if (savePath != null)
            {
                writer.Close();
            }
        }

        private void initialize()
        {
            AspectRatio = (float)ImageWidth / ImageHeight;

            var theta = Utilities.DegreesToRadians(VFov);
            var h = (float)Math.Tan(theta / 2);
            var viewportHeight = 2.0f * h * FocusDistance;
            var viewportWidth = AspectRatio * viewportHeight;

            w = Vector3.Normalize(LookFrom - LookAt);
            u = Vector3.Normalize(Vector3.Cross(VUp, w));
            v = Vector3.Cross(w, u);

            var viewportU = viewportWidth * u;
            var viewportV = viewportHeight * -v;

            _pixelDeltaU = viewportU / ImageWidth;
            _pixelDeltaV = viewportV / ImageHeight;

            _pixelOrigin = LookFrom - (FocusDistance * w) - (viewportU + viewportV) * 0.5f;
            _pixelOrigin += 0.5f * (_pixelDeltaU + _pixelDeltaV);

            var defocusRadius = FocusDistance * (float)Math.Tan(Utilities.DegreesToRadians(DefocusAngle / 2));
            _defocusDiskU = u * defocusRadius;
            _defocusDiskV = v * defocusRadius;
        }

        private Ray GetRay(int i, int j)
        {
            var pixelCenter = _pixelOrigin + i * _pixelDeltaU + j * _pixelDeltaV;
            var pixelSample = pixelCenter + PixelSampleSquare();

            var rayOrigin = (DefocusAngle <= 0) ? LookFrom :  DefocusDiskSample();
            var rayDirection = Vector3.Normalize(pixelSample - rayOrigin);
        
            return new Ray(rayOrigin, rayDirection);
        }

        private Vector3 PixelSampleSquare()
        {
            var px = Utilities.RandomFloat() - 0.5f;
            var py = Utilities.RandomFloat() - 0.5f;
            return px * _pixelDeltaU + py * _pixelDeltaV;
        }

        private Vector3 PixelSampleDisk(float radius)
        {
            var p = radius * Utilities.RandomInUnitDisk();
            return p.X * _defocusDiskU + p.Y * _defocusDiskV;
        }

        private Vector3 DefocusDiskSample()
        {
            var p = Utilities.RandomInUnitDisk();
            return LookFrom + p.X * _defocusDiskU + p.Y * _defocusDiskV;
        }

        private Color RayColor(Ray ray, int depth, IHittable world)
        {
            if (depth <= 0)
            {
                return Color.Black;
            }

            if (world.Hit(ray, new (0.001f, float.MaxValue), out var rec))
            {
                if (rec!.Material!.Scatter(ray, rec, out var attenuation, out var scattered))
                {
                    return attenuation * RayColor(scattered, depth - 1, world);
                }

                return Color.Black;
            }

            var unitDirection = Vector3.Normalize(ray.Direction);
            var a = 0.5f * (unitDirection.Y + 1.0f);
            return (1.0f - a) * Color.White + a * new Color(0.5f, 0.7f, 1.0f);
        }   
    }
}
