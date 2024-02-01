using System.Numerics;
using System.Text;
using RayTracingTheNextWeek.Hits;
using RayTracingTheNextWeek.Common;

namespace RayTracingTheNextWeek
{
    internal class Camera
    {
        public int ImageWidth = 1080;
        public int ImageHeight = 720;

        public int SamplesPerPixel = 10;
        public int MaxDepth = 10;

        public Color Background = new Color(0.5f, 0.7f, 1.0f);

        public float VFov = 90.0f;
        public Vector3 LookFrom = new Vector3(0, 0, -1);
        public Vector3 LookAt = new Vector3(0, 0, 0);
        public Vector3 VUp = new Vector3(0, 1, 0);

        public float DefocusAngle = 0.0f;
        public float FocusDistance = 10.0f;
        
        private float AspectRatio;

        private Vector3 pixelOrigin;
        private Vector3 pixelDeltaU;
        private Vector3 pixelDeltaV;
        private Vector3 u, v, w;

        private Vector3 defocusDiskU;
        private Vector3 defocusDiskV;

        public void Render(in Hittable world, string? savePath = null)
        {
            initialize();
            
            var writer = savePath != null ? new StreamWriter(savePath, false, Encoding.ASCII) : 
                                            new StreamWriter(Console.OpenStandardOutput(), Encoding.ASCII);
            writer.AutoFlush = true;
            writer.Write($"P3\n{ImageWidth} {ImageHeight}\n255\n");

            var consoleUpdater = new Logger("\rScanlines remaining: ");

            for (var j = 0; j < ImageHeight; ++j)
            {
                consoleUpdater.UpdateMessage($"{ImageHeight - j}");

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

            consoleUpdater.ClearAndUpdate();

            if (savePath != null)
            {
                writer.Close();
            }
        }

        private void initialize()
        {
            AspectRatio = (float)ImageWidth / ImageHeight;

            var theta = Utilities.DegreesToRadians(VFov);
            var h = MathF.Tan(theta / 2);
            var viewportHeight = 2.0f * h * FocusDistance;
            var viewportWidth = AspectRatio * viewportHeight;

            w = Vector3.Normalize(LookFrom - LookAt);
            u = Vector3.Normalize(Vector3.Cross(VUp, w));
            v = Vector3.Cross(w, u);

            var viewportU = viewportWidth * u;
            var viewportV = viewportHeight * -v;

            pixelDeltaU = viewportU / ImageWidth;
            pixelDeltaV = viewportV / ImageHeight;

            pixelOrigin = LookFrom - (FocusDistance * w) - (viewportU + viewportV) * 0.5f;
            pixelOrigin += 0.5f * (pixelDeltaU + pixelDeltaV);

            var defocusRadius = FocusDistance * MathF.Tan(Utilities.DegreesToRadians(DefocusAngle / 2));
            defocusDiskU = u * defocusRadius;
            defocusDiskV = v * defocusRadius;
        }

        private Ray GetRay(int i, int j)
        {
            var pixelCenter = pixelOrigin + i * pixelDeltaU + j * pixelDeltaV;
            var pixelSample = pixelCenter + PixelSampleSquare();

            var rayOrigin = (DefocusAngle <= 0) ? LookFrom :  DefocusDiskSample();
            var rayDirection = Vector3.Normalize(pixelSample - rayOrigin);
            var rayTime = Utilities.RandomFloat();

            return new Ray(rayOrigin, rayDirection, rayTime);
        }

        private Vector3 PixelSampleSquare()
        {
            var px = Utilities.RandomFloat() - 0.5f;
            var py = Utilities.RandomFloat() - 0.5f;
            return px * pixelDeltaU + py * pixelDeltaV;
        }

        private Vector3 PixelSampleDisk(float radius)
        {
            var p = radius * Utilities.RandomInUnitDisk();
            return p.X * defocusDiskU + p.Y * defocusDiskV;
        }

        private Vector3 DefocusDiskSample()
        {
            var p = Utilities.RandomInUnitDisk();
            return LookFrom + p.X * defocusDiskU + p.Y * defocusDiskV;
        }

        private Color RayColor(Ray ray, int depth, Hittable world)
        {
            if (depth <= 0)
            {
                return Color.Black;
            }

            var rec = new HitRecord();
            if (!world.Hit(ray, new (0.001f, float.MaxValue), ref rec))
            {
                return Background;
            }

            var scattered = new Ray();
            var attenuation = Color.Black;
            var colorFromEmission = rec.Material!.Emitted(rec.U, rec.V, rec.P);
            
            if (!rec.Material!.Scatter(ray, rec, out attenuation, out scattered))
            {
                return colorFromEmission;
            }

            return colorFromEmission + attenuation! * RayColor(scattered!, depth - 1, world);
        }   
    }
}
