using System;

namespace Inheritance.DataStructure;

/// <summary>
/// Категория сообщения: продукт + тип + тема
/// </summary>
public class Category : IComparable
{
    public string Product { get; }
    public MessageType Type { get; }
    public MessageTopic Topic { get; }

    public Category(string product, MessageType type, MessageTopic topic)
    {
        Product = product; // может быть null — это допустимо
        Type = type;
        Topic = topic;
    }

    // ============ СРАВНЕНИЕ ============

    public int CompareTo(object obj)
    {
        // Если obj == null — считаем, что текущий объект "больше"
        if (obj is null)
            return 1;

        // Если obj не Category — не падаем, а считаем "больше"
        if (obj is not Category other)
            return 1;

        // 1. Сравниваем продукт (null считается меньше любого не-null)
        int productComparison = string.Compare(Product, other.Product, StringComparison.Ordinal);
        if (productComparison != 0)
            return productComparison;

        // 2. Сравниваем тип
        int typeComparison = Type.CompareTo(other.Type);
        if (typeComparison != 0)
            return typeComparison;

        // 3. Сравниваем тему
        return Topic.CompareTo(other.Topic);
    }

    // ============ EQUALS ============

    public override bool Equals(object obj)
    {
        if (obj is not Category other)
            return false;

        return Product == other.Product
            && Type == other.Type
            && Topic == other.Topic;
    }

    // ============ GET HASH CODE ============

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 31 + (Product?.GetHashCode() ?? 0);
            hash = hash * 31 + Type.GetHashCode();
            hash = hash * 31 + Topic.GetHashCode();
            return hash;
        }
    }

    // ============ TO STRING ============

    public override string ToString()
    {
        return $"{Product}.{Type}.{Topic}";
    }

    // ============ ОПЕРАТОРЫ СРАВНЕНИЯ ============

    public static bool operator ==(Category left, Category right)
    {
        // Оба null → равны
        if (left is null && right is null)
            return true;

        // Один из них null → не равны
        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    public static bool operator !=(Category left, Category right)
    {
        return !(left == right);
    }

    public static bool operator <(Category left, Category right)
    {
        // Если left null → считаем, что null меньше любого не-null
        if (left is null)
            return !(right is null);

        // Если right null → left > null, значит left < null = false
        if (right is null)
            return false;

        return left.CompareTo(right) < 0;
    }

    public static bool operator >(Category left, Category right)
    {
        if (left is null)
            return false;

        if (right is null)
            return !(left is null);

        return left.CompareTo(right) > 0;
    }

    public static bool operator <=(Category left, Category right)
    {
        return !(left > right);
    }

    public static bool operator >=(Category left, Category right)
    {
        return !(left < right);
    }
}