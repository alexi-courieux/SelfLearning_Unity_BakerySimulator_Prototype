using UnityEngine;

public interface IHandleItemsBase
{
    bool HaveItems();
    Transform GetAvailableItemSlot();
    bool HasAvailableSlot();
    /// Must be called by the HandleableItem to set the reference in the parent
    public void AddItem(Item item);
    public Item[] GetItems();
    /// Must be called by the HandleableItem to clear the reference in the parent
    public void ClearItem(Item item);
}