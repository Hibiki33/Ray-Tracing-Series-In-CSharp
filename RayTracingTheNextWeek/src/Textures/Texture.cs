using System.Numerics;
using RayTracingTheNextWeek.Common;

namespace RayTracingTheNextWeek.Textures
{
    internal abstract class Texture
    {
        public abstract Color Value(float u, float v, in Vector3 p);
    }
}
