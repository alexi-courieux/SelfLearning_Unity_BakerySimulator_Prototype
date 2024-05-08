using System;
using System.Linq;
using UnityEngine;

public class DisplayStation : MonoBehaviour, IInteractable, IHandleItems, IDisplayItems
{
    public EventHandler OnPutIn;
    public EventHandler OnTakeOut;
    
    [SerializeField] private Transform[] itemSlots;
    private StackList<Item> _items;
    private int _capacity;

    private void Awake()
    {
        _capacity = itemSlots.Length;
        _items = new StackList<Item>();
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
            if (item is not Product product || !product.ProductSo.CanBeSold()) return;
            
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
