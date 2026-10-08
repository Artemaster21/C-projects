using System;
using System.Collections.Generic;

namespace Incapsulation.EnterpriseTask;

public static class DataBase
{
    private static bool _isOpen;

    public static void OpenConnection()
    {
        if (_isOpen)
            return;

        // Реализация открытия соединения
        _isOpen = true;
        Console.WriteLine("Database connection opened");
    }

    public static void CloseConnection()
    {
        if (!_isOpen)
            return;

        _isOpen = false;
        Console.WriteLine("Database connection closed");
    }

    public static IEnumerable<Transaction> Transactions()
    {
        // Здесь должна быть реальная реализация
        // Пока возвращаем пустой список
        return new List<Transaction>();
    }
}