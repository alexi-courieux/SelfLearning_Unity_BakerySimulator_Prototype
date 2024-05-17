using System;
using System.Linq;
using UnityEngine;
using Logger = Utils.Logger;

namespace AshLight.BakerySim.Stations
{
    public class OvenStation : MonoBehaviour, IInteractable, IInteractableAlt, IHandleItems
    {
        public EventHandler OnPutIn;
        public EventHandler OnTakeOut;
        public EventHandler<State> OnStateChanged;

        public enum State
        {
            Idle,
            Processing,
            Burning
        }
    
        [SerializeField] private Transform itemSlot;
        [SerializeField] private RecipesDictionarySo recipesDictionarySo;
        [SerializeField] private ProductSo trashObject;
        private Product _product;
        private State _state;
        private State CurrentState
        {
            get => _state;
            set
            {
                _state = value;
                OnStateChanged?.Invoke(this, _state);
            }
        }
        private OvenRecipeSo _ovenRecipeSo;
        private float _timeToProcess;
    
        private void Update()
        {
            switch (CurrentState)
            {
                case State.Idle:
                    break;
                case State.Processing:
                    _timeToProcess -= Time.deltaTime;
                    if (_timeToProcess <= 0f)
                    {
                        foreach (Item item in GetItems<Product>())
                        {
                            item.DestroySelf();
                        }
                        if (_ovenRecipeSo.burnt)
                        {
                            CurrentState = State.Burning;
                            Item.SpawnItem<Product>(trashObject.prefab, this);
                        }
                        else
                        {
                            Item.SpawnItem<Product>(_ovenRecipeSo.output.prefab, this);
                            CheckForRecipe();
                        }
                    }
                    break;
                case State.Burning:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Interact()
        {
            if (CurrentState is not State.Idle) return;
            bool isPlayerHoldingProduct = Player.Instance.HandleSystem.HaveItems<Product>();
            if (HaveItems<Product>())
            {
                if (isPlayerHoldingProduct) return;
                _product.SetParent<Item>(Player.Instance.HandleSystem);
                OnTakeOut?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                if (!isPlayerHoldingProduct) return;
                Item item = Player.Instance.HandleSystem.GetItem();
                if (item is not Product product)
                {
                    Logger.LogWarning("Station can only hold products!");
                    return;
                }
                product.SetParent<Product>(this);
                OnPutIn?.Invoke(this, EventArgs.Empty);
            }
        }

        public void InteractAlt()
        {
            if (_product is not null)
            {
                if (CurrentState is State.Idle)
                {
                    CheckForRecipe();
                }
                else
                {
                    CurrentState = State.Idle;
                }
            }
        
        }

        private void CheckForRecipe()
        {
            OvenRecipeSo recipe = recipesDictionarySo.ovenRecipes.FirstOrDefault(r => r.input == _product.ProductSo);
            if (recipe is not null)
            {
                CurrentState = State.Processing;
                _timeToProcess = recipe.timeToProcess;
                _ovenRecipeSo = recipe;
            }
            else
            {
                CurrentState = State.Idle;
            }
        }
    
        public void AddItem<T>(Item item) where T : Item
        {
            if(typeof(T) != typeof(Product))
            {
                throw new Exception("This station can only hold products!");
            }
        
            _product = item as Product;
        }

        public Item[] GetItems<T>() where T : Item
        {
            if (typeof(T) == typeof(Product))
            {
                return new Item[]{_product};
            }
            Logger.LogWarning($"This station doesn't have items of the specified type : {typeof(T)}");
            return new Item[]{};
        }
    
        public void ClearItem(Item item)
        {
            if (_product == item as Product)
            {
                _product = null;
            }
        }

        public bool HaveItems<T>() where T : Item
        {
            if (typeof(T) == typeof(Product))
            {
                return _product is not null;
            }
            Logger.LogWarning($"This station doesn't have items of the specified type : {typeof(T)}");
            return false;
        }
    
        public bool HaveAnyItems()
        {
            return _product is not null;
        }

        public Transform GetAvailableItemSlot<T>() where T : Item
        {
            if (typeof(T) == typeof(Product))
            {
                return itemSlot;
            }
            Logger.LogWarning($"This station doesn't have items of the specified type : {typeof(T)}");
            return null;
        }

        public bool HasAvailableSlot<T>() where T : Item
        {
            if(typeof(T) == typeof(Product))
            {
                return _product is null;
            }
            Logger.LogWarning($"This station doesn't have items of the specified type : {typeof(T)}");
            return false;
        }
    }
}
