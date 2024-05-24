using UnityEngine;

namespace AshLight.BakerySim
{
    /// <summary>
    /// Interface for objects that can hold physical items (HandleableItem)
    /// </summary>
    public interface IHandleItems
    {
        /// <summary>
        /// Check if the parent has an available slot for an item
        /// </summary>
        /// <typeparam name="T">Type of the item</typeparam>
        /// <returns>true if the parent has an available slot for this item</returns>
        bool HasAvailableSlot<T>() where T : Item;

        /// <summary>
        /// Check if the parent has an item of the specified type
        /// </summary>
        /// <typeparam name="T">Type of the item</typeparam>
        /// <returns>true if the parent has an item of the specified type</returns>
        bool HaveItems<T>() where T : Item;

        /// <summary>
        /// Check if the parent has any items
        /// </summary>
        /// <returns>true if the parent has any items</returns>
        bool HaveAnyItems();

        /// <summary>
        /// Get a slot where the item can be placed, assuming that the parent has an available slot for the item
        /// </summary>
        /// <typeparam name="T">Type of the item</typeparam>
        /// <returns>Transform of the slot where the item can be placed</returns>
        Transform GetAvailableItemSlot<T>() where T : Item;

        /// <summary>
        /// Add an item to the parent
        /// </summary>
        /// <param name="item">Item to be added</param>
        /// <typeparam name="T">Type of the item</typeparam>
        public void AddItem<T>(Item item) where T : Item;

        /// <summary>
        /// Get all items of the specified type
        /// </summary>
        /// <typeparam name="T">Type of the item</typeparam>
        /// <returns>Array of items of the specified type</returns>
        public Item[] GetItems<T>() where T : Item;

        /// <summary>
        /// Remove a specific item from the parent
        /// </summary>
        /// <param name="item">Item to be removed</param>
        public void ClearItem(Item item);
    }
}