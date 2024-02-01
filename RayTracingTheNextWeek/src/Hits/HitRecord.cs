using System.Diagnostics;
using System.Numerics;
using RayTracingTheNextWeek.Common;
using RayTracingTheNextWeek.Materials;


namespace RayTracingTheNextWeek.Hits
{
    internal class HitRecord
    {
        public Vector3 P { get; set; }
        public Vector3 Normal { get; set; }
        public Material? Material { get; set; }
        public float T { get; set; }
        public float U { get; set; }
        public float V { get; set; }
        public bool FrontFace { get; set; }

        public void SetFaceNormal(in Ray r, in Vector3 outwardNormal)
        {
            FrontFace = Vector3.Dot(r.Direction, outwardNormal) < 0;
            Normal = FrontFace ? outwardNormal : -outwardNormal;
        }
    }
}
