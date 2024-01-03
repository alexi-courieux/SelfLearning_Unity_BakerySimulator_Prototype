using UnityEngine;

public interface IHandleItems
{
    /// Must be called by the HandleableItem to set the reference in the parent
    public void AddItem(HandleableItem item);
    public HandleableItem[] GetItems();
    /// Must be called by the HandleableItem to clear the reference in the parent
    public void ClearItem(HandleableItem item);
    public bool HaveItems();
    public Transform GetAvailableItemSlot();
    public bool HasAvailableSlot();
}