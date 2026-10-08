using System;
using System.Linq;

namespace Incapsulation.EnterpriseTask;

public class Enterprise
{
    // Автосвойство только для чтения
    public Guid Guid { get; }

    // Автосвойство
    public string Name { get; set; }

    // Свойство с валидацией (не автосвойство)
    private string _inn;
    public string Inn
    {
        get => _inn;
        set
        {
            if (string.IsNullOrEmpty(value) || value.Length != 10 || !value.All(char.IsDigit))
                throw new ArgumentException("INN должен содержать ровно 10 цифр");
            _inn = value;
        }
    }

    // Автосвойство
    public DateTime EstablishDate { get; set; }

    // Вычисляемое свойство (только get)
    public TimeSpan ActiveTimeSpan => DateTime.Now - EstablishDate;

    public Enterprise(Guid guid)
    {
        Guid = guid;
    }

    public double GetTotalTransactionsAmount()
    {
        DataBase.OpenConnection();
        return DataBase.Transactions()
            .Where(t => t.EnterpriseGuid == Guid)
            .Sum(t => t.Amount);
    }
}