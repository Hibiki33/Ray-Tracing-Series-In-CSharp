using System.Numerics;
using RayTracingTheNextWeek.Hits;
using RayTracingTheNextWeek.Common;
using RayTracingTheNextWeek.Textures;

namespace RayTracingTheNextWeek.Materials
{
    internal class Lambertian : Material
    {
        private readonly Texture albedo;

        public Lambertian(Texture albedo)
        {
            this.albedo = albedo;
        }

        public Lambertian(Color albedo) : this(new SolidColor(albedo))
        {
        }

        public override bool Scatter(in Ray ray, in HitRecord rec, out Color attenuation, out Ray scattered)
        {
            var scatterDirection = rec.Normal + Utilities.RandomUnitVector();
            
            if (scatterDirection.NearZero())
            {
                scatterDirection = rec.Normal;
            }
            
            scattered = new Ray(rec.P, scatterDirection, ray.Time);
            attenuation = albedo.Value(rec.U, rec.V, rec.P);
            return true;
        }
    }

    internal class Metal : Material
    {
        private readonly Color albedo;
        private readonly float fuzz;

        public Metal(Color albedo, float fuzz)
        {
            this.albedo = albedo;
            this.fuzz = fuzz < 1 ? fuzz : 1;
        }

        public override bool Scatter(in Ray ray, in HitRecord rec, out Color attenuation, out Ray scattered)
        {
            Vector3 reflected = Vector3.Reflect(Vector3.Normalize(ray.Direction), rec.Normal);
            scattered = new Ray(rec.P, reflected + fuzz * Utilities.RandomInUnitSphere(), ray.Time);
            attenuation = albedo;
            return Vector3.Dot(scattered.Direction, rec.Normal) > 0;
        }
    }

    internal class Dielectric : Material
    {
        private readonly float refIdx;

        public Dielectric(float refIdx)
        {
            this.refIdx = refIdx;
        }

        public override bool Scatter(in Ray ray, in HitRecord rec, out Color attenuation, out Ray scattered)
        {
            attenuation = Color.White;
            var refractionRatio = rec.FrontFace ? (1.0f / refIdx) : refIdx;

            Vector3 unitDirection = Vector3.Normalize(ray.Direction);
            float cosTheta = MathF.Min(Vector3.Dot(-unitDirection, rec.Normal), 1.0f);
            float sinTheta = MathF.Sqrt(1.0f - cosTheta * cosTheta);

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

            scattered = new Ray(rec.P, direction, ray.Time);
            return true;
        }

        private static float Reflectance(float cosine, float refIdx)
        {
            float r0 = (1 - refIdx) / (1 + refIdx);
            r0 *= r0;
            return r0 + (1 - r0) * (float)Math.Pow((1 - cosine), 5);
        }
    }

    internal class DiffuseLight : Material
    {
        private readonly Texture emit;

        public DiffuseLight(Texture emit)
        {
            this.emit = emit;
        }

        public DiffuseLight(Color color) : this(new SolidColor(color))
        {
        }

        public override bool Scatter(in Ray ray, in HitRecord rec, out Color? attenuation, out Ray? scattered)
        {
            attenuation = null;
            scattered = null;
            return false;
        }

        public override Color Emitted(float u, float v, in Vector3 p)
        {
            return emit.Value(u, v, p);
        }
    }

    internal class Isotropic : Material
    {
        private readonly Texture albedo;

        public Isotropic(Texture albedo)
        {
            this.albedo = albedo;
        }

        public Isotropic(Color color) : this(new SolidColor(color))
        {
        }

        public override bool Scatter(in Ray ray, in HitRecord rec, out Color? attenuation, out Ray? scattered)
        {
            scattered = new Ray(rec.P, Utilities.RandomInUnitSphere(), ray.Time);
            attenuation = albedo.Value(rec.U, rec.V, rec.P);
            return true;
        }
    }
}
