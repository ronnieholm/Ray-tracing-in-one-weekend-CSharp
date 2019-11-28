using System;

namespace RayTracingInOneWeekend
{
    struct HitRecord
    {
        public double T;
        public Vec3 PointOfIntersection;
        public Vec3 Normal;
        public Material Material;
    }

    abstract class Hitable
    {
        public abstract bool Hit(Ray r, double tMin, double tMax, ref HitRecord record);
    }

    class HitableItems : Hitable
    {
        Hitable[] _hitables;

        public HitableItems(Hitable[] hitables)
        {
            _hitables = hitables;
        }

        public override bool Hit(Ray r, double tMin, double tMax, ref HitRecord record)
        {
            var hitAnything = false;
            var closestSoFar = tMax;

            for (var i = 0; i < _hitables.Length; i++)
            {
                if (_hitables[i].Hit(r, tMin, closestSoFar, ref record))
                {
                    hitAnything = true;
                    closestSoFar = record.T;
                }
            }

            return hitAnything;
        }
    }

    class Sphere : Hitable
    {
        Vec3 _center;
        double _radius;
        Material _material;

        public Sphere(Vec3 center, double radius, Material material)
        {
            _center = center;
            _radius = radius;
            _material = material;
        }

        public override bool Hit(Ray r, double tMin, double tMax, ref HitRecord record)
        {
            var oc = r.Origin - _center;
            var a = Vec3.Dot(r.Direction, r.Direction);
            var b = Vec3.Dot(oc, r.Direction);
            var c = Vec3.Dot(oc, oc) - _radius * _radius;
            var discriminant = b * b - a * c;
            
            if (discriminant > 0)
            {
                var sqrtDiscriminant = Math.Sqrt(discriminant);
                var solution1 = (-b - sqrtDiscriminant) / a;
                if (solution1 < tMax && solution1 > tMin)
                {
                    record.T = solution1;
                    record.PointOfIntersection = r.PointAtParameter(record.T);

                    // Normal is computed by computing the vector center to
                    // point of intersection Dividing by radius causes this
                    // vector to become a unit vector.
                    record.Normal = (record.PointOfIntersection - _center) / _radius;
                    record.Material = _material;
                    return true;
                }

                var solution2 = (-b + sqrtDiscriminant) / a;
                if (solution2 < tMax && solution2 > tMin)
                {
                    record.T = solution2;
                    record.PointOfIntersection = r.PointAtParameter(record.T);
                    record.Normal = (record.PointOfIntersection - _center) / _radius;
                    record.Material = _material;
                    return true;
                }
            }

            return false;
        }
    }
}