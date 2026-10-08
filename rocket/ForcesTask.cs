using System;

namespace func_rocket;

public class ForcesTask
{
    /// <summary>
    /// Создает делегат, возвращающий по ракете вектор силы тяги двигателей этой ракеты.
    /// Сила тяги направлена вдоль ракеты и равна по модулю forceValue.
    /// </summary>
    public static RocketForce GetThrustForce(double forceValue)
    {
        // Возвращаем функцию, которая для данной ракеты вычисляет силу тяги
        return rocket =>
        {
            // Направление ракеты (угол в радианах)
            var angle = rocket.Direction;

            // Вектор направления (единичной длины)
            Vector direction = new Vector(Math.Cos(angle), Math.Sin(angle));

            // Умножаем направление на величину силы
            return direction * forceValue;
        };
    }

    /// <summary>
    /// Преобразует делегат силы гравитации, в делегат силы, действующей на ракету
    /// </summary>
    public static RocketForce ConvertGravityToForce(Gravity gravity, Vector spaceSize)
    {
        return rocket => gravity(spaceSize, rocket.Location);
    }

    /// <summary>
    /// Суммирует все переданные силы, действующие на ракету, и возвращает суммарную силу.
    /// </summary>
    public static RocketForce Sum(params RocketForce[] forces)
    {
        // Возвращаем функцию, которая суммирует все силы
        return rocket =>
        {
            Vector total = Vector.Zero;

            foreach (var force in forces)
                total += force(rocket);

            return total;
        };
    }
}