namespace Inheritance.Geometry.Visitor;

public interface IVisitor<TResult>
{
    TResult Visit(Ball ball);
    TResult Visit(RectangularCuboid cuboid);
    TResult Visit(Cylinder cylinder);
    TResult Visit(CompoundBody compound);
}

public abstract class Body
{
    public Vector3 Position { get; }

    protected Body(Vector3 position)
    {
        Position = position;
    }

    public abstract TResult Accept<TResult>(IVisitor<TResult> visitor);
}

public class Ball : Body
{
    public double Radius { get; }

    public Ball(Vector3 position, double radius) : base(position)
    {
        Radius = radius;
    }

    public override TResult Accept<TResult>(IVisitor<TResult> visitor)
    {
        return visitor.Visit(this);
    }
}

public class RectangularCuboid : Body
{
    public double SizeX { get; }
    public double SizeY { get; }
    public double SizeZ { get; }

    public RectangularCuboid(Vector3 position, double sizeX, double sizeY, double sizeZ)
        : base(position)
    {
        SizeX = sizeX;
        SizeY = sizeY;
        SizeZ = sizeZ;
    }

    public override TResult Accept<TResult>(IVisitor<TResult> visitor)
    {
        return visitor.Visit(this);
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

    public override TResult Accept<TResult>(IVisitor<TResult> visitor)
    {
        return visitor.Visit(this);
    }
}

public class CompoundBody : Body
{
    public IReadOnlyList<Body> Parts { get; }

    public CompoundBody(IReadOnlyList<Body> parts) : base(parts[0].Position)
    {
        Parts = parts;
    }

    public override TResult Accept<TResult>(IVisitor<TResult> visitor)
    {
        return visitor.Visit(this);
    }
}

public class BoundingBoxVisitor : IVisitor<RectangularCuboid>
{
    public RectangularCuboid Visit(Ball ball)
    {
        var side = ball.Radius * 2;
        return new RectangularCuboid(ball.Position, side, side, side);
    }

    public RectangularCuboid Visit(RectangularCuboid cuboid)
    {
        return new RectangularCuboid(cuboid.Position, cuboid.SizeX, cuboid.SizeY, cuboid.SizeZ);
    }

    public RectangularCuboid Visit(Cylinder cylinder)
    {
        return new RectangularCuboid(cylinder.Position, cylinder.Radius * 2, cylinder.Radius * 2, cylinder.SizeZ);
    }

    public RectangularCuboid Visit(CompoundBody compound)
    {
        double minX = double.MaxValue, minY = double.MaxValue, minZ = double.MaxValue;
        double maxX = double.MinValue, maxY = double.MinValue, maxZ = double.MinValue;

        foreach (var part in compound.Parts)
        {
            // ✅ ТЕПЕРЬ БЕЗ ПРИВЕДЕНИЙ! part.Accept(this) возвращает RectangularCuboid
            var box = part.Accept(this);

            minX = Math.Min(minX, box.Position.X - box.SizeX / 2);
            minY = Math.Min(minY, box.Position.Y - box.SizeY / 2);
            minZ = Math.Min(minZ, box.Position.Z - box.SizeZ / 2);
            maxX = Math.Max(maxX, box.Position.X + box.SizeX / 2);
            maxY = Math.Max(maxY, box.Position.Y + box.SizeY / 2);
            maxZ = Math.Max(maxZ, box.Position.Z + box.SizeZ / 2);
        }

        var sizeX = maxX - minX;
        var sizeY = maxY - minY;
        var sizeZ = maxZ - minZ;
        var center = new Vector3((minX + maxX) / 2, (minY + maxY) / 2, (minZ + maxZ) / 2);

        return new RectangularCuboid(center, sizeX, sizeY, sizeZ);
    }
}

public class BoxifyVisitor : IVisitor<Body>
{
    public Body Visit(Ball ball)
    {
        var side = ball.Radius * 2;
        return new RectangularCuboid(ball.Position, side, side, side);
    }

    public Body Visit(RectangularCuboid cuboid)
    {
        return new RectangularCuboid(cuboid.Position, cuboid.SizeX, cuboid.SizeY, cuboid.SizeZ);
    }

    public Body Visit(Cylinder cylinder)
    {
        return new RectangularCuboid(cylinder.Position, cylinder.Radius * 2, cylinder.Radius * 2, cylinder.SizeZ);
    }

    public Body Visit(CompoundBody compound)
    {
        var boxifiedParts = new List<Body>();
        foreach (var part in compound.Parts)
        {
            // ✅ Без приведений! part.Accept(this) возвращает Body
            boxifiedParts.Add(part.Accept(this));
        }
        return new CompoundBody(boxifiedParts);
    }
}