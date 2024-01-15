using System.Collections.Generic;

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


    public override string ToString()
    {
        var items = new List<string>();
        foreach ((HandleableItemSo key, int value) in Items)
        {
            items.Add($"{key.name} x{value}");
        }

        return
            "Order"
            + $"\tCustomer: {Customer.name}"
            + $"\tType: {Type}" 
            + (Type is OrderType.Request ? $"\tTime Limit: {TimeLimit}" : "")
            + "\tItems:"
            + string.Join("\t\t", items);
    }
}