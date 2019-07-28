using System;

namespace RayTracingInOneWeekend
{
    class Camera
    {
        Vec3 _lowerLeftCorner;
        Vec3 _horizontal;
        Vec3 _vertical;
        Vec3 _origin;
        Vec3 _u, _v, _w;
        readonly float _lensRadius;
        readonly Random _rng = new Random();

        // virticalFieldOfViewDegrees is top to bottom in degrees.
        public Camera(Vec3 lookFrom, Vec3 lookAt, Vec3 viewUp, float virticalFieldOfViewDegrees, float aspectRatio, float aperture, float focusDistance)
        {
            _lensRadius = aperture / 2;
            float theta = (float)(virticalFieldOfViewDegrees * Math.PI / 180);
            var halfHeight = MathF.Tan(theta / 2);
            var halfWidth = aspectRatio * halfHeight;

            _origin = lookFrom;
            _w = Vec3.UnitVector(lookFrom - lookAt);
            _u = Vec3.UnitVector(Vec3.Cross(viewUp, _w));
            _v = Vec3.Cross(_w, _u);

            _lowerLeftCorner = _origin - halfWidth * focusDistance * _u - halfHeight * focusDistance *_v - focusDistance * _w;
            _horizontal = 2 * halfWidth * focusDistance * _u;
            _vertical = 2 * halfHeight * focusDistance * _v;
        }

        public Ray GetRay(float s, float t)
        {
            var rayDirection = _lensRadius * RandomInUnitDisk();
            var offset = _u * rayDirection.X + _v * rayDirection.Y;
            return new Ray(_origin + offset, _lowerLeftCorner + s * _horizontal + t * _vertical - _origin - offset);
        }

        private Vec3 RandomInUnitDisk()
        {
            Vec3 p;
            var size = new Vec3(1, 1, 0);
            do
            {
                p = 2f * new Vec3((float)_rng.NextDouble(), (float)_rng.NextDouble(), 0) - size;
            }
            while (Vec3.Dot(p, p) >= 1f);
            return p;
        }
    }
}