using System;
using UnityEngine;

public class PlayerItemHandlingSystem: MonoBehaviour, IHandleItems
{
    private const int IgnoreRaycastLayer = 1 << 1;
    [SerializeField] private Transform itemSlot;
    private HandleableItem _item;
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

    public void AddItem(HandleableItem handleableItem)
    {
        _item = handleableItem;
        _defaultHandleableItemsLayer = _item.gameObject.layer;
        handleableItem.gameObject.layer = IgnoreRaycastLayer;
    }

    public HandleableItem[] GetItems()
    {
        return new[] {_item};
    }

    public HandleableItem GetItem()
    {
        return _item;
    }

    public void ClearItem(HandleableItem item)
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