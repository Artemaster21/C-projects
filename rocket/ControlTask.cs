using System;

namespace func_rocket;

public class ControlTask
{
    private const double DirectionWeight = 1.0;
    private const double VelocityWeight = 3.0;
    private const double TargetShift = 0.5;
    private const double Eps = 0.05;

    public static Turn ControlRocket(Rocket rocket, Vector target)
    {
        Vector toTarget = target - rocket.Location;

        if (toTarget.Length < 10)
            return Turn.None;

        // Вычисляем углы
        double directionAngle = GetAngleToTarget(rocket.Direction, toTarget);
        double velocityAngle = GetAngle(rocket.Velocity, toTarget);

        // Специальная обработка для WhiteHole
        if (IsWhiteHoleLevel(rocket, target))
        {
            velocityAngle = CompensateWhiteHole(rocket, target, velocityAngle);
        }

        double totalWeight = DirectionWeight + VelocityWeight;
        double avgAngle = (DirectionWeight * directionAngle + VelocityWeight * velocityAngle) / totalWeight;

        if (Math.Abs(avgAngle) < Eps)
            return Turn.None;

        return avgAngle > 0 ? Turn.Right : Turn.Left;
    }

    private static double GetAngleToTarget(double rocketDirection, Vector toTarget)
    {
        double targetAngle = Math.Atan2(toTarget.Y, toTarget.X);
        double diff = targetAngle - rocketDirection;

        while (diff > Math.PI) diff -= 2 * Math.PI;
        while (diff < -Math.PI) diff += 2 * Math.PI;

        return diff;
    }

    private static double GetAngle(Vector from, Vector to)
    {
        if (from.Length == 0 || to.Length == 0)
            return 0;

        double fromAngle = Math.Atan2(from.Y, from.X);
        double toAngle = Math.Atan2(to.Y, to.X);

        double diff = toAngle - fromAngle;
        while (diff > Math.PI) diff -= 2 * Math.PI;
        while (diff < -Math.PI) diff += 2 * Math.PI;

        return diff;
    }

    private static bool IsWhiteHoleLevel(Rocket rocket, Vector target)
    {
        // Вектор от цели к ракете (направление отталкивания)
        Vector fromTarget = rocket.Location - target;

        // Если вектор не нулевой, нормализуем
        if (fromTarget.Length == 0)
            return false;

        Vector gravityDirection = fromTarget.Normalize();

        // Вычисляем скалярное произведение вручную
        double dot = rocket.Velocity.X * gravityDirection.X + rocket.Velocity.Y * gravityDirection.Y;

        // Если скорость направлена от цели (положительное скалярное произведение)
        return dot > 0;
    }

    private static double CompensateWhiteHole(Rocket rocket, Vector target, double velocityAngle)
    {
        Vector toTarget = target - rocket.Location;
        double distance = toTarget.Length;

        // Сила отталкивания на уровне WhiteHole: 140 * d / (d² + 1)
        double repulsionForce = 140 * distance / (distance * distance + 1);

        // Корректируем угол с учётом отталкивания
        double compensation = repulsionForce * 0.1;

        if (Math.Abs(velocityAngle) < 0.5)
        {
            return velocityAngle + compensation * Math.Sign(velocityAngle);
        }

        return velocityAngle;
    }
}