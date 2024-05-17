using System;
using System.Linq;
using UnityEngine;
using Utils;
using Logger = Utils.Logger;

namespace AshLight.BakerySim.Stations
{
    public class DisplayStation : MonoBehaviour, IInteractable, IHandleItems, IDisplayItems
    {
        public EventHandler OnPutIn;
        public EventHandler OnTakeOut;
    
        [SerializeField] private Transform[] itemSlots;
        private StackList<Product> _items;
        private int _capacity;

        private void Awake()
        {
            _capacity = itemSlots.Length;
            _items = new StackList<Product>();
        }

        private void Start()
        {
            OrderManager.Instance.DisplayStations.Add(this);
        }

        public void Interact()
        {
            if (Player.Instance.HandleSystem.HaveAnyItems())
            {
                if (!HasAvailableSlot<Product>()) return;
            
                if (Player.Instance.HandleSystem.GetItem() is not Product product)
                {
                    Logger.LogWarning("Station can only hold products!");
                    return;
                }
            
                if (!product.ProductSo.CanBeSold())
                {
                    Logger.LogWarning("Product can't be sold!");
                    return;
                }
            
                product.SetParent<Product>(this);
                OnPutIn?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                if (_items.Count <= 0) return;
            
                Item item = _items.Pop();
                item.SetParent<Item>(Player.Instance.HandleSystem);
                OnTakeOut?.Invoke(this, EventArgs.Empty);
            }
        }

  
        public void AddItem<T>(Item item) where T : Item
        {
            if (typeof(T) != typeof(Product))
            {
                Logger.LogWarning("This station can only hold products!");
                return;
            }
        
            _items.Push(item as Product);
        }

        public Item[] GetItems<T>() where T : Item
        {
            return _items.Cast<Item>().ToArray();
        }
    
        public void ClearItem(Item item)
        {
            _items.Remove(item as Product);
        }

        public bool HaveItems<T>() where T : Item
        {
            return _items.Count > 0;
        }
    
        public bool HaveAnyItems()
        {
            return _items.Count > 0;
        }
    
        public Transform GetAvailableItemSlot<T>() where T : Item
        {
            return itemSlots[_items.Count];
        }

        public bool HasAvailableSlot<T>() where T : Item
        {
            return _items.Count < _capacity;
        }
    
        public ProductSo[] GetItemsSo()
        {
            return _items.Select(item => item.ProductSo).ToArray();
        }
    }
}
