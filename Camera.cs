using System;

namespace RayTracingInOneWeekend;

internal class Camera
{
    private readonly Vec3 _lowerLeftCorner;
    private readonly Vec3 _horizontal;
    private readonly Vec3 _vertical;
    private readonly Vec3 _origin;
    private readonly Vec3 _u;
    private readonly Vec3 _v;
    private readonly double _lensRadius;
    private readonly Random _rng = new();
    private static readonly Vec3 Size = new(1, 1, 0);

    // verticalFieldOfViewDegrees is top to bottom in degrees.
    public Camera(Vec3 lookFrom, Vec3 lookAt, Vec3 viewUp, double verticalFieldOfViewDegrees, double aspectRatio, double aperture, double focusDistance)
    {
        _lensRadius = aperture / 2;
        var theta = verticalFieldOfViewDegrees * Math.PI / 180;
        var halfHeight = Math.Tan(theta / 2);
        var halfWidth = aspectRatio * halfHeight;

        _origin = lookFrom;
        var w = Vec3.UnitVector(lookFrom - lookAt);
        _u = Vec3.UnitVector(Vec3.Cross(viewUp, w));
        _v = Vec3.Cross(w, _u);

        _lowerLeftCorner = _origin - halfWidth * focusDistance * _u - halfHeight * focusDistance * _v - focusDistance * w;
        _horizontal = 2 * halfWidth * focusDistance * _u;
        _vertical = 2 * halfHeight * focusDistance * _v;
    }

    public Ray GetRay(double s, double t)
    {
        var rayDirection = _lensRadius * RandomInUnitDisk();
        var offset = _u * rayDirection.X + _v * rayDirection.Y;
        return new Ray(_origin + offset, _lowerLeftCorner + s * _horizontal + t * _vertical - _origin - offset);
    }

    private Vec3 RandomInUnitDisk()
    {
        Vec3 p;
        do
        {
            p = 2 * new Vec3(_rng.NextDouble(), _rng.NextDouble(), 0) - Size;
        }
        while (Vec3.Dot(p, p) >= 1);
        return p;
    }
}