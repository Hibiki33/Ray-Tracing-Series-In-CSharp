using System.Numerics;
using RayTracingInOneWeekend.Hit;

namespace RayTracingInOneWeekend.Material
{
    internal struct Lambertian : IMaterial
    {
        private readonly Color _albedo;

        public Lambertian(Color albedo)
        {
            _albedo = albedo;
        }

        public bool Scatter(Ray ray, HitRecord rec, out Color attenuation, out Ray scattered)
        {
            Vector3 scatterDirection = rec.Normal + Utilities.RandomUnitVector();
            if (scatterDirection.NearZero())
            {
                scatterDirection = rec.Normal;
            }
            scattered = new Ray(rec.P, scatterDirection);
            attenuation = _albedo;
            return true;
        }
    }

    internal struct Metal : IMaterial
    {
        private readonly Color _albedo;
        private readonly float _fuzz;

        public Metal(Color albedo, float fuzz)
        {
            _albedo = albedo;
            _fuzz = fuzz < 1 ? fuzz : 1;
        }

        public bool Scatter(Ray ray, HitRecord rec, out Color attenuation, out Ray scattered)
        {
            Vector3 reflected = Vector3.Reflect(Vector3.Normalize(ray.Direction), rec.Normal);
            scattered = new Ray(rec.P, reflected + _fuzz * Utilities.RandomInUnitSphere());
            attenuation = _albedo;
            return Vector3.Dot(scattered.Direction, rec.Normal) > 0;
        }
    }

    internal struct Dielectric : IMaterial
    {
        private readonly float _refIdx;

        public Dielectric(float refIdx)
        {
            _refIdx = refIdx;
        }

        public bool Scatter(Ray ray, HitRecord rec, out Color attenuation, out Ray scattered)
        {
            attenuation = new Color(Vector3.One);
            var refractionRatio = rec.FrontFace ? (1.0f / _refIdx) : _refIdx;

            Vector3 unitDirection = Vector3.Normalize(ray.Direction);
            float cosTheta = Math.Min(Vector3.Dot(-unitDirection, rec.Normal), 1.0f);
            float sinTheta = (float)Math.Sqrt(1.0f - cosTheta * cosTheta);

            bool cannotRefract = refractionRatio * sinTheta > 1.0f;
            Vector3 direction;

            if (cannotRefract || Reflectance(cosTheta, refractionRatio) > Utilities.RandomFloat())
            {
                direction = Utilities.Reflect(unitDirection, rec.Normal);
            }
            else
            {
                direction = Utilities.Refract(unitDirection, rec.Normal, refractionRatio);
            }

            scattered = new Ray(rec.P, direction);
            return true;
        }

        private static float Reflectance(float cosine, float refIdx)
        {
            float r0 = (1 - refIdx) / (1 + refIdx);
            r0 *= r0;
            return r0 + (1 - r0) * (float)Math.Pow((1 - cosine), 5);
        }
    }
}
