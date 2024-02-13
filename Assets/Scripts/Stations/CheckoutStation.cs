using System;
using System.Linq;
using UnityEngine;

public class CheckoutStation : MonoBehaviour, IInteractable, IInteractableAlt, IHandleItems
{
    public EventHandler OnAnyCustomerLeave;
    public EventHandler<Order> OnOrderReceived;

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
        if(!HaveCustomers()) return;
        Customer customer = _customerQueue.PeekFirst();
        switch (customer.CurrentState)
        {
            case CustomerState.WaitingToOrder:
                customer.CreateOrder();
                break;
            case CustomerState.WaitingForOrderCompletion:
                if (customer.Order.Type is OrderType.Direct)
                {
                    CheckOrderCompletion(customer);
                }
                else
                {
                    AcceptOrder(customer);
                }
                break;
            case CustomerState.CollectingRequestOrder:
                CheckOrderCompletion(customer);
                break;
            case CustomerState.WaitingInQueue:
            case CustomerState.Leaving:
                throw new Exception("Customer shouldn't be in this state while interacting with checkout station");
            default:
                throw new ArgumentOutOfRangeException();
        }
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
        _items.ToList().ForEach(i =>
        {
            totalPrice += i.HandleableItemSo.sellPrice;
            i.DestroySelf();
        });
        EconomyManager.Instance.AddMoney(totalPrice);
    }
    
    private void CheckOrderCompletion(Customer customer)
    {
        var checkoutItems = _items.GroupBy(i => i.HandleableItemSo)
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

    private void AcceptOrder(Customer customer)
    {
        OrderManager.Instance.AcceptRequest(customer.Order);
        customer.LeaveHappy();
    }
}
