using System;
using System.Collections.Generic;
using JetBrains.Annotations;

public enum OrderType
{
    Direct,
    Request
}

public class Order
{
    public Customer Customer { get; }
    public OrderType Type { get; }
    public Dictionary<HandleableItemSo, int> Items { get; }
    public float TimeLimit { get; }
    
    public Order(Customer customer, OrderType type, Dictionary<HandleableItemSo, int> items, float timeLimit = default)
    {
        Customer = customer;
        Type = type;
        Items = items;
        TimeLimit = timeLimit;
    }
}