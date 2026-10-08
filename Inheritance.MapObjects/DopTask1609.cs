using Inheritance.MapObjects;

public interface IOwnable { int Owner { get; set; } }
public interface IHasArmy { Army Army { get; set; } }
public interface IHasTreasure { Treasure Treasure { get; set; } }

public class Dwelling : IOwnable
{
    public int Owner { get; set; }
}

public class Mine : IOwnable, IHasArmy, IHasTreasure
{
    public int Owner { get; set; }
    public Army Army { get; set; }
    public Treasure Treasure { get; set; }
}

public class Creeps : IHasArmy, IHasTreasure
{
    public Army Army { get; set; }
    public Treasure Treasure { get; set; }
}

public class Wolves : IHasArmy
{
    public Army Army { get; set; }
}

public class ResourcePile : IHasTreasure
{
    public Treasure Treasure { get; set; }
}

public static class Interaction
{
    public static void Make(Player player, object mapObject)
    {
        // 1. Сначала сражаемся (если есть армия)
        if (mapObject is IHasArmy hasArmy)
        {
            if (!player.CanBeat(hasArmy.Army))
            {
                player.Die();
                return; // Проиграли — выходим, ничего не присваиваем и не собираем
            }
        }

        // 2. Присваиваем (если можно)
        if (mapObject is IOwnable ownable)
        {
            ownable.Owner = player.Id;
        }

        // 3. Собираем сокровище (если есть)
        if (mapObject is IHasTreasure hasTreasure)
        {
            player.Consume(hasTreasure.Treasure);
        }
    }
}