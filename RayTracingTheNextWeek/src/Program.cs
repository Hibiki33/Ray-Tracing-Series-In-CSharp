using System.Numerics;
using RayTracingTheNextWeek;
using RayTracingTheNextWeek.Hits;
using RayTracingTheNextWeek.Materials;
using RayTracingTheNextWeek.Common;
using RayTracingTheNextWeek.Textures;
using RayTracingTheNextWeek.src.Hits;

// Main
int mode = 9;

switch (mode)
{
    case 1: RandomSpheres(); break;
    case 2: TwoSpheres(); break;
    case 3: Earth(); break;
    case 4: TwoPerlinSpheres(); break;
    case 5: Quads(); break;
    case 6: SimpleLight(); break;
    case 7: CornellBox(); break;
    case 8: CornellSmoke(); break;
    case 9: FinalScene(); break;
    default: All(); break; 
        
}

void RandomSpheres()
{
    HittableList world = new HittableList();

    var checker = new CheckerTexture(new Color(0.2f, 0.3f, 0.1f), new Color(0.9f, 0.9f, 0.9f), 0.32f);
    world.Add(new Sphere(new Vector3(0, -1000, 0), 1000, new Lambertian(checker)));

    for (var a = -11; a < 11; ++a)
    {
        for (var b = -11; b < 11; ++b)
        {
            var chooseMat = Utilities.RandomFloat();
            var center = new Vector3(a + 0.9f * Utilities.RandomFloat(), 0.2f, b + 0.9f * Utilities.RandomFloat());

            if ((center - new Vector3(4, 0.2f, 0)).Length() > 0.9f)
            {
                Material sphereMaterial;

                if (chooseMat < 0.8f)
                {
                    // diffuse
                    var albedo = Color.Random() * Color.Random();
                    sphereMaterial = new Lambertian(albedo);
                    var center2 = center + new Vector3(0, Utilities.RandomFloat(0, 0.5f), 0);
                    world.Add(new Sphere(center, center2, 0.2f, sphereMaterial));
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
    world.Add(new Sphere(new Vector3(4, 1, 0), 1.0f, material3));

    var scene = new HittableList();
    scene.Add(world);

    var camera = new Camera();
    camera.ImageWidth = 400;
    camera.ImageHeight = 225;
    camera.SamplesPerPixel = 100;
    camera.MaxDepth = 50;
    camera.Background = new Color(0.7f, 0.8f, 1.0f);

    camera.VFov = 20;
    camera.LookFrom = new Vector3(13, 2, 3);
    camera.LookAt = new Vector3(0, 0, 0);
    camera.VUp = new Vector3(0, 1, 0);

    camera.DefocusAngle = 0.02f;
    camera.FocusDistance = 10f;

    camera.Render(scene, "random_spheres.ppm");
}

void TwoSpheres()
{
    HittableList world = new HittableList();

    var checker = new CheckerTexture(new Color(0.2f, 0.3f, 0.1f), new Color(0.9f, 0.9f, 0.9f), 0.32f);
    world.Add(new Sphere(new Vector3(0, -10, 0), 10, new Lambertian(checker)));
    world.Add(new Sphere(new Vector3(0, 10, 0), 10, new Lambertian(checker)));

    Camera camera = new Camera();

    camera.ImageWidth = 400;
    camera.ImageHeight = 225;
    camera.SamplesPerPixel = 100;
    camera.MaxDepth = 50;
    camera.Background = new Color(0.7f, 0.8f, 1.0f);

    camera.VFov = 20;
    camera.LookFrom = new Vector3(13, 2, 3);
    camera.LookAt = new Vector3(0, 0, 0);
    camera.VUp = new Vector3(0, 1, 0);

    camera.DefocusAngle = 0f;
    camera.FocusDistance = 10f;

    camera.Render(world, "two_spheres.ppm");
}  

void Earth()
{
    var earthTexture = new ImageTexture("../../../../Assets/earthmap.jpg");
    var earthSurface = new Lambertian(earthTexture);
    
    var globe = new Sphere(new Vector3(0, 0, 0), 2, earthSurface);

    var camera = new Camera();

    camera.ImageWidth = 400;
    camera.ImageHeight = 225;
    camera.SamplesPerPixel = 100;
    camera.MaxDepth = 50;
    camera.Background = new Color(0.7f, 0.8f, 1.0f);

    camera.VFov = 20;
    camera.LookFrom = new Vector3(0, 0, 12);
    camera.LookAt = new Vector3(0, 0, 0);
    camera.VUp = new Vector3(0, 1, 0);

    camera.DefocusAngle = 0f;
    camera.FocusDistance = 10f;

    camera.Render(globe, "earth.ppm");
}

void TwoPerlinSpheres()
{
    HittableList world = new HittableList();

    var perlinTexture = new NoiseTexture(4);
    world.Add(new Sphere(new Vector3(0, -1000, 0), 1000, new Lambertian(perlinTexture)));
    world.Add(new Sphere(new Vector3(0, 2, 0), 2, new Lambertian(perlinTexture)));

    var camera = new Camera();

    camera.ImageWidth = 400;
    camera.ImageHeight = 225;
    camera.SamplesPerPixel = 100;
    camera.MaxDepth = 50;
    camera.Background = new Color(0.7f, 0.8f, 1.0f);

    camera.VFov = 20;
    camera.LookFrom = new Vector3(13, 2, 3);
    camera.LookAt = new Vector3(0, 0, 0);
    camera.VUp = new Vector3(0, 1, 0);

    camera.DefocusAngle = 0f;
    camera.FocusDistance = 10f;

    camera.Render(world, "two_perlin_spheres.ppm");
}

void Quads()
{
    HittableList world = new HittableList();

    var leftRed = new Lambertian(new Color(1f, 0.2f, 0.2f));
    var backGreen = new Lambertian(new Color(0.2f, 1f, 0.2f));
    var rightBlue = new Lambertian(new Color(0.2f, 0.2f, 1f));
    var upperOrange = new Lambertian(new Color(1f, 0.5f, 0f));
    var lowerTeal = new Lambertian(new Color(0.2f, 0.8f, 0.8f));

    world.Add(new Quad(new Vector3(-3, -2, 5), new Vector3(0, 0, -4), new Vector3(0, 4,  0), leftRed));
    world.Add(new Quad(new Vector3(-2, -2, 0), new Vector3(4, 0,  0), new Vector3(0, 4,  0), backGreen));
    world.Add(new Quad(new Vector3( 3, -2, 1), new Vector3(0, 0,  4), new Vector3(0, 4,  0), rightBlue));
    world.Add(new Quad(new Vector3(-2,  3, 1), new Vector3(4, 0,  0), new Vector3(0, 0,  4), upperOrange));
    world.Add(new Quad(new Vector3(-2, -3, 5), new Vector3(4, 0,  0), new Vector3(0, 0, -4), lowerTeal));

    var camera = new Camera();

    camera.ImageWidth = 400;
    camera.ImageHeight = 400;
    camera.SamplesPerPixel = 100;
    camera.MaxDepth = 50;
    camera.Background = new Color(0.7f, 0.8f, 1.0f);

    camera.VFov = 80;
    camera.LookFrom = new Vector3(0, 0, 9);
    camera.LookAt = new Vector3(0, 0, 0);
    camera.VUp = new Vector3(0, 1, 0);

    camera.DefocusAngle = 0f;
    camera.FocusDistance = 10f;

    camera.Render(world, "quads.ppm");
}

void SimpleLight()
{
    HittableList world = new HittableList();

    var perlinTexture = new NoiseTexture(4);
    world.Add(new Sphere(new Vector3(0, -1000, 0), 1000, new Lambertian(perlinTexture)));
    world.Add(new Sphere(new Vector3(0, 2, 0), 2, new Lambertian(perlinTexture)));

    var difflight = new DiffuseLight(new Color(4, 4, 4));
    world.Add(new Sphere(new Vector3(0, 7, 0), 1, difflight));
    world.Add(new Quad(new Vector3(3, 1, -2), new Vector3(2, 0, 0), new Vector3(0, 2, 0), difflight));

    var camera = new Camera();

    camera.ImageWidth = 400;
    camera.ImageHeight = 225;
    camera.SamplesPerPixel = 100;
    camera.MaxDepth = 50;
    camera.Background = Color.Black;

    camera.VFov = 20;
    camera.LookFrom = new Vector3(26, 3, 6);
    camera.LookAt = new Vector3(0, 2, 0);
    camera.VUp = new Vector3(0, 1, 0);

    camera.DefocusAngle = 0f;
    camera.FocusDistance = 10f;

    camera.Render(world, "simple_light.ppm");
}

void CornellBox()
{
    var world = new HittableList();

    var red = new Lambertian(new Color(0.65f, 0.05f, 0.05f));
    var white = new Lambertian(new Color(0.73f, 0.73f, 0.73f));
    var green = new Lambertian(new Color(0.12f, 0.45f, 0.15f));
    var light = new DiffuseLight(new Color(15, 15, 15));

    world.Add(new Quad(new Vector3(555, 0, 0), new Vector3(0, 555, 0), new Vector3(0, 0, 555), green));
    world.Add(new Quad(new Vector3(0, 0, 0), new Vector3(0, 555, 0), new Vector3(0, 0, 555), red));
    world.Add(new Quad(new Vector3(343, 554, 332), new Vector3(-130, 0, 0), new Vector3(0, 0, -105), light));
    world.Add(new Quad(new Vector3(0, 0, 0), new Vector3(555, 0, 0), new Vector3(0, 0, 555), white));
    world.Add(new Quad(new Vector3(555, 555, 555), new Vector3(-555, 0, 0), new Vector3(0, 0, -555), white));
    world.Add(new Quad(new Vector3(0, 0, 555), new Vector3(555, 0, 0), new Vector3(0, 555, 0), white));

    Hittable box1 =  Quad.Box(new Vector3(0, 0, 0), new Vector3(165, 330, 165), white);
    box1 = new RotateY(box1, 15);
    box1 = new Translate(box1, new Vector3(265, 0, 295));
    world.Add(box1);

    Hittable box2 = Quad.Box(new Vector3(0, 0, 0), new Vector3(165, 165, 165), white);
    box2 = new RotateY(box2, -18);
    box2 = new Translate(box2, new Vector3(130, 0, 65));
    world.Add(box2);

    var camera = new Camera();

    camera.ImageWidth = 600;
    camera.ImageHeight = 600;
    camera.SamplesPerPixel = 200;
    camera.MaxDepth = 50;
    camera.Background = Color.Black;

    camera.VFov = 40;
    camera.LookFrom = new Vector3(278, 278, -800);
    camera.LookAt = new Vector3(278, 278, 0);
    camera.VUp = new Vector3(0, 1, 0);

    camera.DefocusAngle = 0f;
    camera.FocusDistance = 10f;

    camera.Render(world, "cornell_box.ppm");
}

void CornellSmoke()
{
    var world = new HittableList();

    var red = new Lambertian(new Color(0.65f, 0.05f, 0.05f));
    var white = new Lambertian(new Color(0.73f, 0.73f, 0.73f));
    var green = new Lambertian(new Color(0.12f, 0.45f, 0.15f));
    var light = new DiffuseLight(new Color(7, 7, 7));

    world.Add(new Quad(new Vector3(555, 0, 0), new Vector3(0, 555, 0), new Vector3(0, 0, 555), green));
    world.Add(new Quad(new Vector3(0, 0, 0), new Vector3(0, 555, 0), new Vector3(0, 0, 555), red));
    world.Add(new Quad(new Vector3(113, 554, 127), new Vector3(330, 0, 0), new Vector3(0, 0, 305), light));
    world.Add(new Quad(new Vector3(0, 555, 0), new Vector3(555, 0, 0), new Vector3(0, 0, 555), white));
    world.Add(new Quad(new Vector3(0, 0, 0), new Vector3(555, 0, 0), new Vector3(0, 0, 555), white));
    world.Add(new Quad(new Vector3(0, 0, 555), new Vector3(555, 0, 0), new Vector3(0, 555, 0), white));

    Hittable box1 = Quad.Box(new Vector3(0, 0, 0), new Vector3(165, 330, 165), white);
    box1 = new RotateY(box1, 15);
    box1 = new Translate(box1, new Vector3(265, 0, 295));
    
    Hittable box2 = Quad.Box(new Vector3(0, 0, 0), new Vector3(165, 165, 165), white);
    box2 = new RotateY(box2, -18);
    box2 = new Translate(box2, new Vector3(130, 0, 65));

    world.Add(new ConstantMedium(box1, 0.01f, new Color(0, 0, 0)));
    world.Add(new ConstantMedium(box2, 0.01f, new Color(1, 1, 1)));

    var camera = new Camera();

    camera.ImageWidth = 600;
    camera.ImageHeight = 600;
    camera.SamplesPerPixel = 200;
    camera.MaxDepth = 50;
    camera.Background = Color.Black;

    camera.VFov = 40;
    camera.LookFrom = new Vector3(278, 278, -800);
    camera.LookAt = new Vector3(278, 278, 0);
    camera.VUp = new Vector3(0, 1, 0);

    camera.DefocusAngle = 0f;
    camera.FocusDistance = 10f;

    camera.Render(world, "cornell_smoke.ppm");
}

void FinalScene()
{
    var world = new HittableList();

    var boxes1 = new HittableList();
    var ground = new Lambertian(new Color(0.48f, 0.83f, 0.53f));

    const int boxesPerSide = 20;
    for (var i = 0; i < boxesPerSide; ++i)
    {
        for (var j = 0; j < boxesPerSide; ++j)
        {
            const float w = 100.0f;
            var x0 = -1000.0f + i * w;
            var z0 = -1000.0f + j * w;
            var y0 = 0.0f;
            var x1 = x0 + w;
            var y1 = Utilities.RandomFloat(1, 101);
            var z1 = z0 + w;

            boxes1.Add(Quad.Box(new Vector3(x0, y0, z0), new Vector3(x1, y1, z1), ground));
        }
    }

    world.Add(new BVHNode(boxes1));
    // world.Add(boxes1);

    var light = new DiffuseLight(new Color(7, 7, 7));
    world.Add(new Quad(new Vector3(123, 554, 147), new Vector3(300, 0, 0), new Vector3(0, 0, 265), light));

    var center1 = new Vector3(400, 400, 200);
    var center2 = center1 + new Vector3(30, 0, 0);
    var sphereMaterial = new Lambertian(new Color(0.7f, 0.3f, 0.1f));
    world.Add(new Sphere(center1, center2, 50, sphereMaterial));

    world.Add(new Sphere(new Vector3(260, 150, 45), 50, new Dielectric(1.5f)));
    world.Add(new Sphere(new Vector3(0, 150, 145), 50, new Metal(new Color(0.8f, 0.8f, 0.9f), 1.0f)));

    var boundary = new Sphere(new Vector3(360, 150, 145), 70, new Dielectric(1.5f));
    world.Add(boundary);
    world.Add(new ConstantMedium(boundary, 0.2f, new Color(0.2f, 0.4f, 0.9f)));
    boundary = new Sphere(new Vector3(0, 0, 0), 5000, new Dielectric(1.5f));
    world.Add(new ConstantMedium(boundary, 0.0001f, new Color(1, 1, 1)));

    var emat = new Lambertian(new ImageTexture("../../../../Assets/earthmap.jpg"));
    world.Add(new Sphere(new Vector3(400, 200, 400), 100, emat));
    var pertext = new NoiseTexture(0.1f);
    world.Add(new Sphere(new Vector3(220, 280, 300), 80, new Lambertian(pertext)));

    var boxes2 = new HittableList();
    var white = new Lambertian(new Color(0.73f, 0.73f, 0.73f));
    const int ns = 1000;
    for (var j = 0; j < ns; ++j)
    {
        boxes2.Add(new Sphere(
            new Vector3(
                Utilities.RandomFloat(0, 165), 
                Utilities.RandomFloat(0, 165), 
                Utilities.RandomFloat(0, 165)), 
            10, white)
        );
    }

    world.Add(new Translate(new RotateY(new BVHNode(boxes2), 15), new Vector3(-100, 270, 395)));
    // world.Add(new Translate(new RotateY(boxes2, 15), new Vector3(-100, 270, 395)));

    var camera = new Camera();

    camera.ImageWidth = 800;
    camera.ImageHeight = 800;
    camera.SamplesPerPixel = 10000;
    camera.MaxDepth = 40;
    camera.Background = Color.Black;

    camera.VFov = 40;
    camera.LookFrom = new Vector3(478, 278, -600);
    camera.LookAt = new Vector3(278, 278, 0);
    camera.VUp = new Vector3(0, 1, 0);

    camera.DefocusAngle = 0f;
    camera.FocusDistance = 10f;

    camera.Render(world, "final_scene.ppm");
}

void All()
{
    RandomSpheres();
    TwoSpheres();
    Earth();
    TwoPerlinSpheres();
    Quads();
    SimpleLight();
    CornellBox();
    CornellSmoke();
    FinalScene();
}
