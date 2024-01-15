using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class OrderManager : MonoBehaviour
{
    
    public static OrderManager Instance { get; private set; }
    
    private const float MinimumRequestTimeLimit = 120f;
    private const float MinimumPerItemRequestTimeLimit = 30f;
    private const float MaximumPerItemRequestTimeLimit = 60f;
    
    [SerializeField] private SellableItemDictionarySo sellableItemDictionarySo;
    [SerializeField] private Transform[] displayStations;
    private IDisplayItems[] _displayStations;
    private List<Order> _orders;

    private List<HandleableItemSo> AvailableItems => _displayStations.SelectMany(s => s.GetItemsSo()).ToList();

    private void Awake()
    {
        Instance = this;
        _orders = new List<Order>();
        _displayStations = displayStations.Select(s => s.GetComponent<IDisplayItems>()).ToArray();
    }

    public Order CreateOrder(Customer customer, OrderType type)
    {
        return type switch
        {
            OrderType.Direct => CreateDirectOrder(customer),
            OrderType.Request => CreateRequestOrder(customer),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Invalid order type")
        };
    }
    
    private Order CreateDirectOrder(Customer customer)
    {
        // iterate through the display stations and get a random item from each
        var items = new Dictionary<HandleableItemSo, int>();
        var availableItems = AvailableItems;
        int orderSize = Mathf.Min(Random.Range(1, 5), availableItems.Count);
        for (int i = 0; i < orderSize; i++)
        {
            HandleableItemSo item = availableItems[Random.Range(0, availableItems.Count)];
            availableItems.Remove(item);
            if (items.ContainsKey(item))
            {
                items[item]++;
            }
            else
            {
                items.Add(item, 1);
            }
        }

        Order order = new (customer, OrderType.Direct, items);
        _orders.Add(order);
        return order;
    }
    
    private Order CreateRequestOrder(Customer customer)
    {
        // iterate through the display stations and get a random item from each
        var items = new Dictionary<HandleableItemSo, int>();
        int orderSize = Random.Range(1, 5);
        for (int i = 0; i < orderSize; i++)
        {
            HandleableItemSo item = sellableItemDictionarySo.sellableItems[Random.Range(0, sellableItemDictionarySo.sellableItems.Count())];
            if (items.ContainsKey(item))
            {
                items[item]++;
            }
            else
            {
                items.Add(item, 1);
            }
        }

        float timeLimit = MinimumRequestTimeLimit + (orderSize * Random.Range(MinimumPerItemRequestTimeLimit, MaximumPerItemRequestTimeLimit));
        
        Order order = new (customer, OrderType.Request, items, timeLimit);
        _orders.Add(order);
        return order;
    }

    public void RemoveOrder(Customer customer)
    {
        _orders.RemoveAll(o => o.Customer == customer);
    }

    public bool CanPerformDirectOrder()
    {
        return AvailableItems.Count > 0;
    }
}