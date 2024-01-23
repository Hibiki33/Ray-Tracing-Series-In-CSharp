using System.Numerics;

namespace RayTracingInOneWeekend
{
    internal static class Utilities
    {
        private static readonly Random random = new Random();

        public static float RandomFloat(float min = 0f, float max = 1f)
        {
            return (float)random.NextDouble() * (max - min) + min;
        }

        public static float RandomGaussian(float mean = 0.0f, float stddev = 1.0f)
        {
            float u1, u2, s;
            do
            {
                u1 = 2.0f * (float)random.NextDouble() - 1.0f;
                u2 = 2.0f * (float)random.NextDouble() - 1.0f;
                s = u1 * u1 + u2 * u2;
            }
            while (s >= 1.0f || s == 0f);

            // Box-Muller transform
            float z = (float)Math.Sqrt(-2.0f * Math.Log(s) / s);
            return mean + stddev * u1 * z;
        }

        public static Vector3 RandomInUnitDisk()
        {
            while (true)
            {
                var p = new Vector3(RandomFloat(-1, 1), RandomFloat(-1, 1), 0);
                if (p.LengthSquared() < 1)
                {
                    return p;
                }
            }
        }

        public static Vector3 RandomInUnitSphere()
        {
            while (true)
            {
                Vector3 p = new Vector3(RandomFloat(-1, 1), RandomFloat(-1, 1), RandomFloat(-1, 1));
                if (p.LengthSquared() < 1)
                {
                    return p;
                }
            }
        }

        public static Vector3 RandomUnitVector()
        {
            return Vector3.Normalize(RandomInUnitSphere());
        }

        public static bool NearZero(this Vector3 v)
        {
            const float s = 1e-8f;
            return Math.Abs(v.X) < s && Math.Abs(v.Y) < s && Math.Abs(v.Z) < s;
        }
        
        public static Vector3 Reflect(Vector3 v, Vector3 n)
        {
            // Actually, this is already implemented in System.Numerics.Vector3
            return Vector3.Reflect(v, n);
        }

        public static Vector3 Refract(Vector3 uv, Vector3 n, float etaiOverEtat)
        {
            float cosTheta = Math.Min(Vector3.Dot(-uv, n), 1.0f);
            Vector3 rOutPerp = etaiOverEtat * (uv + cosTheta * n);
            Vector3 rOutParallel = -(float)Math.Sqrt(Math.Abs(1.0f - rOutPerp.LengthSquared())) * n;
            return rOutPerp + rOutParallel;
        }

        public static float DegreesToRadians(float degrees)
        {
            return degrees * (float)Math.PI / 180.0f;
        }

        public static float RadiansToDegrees(float radians)
        {
            return radians * 180.0f / (float)Math.PI;
        }
    }
}
