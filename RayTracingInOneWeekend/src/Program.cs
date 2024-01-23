using System.Numerics;
using RayTracingInOneWeekend;
using RayTracingInOneWeekend.Hit;
using RayTracingInOneWeekend.Material;

// Main
var world = new HittableList();

var groudMaterial = new Lambertian(new Color(0.5f, 0.5f, 0.5f));
world.Add(new Sphere(new Vector3(0, -1000, 0), 1000, groudMaterial));

for (var a = -11; a < 11; ++a)
{
    for (var b = -11; b < 11; ++b)
    {
        var chooseMat = Utilities.RandomFloat();
        var center = new Vector3(a + 0.9f * Utilities.RandomFloat(), 0.2f, b + 0.9f * Utilities.RandomFloat());

        if ((center - new Vector3(4, 0.2f, 0)).Length() > 0.9f)
        {
            IMaterial sphereMaterial;

            if (chooseMat < 0.8f)
            {
                // diffuse
                var albedo = Color.Random() * Color.Random();
                sphereMaterial = new Lambertian(albedo);
                world.Add(new Sphere(center, 0.2f, sphereMaterial));
            }
            else if (chooseMat < 0.95f)
            {
                // metal
                var albedo = Color.Random(0.5f, 1);
                var fuzz = Utilities.RandomFloat(0, 0.5f);
                sphereMaterial = new Metal(albedo, fuzz);
                world.Add(new Sphere(center, 0.2f, sphereMaterial));
            }
            else
            {
                // glass
                sphereMaterial = new Dielectric(1.5f);
                world.Add(new Sphere(center, 0.2f, sphereMaterial));
            }
        }
    }
}

var material1 = new Dielectric(1.5f);
world.Add(new Sphere(new Vector3(0, 1, 0), 1.0f, material1));

var material2 = new Lambertian(new Color(0.4f, 0.2f, 0.1f));
world.Add(new Sphere(new Vector3(-4, 1, 0), 1.0f, material2));

var material3 = new Metal(new Color(0.7f, 0.6f, 0.5f), 0.0f);
world.Add(new Sphere(new    (4, 1, 0), 1.0f, material3));

var camera = new Camera();
camera.ImageWidth = 1080;
camera.ImageHeight = 720;
camera.SamplesPerPixel = 100;
camera.MaxDepth = 20;
camera.VFov = 20.0f;
camera.LookFrom = new Vector3(13, 2, 3);
camera.LookAt = new Vector3(0, 0, 0);
camera.VUp = new Vector3(0, 1, 0);
camera.DefocusAngle = 0.6f;
camera.FocusDistance = 10.0f;

camera.Render(world, "./result1.ppm");
