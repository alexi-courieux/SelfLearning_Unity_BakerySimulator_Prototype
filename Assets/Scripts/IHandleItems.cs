using System;
using System.Linq;

/// <summary>
/// Interface for objects that can hold physical items (HandleableItem)
/// </summary>
public interface IHandleItems<T> : IHandleItemsBase where T : Item
{
    public void AddItem(T item);
    public new T[] GetItems();
    public void ClearItem(T item);

    void IHandleItemsBase.AddItem(Item item)
    {
        if (item is not T)
        {
            throw new Exception($"Item is not of type {typeof(T)}");
        }
        AddItem(item as T);
    }
    
    Item[] IHandleItemsBase.GetItems()
    {
        var items = GetItems().Cast<Item>().ToArray();
        return items;
    }
    
    void IHandleItemsBase.ClearItem(Item item)
    {
        if (item is not T)
        {
            throw new Exception($"Item is not of type {typeof(T)}");
        }
        ClearItem(item as T);
    }
}