using System.Numerics;
using RayTracingInOneWeekend.Hit;

namespace RayTracingInOneWeekend.Material
{
    internal interface IMaterial
    {
        bool Scatter(Ray ray, HitRecord rec, out Color attenuation, out Ray scattered);
    }
}
