using System;
using System.Linq;
using UnityEngine;

public class DisplayStation : MonoBehaviour, IInteractable, IHandleItems<Product>, IDisplayItems
{
    public EventHandler OnPutIn;
    public EventHandler OnTakeOut;
    
    [SerializeField] private Transform[] itemSlots;
    private StackList<Product> _items;
    private int _capacity;

    private void Awake()
    {
        _capacity = itemSlots.Length;
        _items = new StackList<Product>();
    }

    private void Start()
    {
        OrderManager.Instance.DisplayStations.Add(this);
    }

    public void Interact()
    {
        if (Player.Instance.HandleSystem.HaveItems())
        {
            if (!HasAvailableSlot()) return;
            Item item = Player.Instance.HandleSystem.GetItem();
            if (Player.Instance.HandleSystem.GetItem() is not Product product)
            {
                Logger.LogWarning("Station can only hold products!");
                return;
            }
            if (!product.ProductSo.CanBeSold())
            {
                Logger.LogWarning("Product can't be sold!");
                return;
            }
            
            Player.Instance.HandleSystem.GetItem().SetParent(this);
            OnPutIn?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            if (_items.Count > 0) 
            {
                Item item = _items.Pop();
                item.SetParent(Player.Instance.HandleSystem);
                OnTakeOut?.Invoke(this, EventArgs.Empty);
            }
        }
    }

  
   public void AddItem(Product item)
    {
        _items.Push(item);
    }

    public Product[] GetItems()
    {
        return _items.ToArray();
    }
    
    public void ClearItem(Product item)
    {
        _items.Remove(item);
    }

    public bool HaveItems()
    {
        return _items.Count > 0;
    }
    
    public Transform GetAvailableItemSlot()
    {
        return itemSlots[_items.Count];
    }

    public bool HasAvailableSlot()
    {
        return _items.Count < _capacity;
    }
    
    public ProductSo[] GetItemsSo()
    {
        return _items.Cast<Product>().Select(item => item.ProductSo).ToArray();
    }
}
