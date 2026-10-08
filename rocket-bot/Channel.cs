using System.Collections.Generic;

namespace rocket_bot;

public class Channel<T> where T : class
{
    private readonly List<T> items = new List<T>();
    private readonly object locker = new object();

    /// <summary>
    /// Возвращает элемент по индексу или null, если такого элемента нет.
    /// При присвоении удаляет все элементы после.
    /// Если индекс в точности равен размеру коллекции, работает как Append.
    /// </summary>
    public T this[int index]
    {
        get
        {
            lock (locker)
            {
                if (index < 0 || index >= items.Count)
                    return null;
                return items[index];
            }
        }
        set
        {
            lock (locker)
            {
                if (index < 0)
                    return;

                if (index == items.Count)
                {
                    // Append
                    items.Add(value);
                }
                else if (index < items.Count)
                {
                    // Заменяем элемент и удаляем всё после него
                    items[index] = value;
                    int removeCount = items.Count - (index + 1);
                    if (removeCount > 0)
                        items.RemoveRange(index + 1, removeCount);
                }
            }
        }
    }

    /// <summary>
    /// Возвращает последний элемент или null, если такого элемента нет
    /// </summary>
    public T LastItem()
    {
        lock (locker)
        {
            if (items.Count == 0)
                return null;
            return items[items.Count - 1];
        }
    }

    /// <summary>
    /// Добавляет item в конец только если lastItem является последним элементом
    /// </summary>
    public void AppendIfLastItemIsUnchanged(T item, T knownLastItem)
    {
        lock (locker)
        {
            T currentLast = items.Count == 0 ? null : items[items.Count - 1];

            // Проверяем, совпадает ли известный последний элемент с текущим последним
            if (currentLast == knownLastItem ||
                (currentLast != null && currentLast.Equals(knownLastItem)))
            {
                items.Add(item);
            }
        }
    }

    /// <summary>
    /// Возвращает количество элементов в коллекции
    /// </summary>
    public int Count
    {
        get
        {
            lock (locker)
            {
                return items.Count;
            }
        }
    }
}