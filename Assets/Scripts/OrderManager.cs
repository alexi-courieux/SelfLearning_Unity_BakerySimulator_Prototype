using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class OrderManager : MonoBehaviour
{
    
    public static OrderManager Instance { get; private set; }

    private const float MinimumRequestTimeLimit = 1f;
    private const float MinimumPerItemRequestTimeLimit = 3f;
    private const float MaximumPerItemRequestTimeLimit = 6f;
    private const float PhoneRequestFrequency = 3f;
    private const float PhoneRequestChance = 0.5f;

    public EventHandler OnRequestListChanged;
    
    [SerializeField] private SellableItemDictionarySo sellableItemDictionarySo;
    [SerializeField] private Transform[] displayStations;
    [SerializeField] private PhoneStation phoneStation;
    private IDisplayItems[] _displayStations;
    
    private List<Order> _requests;
    private float _phoneRequestTimer;

    public List<Order> Requests => _requests;

    private List<HandleableItemSo> AvailableItems => _displayStations.SelectMany(s => s.GetItemsSo()).ToList();

    private void Awake()
    {
        Instance = this;
        _requests = new List<Order>();
        _displayStations = displayStations.Select(s => s.GetComponent<IDisplayItems>()).ToArray();
        _phoneRequestTimer = PhoneRequestFrequency;
    }

    private void Update()
    {
        _phoneRequestTimer -= Time.deltaTime;
        if (_phoneRequestTimer <= 0f)
        {
            if (Random.value < PhoneRequestChance)
            {
                phoneStation.Ring();
            }
            _phoneRequestTimer = PhoneRequestFrequency;
        }
    }

    public Order CreateOrder(OrderType type, Customer customer = null)
    {
        return type switch
        {
            OrderType.Direct => CreateDirectOrder(customer),
            OrderType.Request => CreateRequestOrder(customer),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Invalid order type")
        };
    }

    /// <summary>
    /// Create a new direct order with items in stock
    /// </summary>
    /// <param name="customer">needing the order</param>
    /// <returns>Order with a random number of sellable items in stock</returns>
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

        Order order = new (OrderType.Direct, items, customer);
        return order;
    }
    
    /// <summary>
    /// Create a new request order
    /// </summary>
    /// <param name="customer">Link the order to a specific customer</param>
    /// <returns>Order with a random number of sellable items</returns>
    private Order CreateRequestOrder(Customer customer = null)
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
        
        Order order = new (OrderType.Request, items, customer, timeLimit);
        return order;
    }
    
    /// <summary>
    /// Add a request to the list and start its countdown
    /// </summary>
    /// <param name="order"></param>
    public void AcceptRequest(Order order)
    {
        _requests.Add(order);
        StartCoroutine(HandleRequests(order));
        OnRequestListChanged?.Invoke(this, EventArgs.Empty);
    }

    public bool HaveRequests(Customer customer)
    {
        return _requests.Any(o => o.Customer == customer);
    }
    
    public void RemoveRequest(Customer customer)
    {
        _requests.RemoveAll(o => o.Customer == customer);
        OnRequestListChanged?.Invoke(this, EventArgs.Empty);
    }

    private IEnumerator HandleRequests(Order request)
    {
        while (request.TimeLimit > 0f)
        {
            yield return new WaitForSeconds(1f);
            request.TimeLimit -= 1f;
            
        }

        if (request.Customer != null)
        {
            CustomerManager.Instance.RespawnForRequest(request.Customer);
        }
        else
        {
            CustomerManager.Instance.SpawnForRequest(request);
        }
    }

    public bool CanPerformDirectOrder()
    {
        return AvailableItems.Count > 0;
    }
}