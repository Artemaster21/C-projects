namespace Geometry;

public class Vector
{
    public double X;
    public double Y;
}

public class Segment
{
    public Vector Begin;
    public Vector End;
}

public static class Geometry
{
    public static double GetLength(Vector vector)
    {
        return Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
    }

    public static Vector Add(Vector first, Vector second)
    {
        return new Vector
        {
            X = first.X + second.X,
            Y = first.Y + second.Y
        };
    }

    public static double GetLength(Segment segment)
    {
        double deltaX = segment.End.X - segment.Begin.X;
        double deltaY = segment.End.Y - segment.Begin.Y;
        return Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
    }

    public static bool IsVectorInSegment(Vector vector, Segment segment)
    {
        if (segment.Begin.X == segment.End.X && segment.Begin.Y == segment.End.Y)
        {
            return vector.X == segment.Begin.X && vector.Y == segment.Begin.Y;
        }

        var crossProduct = (vector.X - segment.Begin.X) * (segment.End.Y - segment.Begin.Y) -
                             (vector.Y - segment.Begin.Y) * (segment.End.X - segment.Begin.X);

        if (Math.Abs(crossProduct) > 1e-10)
            return false;

        var dotProduct = (vector.X - segment.Begin.X) * (segment.End.X - segment.Begin.X) +
                           (vector.Y - segment.Begin.Y) * (segment.End.Y - segment.Begin.Y);

        if (dotProduct < 0)
            return false;

        var segmentLengthSquared = (segment.End.X - segment.Begin.X) * (segment.End.X - segment.Begin.X) +
                                     (segment.End.Y - segment.Begin.Y) * (segment.End.Y - segment.Begin.Y);

        return !(dotProduct > segmentLengthSquared);
    }
}