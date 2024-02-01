using System.Numerics;
using RayTracingTheNextWeek.Common;

namespace RayTracingTheNextWeek.Textures
{
    internal class Perlin
    {
        private static readonly int pointCount = 256;

        private readonly int[] permX;
        private readonly int[] permY;
        private readonly int[] permZ;
        private readonly Vector3[] ranVec;

        public Perlin()
        {
            ranVec = new Vector3[pointCount];
            for (var i = 0; i < pointCount; ++i)
            {
                ranVec[i] = Vector3.Normalize(new Vector3(
                    Utilities.RandomFloat(-1, 1),
                    Utilities.RandomFloat(-1, 1),
                    Utilities.RandomFloat(-1, 1))
                );
            }

            permX = GeneratePerm();
            permY = GeneratePerm();
            permZ = GeneratePerm();
        }

        public float Noise(in Vector3 p)
        {
            var Floor = MathF.Floor;

            var u = p.X - Floor(p.X);
            var v = p.Y - Floor(p.Y);
            var w = p.Z - Floor(p.Z);

            var i = (int)Floor(p.X);
            var j = (int)Floor(p.Y);
            var k = (int)Floor(p.Z);

            var c = new Vector3[2, 2, 2];
            for (var di = 0; di < 2; ++di)
            {
                for (var dj = 0; dj < 2; ++dj)
                {
                    for (var dk = 0; dk < 2; ++dk)
                    {
                        c[di, dj, dk] = ranVec[
                            permX[(i + di) & 255] ^
                            permY[(j + dj) & 255] ^
                            permZ[(k + dk) & 255]
                        ];
                    }
                }
            }

            return Interp(c, u, v, w);
        }

        public float Turb(in Vector3 p, int depth = 7)
        {
            var accum = 0.0f;
            var tempP = p;
            var weight = 1.0f;
            for (var i = 0; i < depth; ++i)
            {
                accum += weight * Noise(tempP);
                weight *= 0.5f;
                tempP *= 2;
            }

            return MathF.Abs(accum);
        }
        
        private static int[] GeneratePerm()
        {
            var p = new int[pointCount];
            for (var i = 0; i < pointCount; ++i)
            {
                p[i] = i;
            }

            Permute(p, pointCount);
            return p;
        }

        private static void Permute(int[] p, int n)
        {
            for (var i = n - 1; i > 0; --i)
            {
                var target = Utilities.RandomInt(0, i + 1);
                var tmp = p[i];
                p[i] = p[target];
                p[target] = tmp;
            }
        }

        private static float Interp(Vector3[,,] c, float u, float v, float w)
        {
            var uu = u * u * (3 - 2 * u);
            var vv = v * v * (3 - 2 * v);
            var ww = w * w * (3 - 2 * w);

            var accum = 0.0f;
            for (var i = 0; i < 2; ++i)
            {
                for (var j = 0; j < 2; ++j)
                {
                    for (var k = 0; k < 2; ++k)
                    {
                        accum += (i * uu + (1 - i) * (1 - uu)) *
                                 (j * vv + (1 - j) * (1 - vv)) *
                                 (k * ww + (1 - k) * (1 - ww)) *
                                 Vector3.Dot(c[i, j, k], new Vector3(u - i, v - j, w - k));
                    }
                }
            }

            return accum;
        }
    }
}
