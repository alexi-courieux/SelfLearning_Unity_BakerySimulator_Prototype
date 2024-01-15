using System;
using System.Linq;
using UnityEngine;

public class DisplayStation : MonoBehaviour, IInteractable, IHandleItems, IDisplayItems
{
    public EventHandler OnPutIn;
    public EventHandler OnTakeOut;
    
    [SerializeField] private Transform[] itemSlots;
    private StackList<HandleableItem> _items;
    private int _capacity;

    private void Awake()
    {
        _capacity = itemSlots.Length;
        _items = new StackList<HandleableItem>();
    }

    public void Interact()
    {
        if (Player.Instance.HandleSystem.HaveItems() && Player.Instance.HandleSystem.GetItem().HandleableItemSo.CanBeSold())
        {
            if (HasAvailableSlot())
            {
                Player.Instance.HandleSystem.GetItem().SetParent(this);
                OnPutIn?.Invoke(this, EventArgs.Empty);
            }
        }
        else
        {
            if (_items.Count > 0) 
            {
                HandleableItem item = _items.Pop();
                item.SetParent(Player.Instance.HandleSystem);
                OnTakeOut?.Invoke(this, EventArgs.Empty);
            }
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
        return itemSlots[_items.Count];
    }

    public bool HasAvailableSlot()
    {
        return _items.Count < _capacity;
    }
    
    public HandleableItemSo[] GetItemsSo()
    {
        return _items.Select(item => item.HandleableItemSo).ToArray();
    }
}
