using System;
using UnityEngine;

namespace AshLight.BakerySim
{
    public class PlayerItemHandlingSystem : MonoBehaviour, IHandleItems
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
            if (!HaveItems<Item>()) return;
            _item.Drop();
        }

        public void AddItem<T>(Item item) where T : Item
        {
            _item = item;
            _defaultHandleableItemsLayer = _item.gameObject.layer;
            item.gameObject.layer = IgnoreRaycastLayer;
        }

        public Item[] GetItems<T>() where T : Item
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

        public bool HaveItems<T>() where T : Item
        {
            return _item is T;
        }

        public bool HaveAnyItems()
        {
            return _item is not null;
        }

        public Transform GetAvailableItemSlot<T>() where T : Item
        {
            return itemSlot;
        }

        public bool HasAvailableSlot<T>() where T : Item
        {
            return _item is null;
        }
    }
}