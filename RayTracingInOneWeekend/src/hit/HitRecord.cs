using System.Numerics;
using RayTracingInOneWeekend.Material;


namespace RayTracingInOneWeekend.Hit
{
    internal class HitRecord
    {
        public Vector3 P { get; set; }
        public Vector3 Normal { get; set; }
        public IMaterial? Material { get; set; }
        public float T { get; set; }
        public bool FrontFace { get; set; }

        public void SetFaceNormal(Ray r, Vector3 outwardNormal)
        {
            FrontFace = Vector3.Dot(r.Direction, outwardNormal) < 0;
            Normal = FrontFace ? outwardNormal : -outwardNormal;
        }
    }
}
