using Inheritance.MapObjects;

public interface IPlayerInteraction
{
    void InteractWith(Player player);
}

public class Ownable : IPlayerInteraction
{
    public int Owner { get; private set; } = -1;

    public void InteractWith(Player player)
    {
        Owner = player.Id;
    }
}

public class Combatable : IPlayerInteraction
{
    public Army Army { get; }

    public Combatable(Army army) => Army = army;

    public void InteractWith(Player player)
    {
        if (!player.CanBeat(Army))
            player.Die();
    }
}

public class Collectable : IPlayerInteraction
{
    public Treasure Treasure { get; }

    public Collectable(Treasure treasure) => Treasure = treasure;

    public void InteractWith(Player player)
    {
        player.Consume(Treasure);
    }
}

public class AbstractMapObject
{
    public IReadOnlyList<IPlayerInteraction> Interactions { get; }

    public AbstractMapObject(params IPlayerInteraction[] interactions)
    {
        Interactions = interactions;
    }

    public void InteractWith(Player player)
    {
        foreach (var trait in Interactions)
        {
            if (player.Dead) break;
            trait.InteractWith(player);
        }
    }
}

public class Dwelling : AbstractMapObject
{
    public Dwelling() : base(new Ownable()) { }
}

public class Mine : AbstractMapObject
{
    public Mine(Army army, Treasure treasure)
        : base(new Combatable(army), new Ownable(), new Collectable(treasure)) { }
}

public class Creeps : AbstractMapObject
{
    public Creeps(Army army, Treasure treasure)
        : base(new Combatable(army), new Collectable(treasure)) { }
}

public class Wolves : AbstractMapObject
{
    public Wolves(int power)
        : base(new Combatable(new Army { Power = power })) { }
}

public class ResourcePile : AbstractMapObject
{
    public ResourcePile(Treasure treasure)
        : base(new Collectable(treasure)) { }
}

public static class Interaction
{
    public static void Make(Player player, AbstractMapObject mapObject)
    {
        mapObject.InteractWith(player);
    }
}