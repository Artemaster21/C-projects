using Inheritance.Geometry.Visitor;

namespace Inheritance.Geometry.Virtual;

public abstract class Body
{
    public Vector3 Position { get; }

    protected Body(Vector3 position)
    {
        Position = position;
    }

    public abstract bool ContainsPoint(Vector3 point);
    public abstract RectangularCuboid GetBoundingBox();
}

public class Ball : Body
{
    public double Radius { get; }

    public Ball(Vector3 position, double radius) : base(position)
    {
        Radius = radius;
    }

    public override bool ContainsPoint(Vector3 point)
    {
        var vector = point - Position;
        var length2 = vector.GetLength2();
        return length2 <= Radius * Radius;
    }

    public override RectangularCuboid GetBoundingBox()
    {
        var length = Radius * 2;
        return new RectangularCuboid(Position, length, length, length);
    }
}

public class RectangularCuboid : Body
{
    public double SizeX { get; }
    public double SizeY { get; }
    public double SizeZ { get; }

    public RectangularCuboid(Vector3 position, double sizeX, double sizeY, double sizeZ) : base(position)
    {
        SizeX = sizeX;
        SizeY = sizeY;
        SizeZ = sizeZ;
    }

    public override bool ContainsPoint(Vector3 point)
    {
        var minPoint = new Vector3(
            Position.X - SizeX / 2,
            Position.Y - SizeY / 2,
            Position.Z - SizeZ / 2);
        var maxPoint = new Vector3(
            Position.X + SizeX / 2,
            Position.Y + SizeY / 2,
            Position.Z + SizeZ / 2);

        return point >= minPoint && point <= maxPoint;
    }

    public override RectangularCuboid GetBoundingBox()
    {
        // BoundingBox дл€ параллелепипеда Ч это он сам
        return new RectangularCuboid(Position, SizeX, SizeY, SizeZ);
    }
}

public class Cylinder : Body
{
    public double SizeZ { get; }
    public double Radius { get; }

    public Cylinder(Vector3 position, double sizeZ, double radius) : base(position)
    {
        SizeZ = sizeZ;
        Radius = radius;
    }

    public override bool ContainsPoint(Vector3 point)
    {
        var vectorX = point.X - Position.X;
        var vectorY = point.Y - Position.Y;
        var length2 = vectorX * vectorX + vectorY * vectorY;
        var minZ = Position.Z - SizeZ / 2;
        var maxZ = minZ + SizeZ;

        return length2 <= Radius * Radius && point.Z >= minZ && point.Z <= maxZ;
    }

    public override RectangularCuboid GetBoundingBox()
    {
        return new RectangularCuboid(
            Position,
            Radius * 2,
            Radius * 2,
            SizeZ);
    }
}

public class CompoundBody : Body
{
    public IReadOnlyList<Body> Parts { get; }

    public CompoundBody(IReadOnlyList<Body> parts) : base(parts[0].Position)
    {
        Parts = parts;
    }

    public override bool ContainsPoint(Vector3 point)
    {
        return Parts.Any(body => body.ContainsPoint(point));
    }

    public override RectangularCuboid GetBoundingBox()
    {
        var minPoint = GetMinPoint();
        var maxPoint = GetMaxPoint();
        return CreateBoundingBox(minPoint, maxPoint);
    }

    private Vector3 GetMinPoint()
    {
        var minX = double.MaxValue;
        var minY = double.MaxValue;
        var minZ = double.MaxValue;

        foreach (var part in Parts)
        {
            var box = part.GetBoundingBox();
            minX = Math.Min(minX, box.Position.X - box.SizeX / 2);
            minY = Math.Min(minY, box.Position.Y - box.SizeY / 2);
            minZ = Math.Min(minZ, box.Position.Z - box.SizeZ / 2);
        }

        return new Vector3(minX, minY, minZ);
    }

    private Vector3 GetMaxPoint()
    {
        var maxX = double.MinValue;
        var maxY = double.MinValue;
        var maxZ = double.MinValue;

        foreach (var part in Parts)
        {
            var box = part.GetBoundingBox();
            maxX = Math.Max(maxX, box.Position.X + box.SizeX / 2);
            maxY = Math.Max(maxY, box.Position.Y + box.SizeY / 2);
            maxZ = Math.Max(maxZ, box.Position.Z + box.SizeZ / 2);
        }

        return new Vector3(maxX, maxY, maxZ);
    }

    private static RectangularCuboid CreateBoundingBox(Vector3 minPoint, Vector3 maxPoint)
    {
        var sizeX = maxPoint.X - minPoint.X;
        var sizeY = maxPoint.Y - minPoint.Y;
        var sizeZ = maxPoint.Z - minPoint.Z;

        var center = new Vector3(
            (minPoint.X + maxPoint.X) / 2,
            (minPoint.Y + maxPoint.Y) / 2,
            (minPoint.Z + maxPoint.Z) / 2);

        return new RectangularCuboid(center, sizeX, sizeY, sizeZ);
    }
}