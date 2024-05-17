using System;
using System.Linq;
using UnityEngine;

public class CheckoutStation : MonoBehaviour, IInteractable, IHandleItems
{
    public EventHandler OnAnyCustomerLeave;
    public EventHandler<Order> OnOrderReceived;

    [SerializeField] private Transform itemSlot;
    [SerializeField] private int customerLimit;
    [SerializeField] private Vector3 customerOffset;
    [SerializeField] private Transform customerCheckoutPosition;

    private WaitingQueue<Customer> _customerQueue;
    private readonly StackList<Product> _products = new();
    

    private void Start()
    {
        CustomerManager.Instance.CheckoutStations.Add(this);
        _customerQueue = new WaitingQueue<Customer>(customerLimit, customerOffset, customerCheckoutPosition);
    }

    public void Interact()
    {
        if (Player.Instance.HandleSystem.HaveAnyItems())
        {
            if (!Player.Instance.HandleSystem.HaveItems<Product>())
            {
                Logger.LogWarning("Station can only hold products!");
                return;
            }
            Player.Instance.HandleSystem.GetItem().SetParent<Product>(this);
        }
        else
        {
            if (_products.Count > 0)
            {
                Item item = _products.Pop();
                item.SetParent<Item>(Player.Instance.HandleSystem);
            }
        }
    }

    public void AddItem<T>(Item item) where T : Item
    {
        if (typeof(T) != typeof(Product))
        {
            Logger.LogWarning("This station can only hold products!");
            return;
        }
        
        _products.Push(item as Product);
    }

    public Item[] GetItems<T>() where T : Item
    {
        if (typeof(T) != typeof(Product))
        {
            Logger.LogWarning("This station can only hold products!");
            return null;
        }
        
        return _products.Cast<Item>().ToArray();
    }

    public void ClearItem(Item item)
    {
        _products.Remove(item as Product);
    }

    public bool HaveItems<T>() where T : Item
    {
        if (typeof(T) != typeof(Product))
        {
            Logger.LogWarning("This station can only hold products!");
            return false;
        }
        
        return _products.Count > 0;
    }
    
    public bool HaveAnyItems()
    {
        return _products.Count > 0;
    }
    
    public Transform GetAvailableItemSlot<T>() where T : Item
    {
        if (typeof(T) != typeof(Product))
        {
            Logger.LogWarning("This station can only hold products!");
            return null;
        }
        
        return itemSlot;
    }

    public bool HasAvailableSlot<T>() where T : Item
    {
        if (typeof(T) != typeof(Product))
        {
            Logger.LogWarning("This station can only hold products!");
            return false;
        }
        
        return true;
    }

    public bool IsAvailable()
    {
        return _customerQueue.Count < customerLimit;
    }
    
    public void AddCustomer(Customer customer)
    {
        _customerQueue.Add(customer);
        customer.OnStateChange += CustomersInQueue_OnStateChange;
    }

    private void CustomersInQueue_OnStateChange(object sender, CustomerState state)
    {
        if (state is CustomerState.Leaving)
        {
            Customer c = (Customer) sender;
            c.OnStateChange -= CustomersInQueue_OnStateChange;
            _customerQueue.Remove(c);
            OnAnyCustomerLeave?.Invoke(this, EventArgs.Empty);
        }

        if (state is CustomerState.WaitingForOrderCompletion)
        {
            OnOrderReceived?.Invoke(this, ((Customer) sender).Order);
        }
    }

    public Vector3 GetCustomerPosition(Customer customer)
    {
        return _customerQueue.GetPosition(customer);
    }
    
    public int GetCustomerPositionIndex(Customer customer)
    {
        return _customerQueue.GetPositionIndex(customer);
    }

    public int CustomerCount()
    {
        return _customerQueue.Count;
    }

    private void Pay()
    {
        float totalPrice = 0f;
        _products.ToList().ForEach(i =>
        {
            totalPrice += i.ProductSo.sellPrice;
            i.DestroySelf();
        });
        EconomyManager.Instance.AddMoney(totalPrice);
    }

    public void CheckOrderCompletion(Customer customer)
    {
        var checkoutItems = _products.GroupBy(i => i.ProductSo)
            .ToDictionary(i => i.Key, i => i.Count());
        var orderItems = customer.Order.Items;
        if (checkoutItems.Count == orderItems.Count && !checkoutItems.Except(orderItems).Any())
        {
            Pay();
            customer.LeaveHappy();
        }
        else
        {
            customer.ReceiveFailedOrder();
        }
    }
}
