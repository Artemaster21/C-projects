using System;
using System.Collections.Generic;

namespace func_rocket;

public class LevelsTask
{
    static readonly Physics standardPhysics = new();

    private static readonly Rocket _rocket = new Rocket(new Vector(200, 500), Vector.Zero, -0.5 * Math.PI);
    private static readonly Vector _target = new Vector(600, 200);
    private static readonly Vector _targetUp = new Vector(700, 500);

    public static IEnumerable<Level> CreateLevels()
    {
        yield return CreateLevel("Zero", (size, v) => Vector.Zero);
        yield return CreateLevel("Heavy", (size, v) => new Vector(0, 0.9));
        yield return CreateLevel("Up", GetUpGravity, _targetUp);
        yield return CreateLevel("WhiteHole", GetWhiteHoleGravity);
        yield return CreateLevel("BlackHole", GetBlackHoleGravity);
        yield return CreateLevel("BlackAndWhite", GetBlackAndWhiteGravity);
    }

    private static Level CreateLevel(string name, Gravity gravity, Vector? target = null)
    {
        return new Level(name, _rocket, target ?? _target, gravity, standardPhysics);
    }

    // Уровень Up - гравитация зависит от высоты
    private static Vector GetUpGravity(Vector size, Vector v)
    {
        // Размер поля передаётся через параметр size типа Vector
        // size.X - ширина, size.Y - высота
        double d = size.Y - v.Y;  // расстояние от нижнего края
        double magnitude = 300 / (d + 300.0);
        return new Vector(0, magnitude);
    }

    // Уровень WhiteHole - гравитация от цели
    private static Vector GetWhiteHoleGravity(Vector size, Vector v)
    {
        double d = (v - _target).Length;  // Length - свойство, не метод!
        double magnitude = 140 * d / (d * d + 1);

        Vector direction = (v - _target).Normalize();
        return direction * magnitude;
    }

    // Уровень BlackHole - гравитация к аномалии
    private static Vector GetBlackHoleGravity(Vector size, Vector v)
    {
        Vector anomaly = (_rocket.Location + _target) / 2;

        double d = (v - anomaly).Length;  // Length - свойство
        double magnitude = 300 * d / (d * d + 1);

        Vector direction = (anomaly - v).Normalize();
        return direction * magnitude;
    }

    // Уровень BlackAndWhite - среднее арифметическое
    private static Vector GetBlackAndWhiteGravity(Vector size, Vector v)
    {
        Vector whiteHole = GetWhiteHoleGravity(size, v);
        Vector blackHole = GetBlackHoleGravity(size, v);

        return (whiteHole + blackHole) / 2;
    }
}