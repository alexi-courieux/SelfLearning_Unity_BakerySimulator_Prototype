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
    private readonly StackList<Item> _items = new();
    

    private void Start()
    {
        CustomerManager.Instance.CheckoutStations.Add(this);
        _customerQueue = new WaitingQueue<Customer>(customerLimit, customerOffset, customerCheckoutPosition);
    }

    public void Interact()
    {
        if (Player.Instance.HandleSystem.HaveItems())
        {
            Item item = Player.Instance.HandleSystem.GetItem();
            item.SetParent(this);
        }
        else
        {
            if (_items.Count > 0)
            {
                Item item = _items.Pop();
                item.SetParent(Player.Instance.HandleSystem);
            }
        }
    }

    public void AddItem(Item item)
    {
        _items.Push(item);
    }

    public Item[] GetItems()
    {
        return _items.ToArray();
    }

    public void ClearItem(Item item)
    {
        _items.Remove(item);
    }

    public bool HaveItems()
    {
        return _items.Count > 0;
    }

    public Transform GetAvailableItemSlot()
    {
        return itemSlot;
    }

    public bool HasAvailableSlot()
    {
        
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
    
    private bool HaveCustomers()
    {
        return _customerQueue.Count > 0;
    }

    public int CustomerCount()
    {
        return _customerQueue.Count;
    }

    private void Pay()
    {
        float totalPrice = 0f;
        _items.Cast<Product>().ToList().ForEach(i =>
        {
            totalPrice += i.ProductSo.sellPrice;
            i.DestroySelf();
        });
        EconomyManager.Instance.AddMoney(totalPrice);
    }

    public void CheckOrderCompletion(Customer customer)
    {
        var checkoutItems = _items.Cast<Product>().GroupBy(i => i.ProductSo)
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
