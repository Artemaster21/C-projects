using System;

namespace Incapsulation.EnterpriseTask;

public class Transaction
{
    // Поля лучше сделать свойствами
    public double Amount { get; set; }
    public Guid EnterpriseGuid { get; set; }
}