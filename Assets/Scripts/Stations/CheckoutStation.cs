using System;
using System.Linq;
using UnityEngine;

public class CheckoutStation : MonoBehaviour, IInteractable, IHandleItems<Product>
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
        if (Player.Instance.HandleSystem.HaveItems())
        {
            Item item = Player.Instance.HandleSystem.GetItem();
            if (Player.Instance.HandleSystem.GetItem() is not Product product)
            {
                Logger.LogWarning("Station can only hold products!");
                return;
            }
            product.SetParent(this);
        }
        else
        {
            if (_products.Count > 0)
            {
                Item item = _products.Pop();
                item.SetParent(Player.Instance.HandleSystem);
            }
        }
    }

    public void AddItem(Product item)
    {
        _products.Push(item);
    }

    public Product[] GetItems()
    {
        return _products.ToArray();
    }

    public void ClearItem(Product item)
    {
        _products.Remove(item);
    }

    public bool HaveItems()
    {
        return _products.Count > 0;
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
        _products.Cast<Product>().ToList().ForEach(i =>
        {
            totalPrice += i.ProductSo.sellPrice;
            i.DestroySelf();
        });
        EconomyManager.Instance.AddMoney(totalPrice);
    }

    public void CheckOrderCompletion(Customer customer)
    {
        var checkoutItems = _products.Cast<Product>().GroupBy(i => i.ProductSo)
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
