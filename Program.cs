using System;
using System.Collections.Generic;

// Tips: on Windows, make sure to use cmd.exe to run the application. With
// PowerShell, the redirected output ends up with a Unicode byte order mark
// (0xFF, 0xFE) at the beginning of the file. Tools such as display on Linux
// cannot parse a ppm file with byte order mark.

namespace RayTracingInOneWeekend
{
    static class Program
    {
        static readonly Random Rng = new Random();

        static Vec3 Color(Ray ray, HitableItems world, int depth)
        {
            var record = new HitRecord();

            // Ignore close hits as they're likely the cause of rounding errors
            // with t values. Not ignoring causes an undesirable visual effect
            // called the shadow acne.
            if (world.Hit(ray, 0.001f, float.MaxValue, ref record))
            {
                return (depth < 50 && record.Material.Scatter(ray, record, out Vec3 attenuation, out Ray scatterRay))
                    ? attenuation * Color(scatterRay, world, depth + 1)
                    : new Vec3(0, 0, 0);
            }
            else
            {
                var unitDirection = Vec3.UnitVector(ray.Direction);
                var t = 0.5f * (unitDirection.Y + 1);
                return (1 - t) * new Vec3(1, 1, 1) + t * new Vec3(0.5f, 0.7f, 1);
            }
        }

        static HitableItems RandomScene()
        {
            var hitables = new List<Hitable>();
            hitables.Add(new Sphere(new Vec3(0, -1000, 0), 1000, new Lambertian(new Vec3(0.5f, 0.5f, 0.5f))));

            for (var a = -11; a < 11; a++)
            {
                for (var b = -11; b < 11; b++)
                {
                    var chooseMaterial = Rng.NextDouble();
                    var center = new Vec3(a + 0.9f * (float)Rng.NextDouble(), 0.2f, b + 0.9f * (float)Rng.NextDouble());

                    if ((center - new Vec3(4, 0.2f, 0)).Length() > 0.9)
                    {
                        // Diffuse
                        if (chooseMaterial < 0.8)
                        {
                            hitables.Add(
                                new Sphere(center, 0.2f,
                                    new Lambertian(
                                        new Vec3(
                                            (float)(Rng.NextDouble() * Rng.NextDouble()),
                                            (float)(Rng.NextDouble() * Rng.NextDouble()),
                                            (float)(Rng.NextDouble() * Rng.NextDouble())))));                                
                        }
                        // Metal
                        else if (chooseMaterial < 0.95)
                        {
                            hitables.Add(
                                new Sphere(center, 0.2f,
                                    new Metal(
                                        new Vec3(
                                            0.5f * (1 + (float)Rng.NextDouble()),
                                            0.5f * (1 + (float)Rng.NextDouble()),
                                            0.5f * (1 + (float)Rng.NextDouble())), 0.1f)));
                        }
                        // Glass
                        else
                        {
                            hitables.Add(new Sphere(center, 0.2f, new Dielectric(1.5f)));
                        }
                    }
                }
            }

            hitables.Add(new Sphere(new Vec3(0, 1, 0), 1, new Dielectric(1.5f)));
            hitables.Add(new Sphere(new Vec3(-4, 1, 0), 1, new Lambertian(new Vec3(0.4f, 0.2f, 0.1f))));
            hitables.Add(new Sphere(new Vec3(4, 1, 0), 1, new Metal(new Vec3(0.7f, 0.6f, 0.5f), 0.0f)));
            return new HitableItems(hitables.ToArray());
        }

        static void Main()
        {
            const int numX = 1200;
            const int numY = 800;
            const int numSamples = 10;

            Console.WriteLine($"P3\n{numX} {numY}\n255\n");

            var world = RandomScene();
            var lookFrom = new Vec3(13, 2, 3);
            var lookAt = new Vec3(0, 0, 0);
            var distanceToFocus = 10;
            var aperture = 0.1f;
            var camera = new Camera(lookFrom, lookAt, new Vec3(0, 1, 0), 20, numX / (float)numY, aperture, distanceToFocus);

            for (var j = numY - 1; j >= 0; j--)
            {
                for (var i = 0; i < numX; i++)
                {
                    var col = new Vec3(0, 0, 0);
                    for (var s = 0; s < numSamples; s++)
                    {
                        var u = (float)(i + Rng.NextDouble()) / numX;
                        var v = (float)(j + Rng.NextDouble()) / numY;
                        var r = camera.GetRay(u, v);
                        col += Color(r, world, 0);
                    }
                    
                    col /= numSamples;
                    var ir = (int)(255.99 * MathF.Sqrt(col.R));
                    var ig = (int)(255.99 * MathF.Sqrt(col.G));
                    var ib = (int)(255.99 * MathF.Sqrt(col.B));
                    Console.WriteLine($"{ir} {ig} {ib}");
                }
            }
        }
    }
}
