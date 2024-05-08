using System;
using System.Linq;
using UnityEngine;

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
                    foreach (Item item in GetItems())
                    {
                        item.DestroySelf();
                    }
                    if (_ovenRecipeSo.burnt)
                    {
                        CurrentState = State.Burning;
                        Item.SpawnItem(trashObject.prefab, this);
                    }
                    else
                    {
                        Item.SpawnItem(_ovenRecipeSo.output.prefab, this);
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
        if (_product is not null)
        {
            if (!Player.Instance.HandleSystem.HaveItems())
            {
                _product.SetParent(Player.Instance.HandleSystem);
                OnTakeOut?.Invoke(this, EventArgs.Empty);
            }
        }
        else
        {
            if (Player.Instance.HandleSystem.HaveItems())
            {
                Player.Instance.HandleSystem.GetItem().SetParent(this);
                OnPutIn?.Invoke(this, EventArgs.Empty);
            }
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

    public void AddItem(Item item)
    {
        _product = item as Product;
    }

    public Item[] GetItems()
    {
        return new []{_product};
    }
    
    public void ClearItem(Item item)
    {
        _product = null;
    }

    public bool HaveItems()
    {
        return _product != null;
    }

    public Transform GetAvailableItemSlot()
    {
        return itemSlot;
    }

    public bool HasAvailableSlot()
    {
        return _product is null;
    }
}
