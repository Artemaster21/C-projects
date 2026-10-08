using Inheritance.MapObjects;

namespace Inheritance.MapObjects;

public interface IInteraction
{
    bool Apply(Player player);
}

public class FightInteraction : IInteraction
{
    public Army Army { get; set; }

    public bool Apply(Player player)
    {
        if (player.CanBeat(Army))
            return true;

        player.Die();
        return false; 
    }
}

public class OwnInteraction : IInteraction
{
    public int Owner { get; private set; }

    public bool Apply(Player player)
    {
        Owner = player.Id;
        return true;
    }
}

public class ConsumeInteraction : IInteraction
{
    public Treasure Treasure { get; set; }

    public bool Apply(Player player)
    {
        player.Consume(Treasure);
        return true;
    }
}

public abstract class MapObject
{
    public abstract IEnumerable<IInteraction> GetInteractions();

    public void Apply(Player player)
    {
        foreach (var interaction in GetInteractions())
        {
            if (!interaction.Apply(player))
                break;
        }
    }
}

public class Dwelling : MapObject
{
    private readonly OwnInteraction _own = new();

    public int Owner => _own.Owner;

    public override IEnumerable<IInteraction> GetInteractions()
    {
        yield return _own;
    }
}

public class Mine : MapObject
{
    private readonly FightInteraction _fight = new() { Army = new Army { Power = 10 } };
    private readonly OwnInteraction _own = new();
    private readonly ConsumeInteraction _consume = new() { Treasure = new Treasure { Amount = 5 } };

    public int Owner => _own.Owner;

    public override IEnumerable<IInteraction> GetInteractions()
    {
        yield return _fight;
        yield return _own;
        yield return _consume;
    }
}

public class Creeps : MapObject
{
    private readonly FightInteraction _fight = new() { Army = new Army { Power = 3 } };
    private readonly ConsumeInteraction _consume = new() { Treasure = new Treasure { Amount = 10 } };

    public override IEnumerable<IInteraction> GetInteractions()
    {
        yield return _fight;
        yield return _consume;
    }
}

public class Wolves : MapObject
{
    private readonly FightInteraction _fight = new() { Army = new Army { Power = 7 } };

    public override IEnumerable<IInteraction> GetInteractions()
    {
        yield return _fight;
    }
}

public class ResourcePile : MapObject
{
    private readonly ConsumeInteraction _consume = new() { Treasure = new Treasure { Amount = 15 } };

    public override IEnumerable<IInteraction> GetInteractions()
    {
        yield return _consume;
    }
}

public class Army
{
    public int Power { get; set; }
}

public class Player
{
    public int Gold { get; private set; }
    public bool Dead { get; private set; }
    public int Id { get; set; }

    public bool CanBeat(Army army)
    {
        return army.Power < 5;
    }

    public void Consume(Treasure treasure)
    {
        Gold += treasure.Amount;
    }

    public void Die()
    {
        Dead = true;
    }
}

public static class Interaction
{
    public static void Make(Player player, MapObject mapObject)
    {
        mapObject.Apply(player);
    }
}