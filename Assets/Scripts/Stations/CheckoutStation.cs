using System;
using System.Linq;
using UnityEngine;

public class CheckoutStation : MonoBehaviour, IInteractable, IInteractableAlt, IHandleItems
{
    public EventHandler OnCustomerLeave;
    
    [SerializeField] private Transform itemSlot;
    [SerializeField] private int customerLimit;
    [SerializeField] private Vector3 customerOffset;
    [SerializeField] private Transform customerCheckoutPosition;

    private WaitingQueue<Customer> _customerQueue;
    private readonly StackList<HandleableItem> _items = new();
    

    private void Start()
    {
        CustomerManager.Instance.CheckoutStations.Add(this);
        _customerQueue = new WaitingQueue<Customer>(customerLimit, customerOffset, customerCheckoutPosition);
    }

    public void Interact()
    {
        if (Player.Instance.HandleSystem.HaveItems())
        {
            HandleableItem item = Player.Instance.HandleSystem.GetItem();
            item.SetParent(this);
        }
        else
        {
            if (_items.Count > 0)
            {
                HandleableItem item = _items.Pop();
                item.SetParent(Player.Instance.HandleSystem);
            }
        }
    }

    public void InteractAlt()
    {
        if(!HaveItems()) return;
        if(!HaveCustomers()) return;
        float totalPrice = 0f;
        _items.ToList().ForEach(i =>
        {
            totalPrice += i.HandleableItemSo.sellPrice;
            i.DestroySelf();
        });
        EconomyManager.Instance.AddMoney(totalPrice);
        Customer customer = _customerQueue.Shift();
        customer.Checkout();
    }

    public void AddItem(HandleableItem item)
    {
        _items.Push(item);
    }

    public HandleableItem[] GetItems()
    {
        return _items.ToArray();
    }

    public void ClearItem(HandleableItem item)
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
        customer.OnStateChange += Customer_OnStateChange;
    }

    private void Customer_OnStateChange(object sender, CustomerState state)
    {
        if (state is not CustomerState.Leaving) return;
        Customer c = (Customer) sender;
        c.OnStateChange -= Customer_OnStateChange;
        _customerQueue.Remove(c);
        OnCustomerLeave?.Invoke(this, EventArgs.Empty);
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
}
