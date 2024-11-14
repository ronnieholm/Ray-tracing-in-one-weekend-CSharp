namespace RayTracingInOneWeekend;

internal readonly struct Ray(Vec3 origin, Vec3 direction)
{
    public Vec3 Origin { get; } = origin;
    public Vec3 Direction { get; } = direction;

    public Vec3 PointAtParameter(double t) => Origin + t * Direction;
}