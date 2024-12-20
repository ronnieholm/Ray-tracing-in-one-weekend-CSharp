using System;

namespace RayTracingInOneWeekend;

internal abstract class Material
{
    protected static readonly Random Rng = new();
    protected static Vec3 UnitVector = new (1, 1, 1);
    private static readonly Vec3 ZeroVector = new(0, 0, 0);

    public abstract bool Scatter(Ray incidentRay, HitRecord rec, out Vec3 attenuation, out Ray scatteredRay);

    protected static Vec3 RandomInUnitSphere()
    {
        Vec3 p;

        // We could test for Length() > 1 instead as a unit vector would
        // satisfy >= 1. But computing the length requires squaring its
        // components whereas the squared length doesn't.
        do
        {
            // Pick a random point in the unit cube where x, y, z are in the
            // range -1 to +1. To do so, first pick a random x, y, and z in
            // the range 0 to +1. Then multiply those values by 2 to move
            // into range 0 to +2. Then subtract 1 to bring those values
            // into the final range -1 to +1.
            p = 2 * new Vec3(Rng.NextDouble(), Rng.NextDouble(), Rng.NextDouble()) - UnitVector;
        }
        while (p.SquaredLength() >= 1);
        return p;
    }

    protected static bool Refract(Vec3 v, Vec3 n, double niOverNt, out Vec3 refractedRay)
    {
        var uv = Vec3.UnitVector(v);
        var dt = Vec3.Dot(uv, n);
        var discriminant = 1 - niOverNt * niOverNt * (1 - dt * dt);

        if (discriminant > 0)
        {
            refractedRay = niOverNt * (uv - n * dt) - n * Math.Sqrt(discriminant);
            return true;
        }

        refractedRay = ZeroVector;
        return false;
    }

    protected static Vec3 Reflect(Vec3 ray, Vec3 normal) => ray - 2 * Vec3.Dot(ray, normal) * normal;

    protected static double Schlick(double cosine, double refractionIndex)
    {
        var r0 = (1 - refractionIndex) / (1 + refractionIndex);
        r0 *= r0;
        return r0 + (1 - r0) * Math.Pow(1 - cosine, 5);
    }
}

internal class Lambertian(Vec3 albedo) : Material
{
    // The ratio of the light reflected by an object to that received by it.

    public override bool Scatter(Ray incidentRay, HitRecord rec, out Vec3 attenuation, out Ray scatteredRay)
    {
        // Book calls targetOnUnitSphere for s: a point within the unit
        // radius sphere that is tangent to what the book called the
        // hit point, here PointOfIntersection. The unit sphere's center is
        // at p + N where N is the unit normal to p.
        var targetOnUnitSphere = rec.PointOfIntersection + rec.Normal + RandomInUnitSphere();
        scatteredRay = new Ray(rec.PointOfIntersection, targetOnUnitSphere - rec.PointOfIntersection);
        attenuation = albedo;
        return true;
    }
}

internal class Metal(Vec3 albedo, double fuzziness) : Material
{
    readonly Vec3 _albedo = albedo;
    readonly double _fuzziness = fuzziness < 1 ? fuzziness : 1;

    public override bool Scatter(Ray incidentRay, HitRecord rec, out Vec3 attenuation, out Ray scatteredRay)
    {
        var reflected = Reflect(Vec3.UnitVector(incidentRay.Direction), rec.Normal);
        scatteredRay = new Ray(rec.PointOfIntersection, reflected + _fuzziness * RandomInUnitSphere());
        attenuation = _albedo;
        return Vec3.Dot(scatteredRay.Direction, rec.Normal) > 0;
    }
}

internal class Dielectric(double refractionIndex) : Material
{
    public override bool Scatter(Ray incidentRay, HitRecord rec, out Vec3 attenuation, out Ray scatteredRay)
    {
        attenuation = UnitVector;
        Vec3 outwardNormal;
        double niOverNt;
        double cosine;
        var reflectedRay = Reflect(incidentRay.Direction, rec.Normal);

        if (Vec3.Dot(incidentRay.Direction, rec.Normal) > 0)
        {
            outwardNormal = -1 * rec.Normal;
            niOverNt = refractionIndex;
            cosine = refractionIndex * Vec3.Dot(incidentRay.Direction, rec.Normal) / incidentRay.Direction.Length();
        }
        else
        {
            outwardNormal = rec.Normal;
            niOverNt = 1 / refractionIndex;
            cosine = -Vec3.Dot(incidentRay.Direction, rec.Normal) / incidentRay.Direction.Length();
        }

        var reflectionProbability = Refract(incidentRay.Direction, outwardNormal, niOverNt, out var refractedRay)
            ? Schlick(cosine, refractionIndex)
            : 1;

        scatteredRay = Rng.NextDouble() < reflectionProbability
            ? new Ray(rec.PointOfIntersection, reflectedRay)
            : new Ray(rec.PointOfIntersection, refractedRay);

        return true;
    }
}