using UnityEngine;

/// <summary>
/// Interface for objects that can hold physical items (HandleableItem)
/// </summary>
public interface IHandleItems
{
    /// Must be called by the HandleableItem to set the reference in the parent
    public void AddItem(Item item);
    public Item[] GetItems();
    /// Must be called by the HandleableItem to clear the reference in the parent
    public void ClearItem(Item item);
    public bool HaveItems();
    public Transform GetAvailableItemSlot();
    public bool HasAvailableSlot();
}