using System.Collections.Generic;

public enum OrderType
{
    Direct,
    Request
}

public class Order
{
    public Customer Customer { get; set; }
    public OrderType Type { get; }
    public Dictionary<ProductSo, int> Items { get; }
    public float TimeLimit { get; set; }
    
    public Order(OrderType type, Dictionary<ProductSo, int> items, Customer customer = null, float timeLimit = default)
    {
        Customer = customer;
        Type = type;
        Items = items;
        TimeLimit = timeLimit;
    }


    public override string ToString()
    {
        var items = new List<string>();
        foreach ((ProductSo key, int value) in Items)
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