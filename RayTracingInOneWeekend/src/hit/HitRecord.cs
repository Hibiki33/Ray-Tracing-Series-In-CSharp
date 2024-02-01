using System.Diagnostics;
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
            Debug.Assert(Math.Abs(outwardNormal.Length()) - 1.0f < 0.0001f);

            FrontFace = Vector3.Dot(r.Direction, outwardNormal) < 0;
            Normal = FrontFace ? outwardNormal : -outwardNormal;
        }
    }
}
