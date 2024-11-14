using System;
using System.Runtime.CompilerServices;

namespace RayTracingInOneWeekend;

// In C#, we cannot create a generic Vec3<T> where T could be int, long,
// float, or double. Using where to constrain T by design has no "where
// numeric type". It's only possible to create a Vec3<T> and dispatch on
// type at runtime. For some types this is okay, but not for a Vec3 on the
// hot path.
//
// To avoid heap allocation, Vec3 is implemented as a struct instead of a
// class. That way allocations happen on the stack instead. Turns out in
// .NET, there's negligible difference between float and double, so we went
// with double.
internal readonly struct Vec3(double e0, double e1, double e2)
{
    public double X { get; } = e0;
    public double Y { get; } = e1;
    public double Z { get; } = e2;

    public double R => X;
    public double G => Y;
    public double B => Z;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double Length() => Math.Sqrt(X * X + Y * Y + Z * Z);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double SquaredLength() => X * X + Y * Y + Z * Z;

    // In C#, overloading a binary operator automatically overloads its
    // compound equivalent, i.e., we get both Vec3 + Vec3 and Vec3 +=
    // Vec3 with a single overload.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec3 operator+(Vec3 v1, Vec3 v2) => new(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec3 operator-(Vec3 v1, Vec3 v2) => new (v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec3 operator*(Vec3 v1, Vec3 v2) => new (v1.X * v2.X, v1.Y * v2.Y, v1.Z * v2.Z);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec3 operator*(Vec3 v, double t) => new (v.X * t, v.Y * t, v.Z * t);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec3 operator*(double t, Vec3 v) => new (v.X * t, v.Y * t, v.Z * t);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec3 operator/(Vec3 v, double t) => new (v.X / t, v.Y / t, v.Z / t);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec3 UnitVector(Vec3 v) => v / v.Length();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double Dot(Vec3 v1, Vec3 v2) => v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vec3 Cross(Vec3 v1, Vec3 v2) =>
        new (v1.Y * v2.Z - v1.Z * v2.Y,
            v1.Z * v2.X - v1.X * v2.Z,
            v1.X * v2.Y - v1.Y * v2.X);
}