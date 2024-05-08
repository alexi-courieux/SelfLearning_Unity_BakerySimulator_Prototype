using System;
using UnityEngine;

public class PlayerItemHandlingSystem: MonoBehaviour, IHandleItems
{
    private const int IgnoreRaycastLayer = 1 << 1;
    [SerializeField] private Transform itemSlot;
    private Item _item;
    private int _defaultHandleableItemsLayer;

    private void Start()
    {
        InputManager.Instance.OnDrop += InputManager_Drop;
    }

    private void InputManager_Drop(object sender, EventArgs e)
    {
        Drop();
    }

    private void Drop()
    {
        if (!HaveItems()) return;
        _item.SetParent(null);
    }

    public void AddItem(Item item)
    {
        _item = item;
        _defaultHandleableItemsLayer = _item.gameObject.layer;
        item.gameObject.layer = IgnoreRaycastLayer;
    }

    public Item[] GetItems()
    {
        return new[] {_item};
    }

    public Item GetItem()
    {
        return _item;
    }

    public void ClearItem(Item item)
    {
        _item.gameObject.layer = _defaultHandleableItemsLayer;
        _item = null;
        _defaultHandleableItemsLayer = 0;
    }

    public bool HaveItems()
    {
        return _item is not null;
    }
    
    public Transform GetAvailableItemSlot()
    {
        return itemSlot;
    }

    public bool HasAvailableSlot()
    {
        return _item is null;
    }
}