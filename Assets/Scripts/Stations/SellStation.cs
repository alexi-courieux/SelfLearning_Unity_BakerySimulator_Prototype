using System;
using System.Linq;
using UnityEngine;

public class SellStation : MonoBehaviour, IInteractable, IInteractableAlt, IHandleItems
{
    [SerializeField] private Transform itemSlot;
    private readonly StackList<HandleableItem> _items = new();
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
        float totalPrice = 0f;
        _items.ToList().ForEach(i =>
        {
            totalPrice += i.HandleableItemSo.sellPrice;
            i.DestroySelf();
        });
        EconomyManager.Instance.AddMoney(totalPrice);
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
}
