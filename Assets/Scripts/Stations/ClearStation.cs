using System;
using System.Linq;
using UnityEngine;

public class ClearStation : MonoBehaviour, IInteractable, IHandleItems, IInteractableAlt, IFocusable, IInteractableNext, IInteractablePrevious
{
    public EventHandler OnFocus;
    public EventHandler OnStopFocus;
    public EventHandler<RecipeSelectedEventArgs> OnRecipeSelected;
    [SerializeField] private RecipesDictionarySo recipesDictionarySo;
    [SerializeField] private Transform productSlot;
    [SerializeField] private Transform toolSlot;
    private Product _product;
    private Tool _tool;
    private ToolRecipeSo[] _recipes;
    private ToolRecipeSo _selectedRecipe;
    
   public void Interact()
    {
        if (Player.Instance.HandleSystem.HaveAnyItems())
        {
            if (Player.Instance.HandleSystem.HaveItems<Product>())
            {
                if (HaveItems<Product>()) return;
                Player.Instance.HandleSystem.GetItem().SetParent<Product>(this);
            }
            else if (Player.Instance.HandleSystem.HaveItems<Tool>())
            {
                if (HaveItems<Tool>()) return;
                Player.Instance.HandleSystem.GetItem().SetParent<Tool>(this);
            }
            else
            {
                Logger.LogWarning("Station can only hold products or tools!");
            }
        }
        else if (HaveAnyItems())
        {
            if (HaveItems<Product>())
            {
                _product.SetParent<Product>(Player.Instance.HandleSystem);
            } 
            else if (HaveItems<Tool>())
            {
                _tool.SetParent<Tool>(Player.Instance.HandleSystem);
            }
        }
        RefreshRecipes();
    }

    public void InteractAlt()
    {
        if (!HaveItems<Product>()) return;
        if (!HaveItems<Tool>() && !Player.Instance.HandleSystem.HaveItems<Tool>()) return;
        if (_selectedRecipe is null) return;
        if(_product.ProductSo != _selectedRecipe.input) return;
        Tool tool = GetToolForRecipe();
        if(tool?.ToolSo != _selectedRecipe.tool) return;
        
        _product.DestroySelf();
        Item.SpawnItem<Product>(_selectedRecipe.output.prefab, this);
    }

    public void Focus()
    {
        RefreshRecipes();
        OnFocus?.Invoke(this, EventArgs.Empty);
    }

    private void RefreshRecipes()
    {
        Tool tool = GetToolForRecipe();

        if (tool is null || _product is null)
        {
            _recipes = null;
            _selectedRecipe = null;
            OnRecipeSelected?.Invoke(this, new RecipeSelectedEventArgs(null, 0));
        }
        else
        {
            _recipes = CheckForRecipes(tool.ToolSo, _product.ProductSo);
            _selectedRecipe = _recipes.FirstOrDefault();
            OnRecipeSelected?.Invoke(this, new RecipeSelectedEventArgs(_selectedRecipe, _recipes.Length));
        }
    }
    
    private Tool GetToolForRecipe()
    {
        if(Player.Instance.HandleSystem.HaveItems<Tool>())
        {
            // We try to use tool holded by player in priority
            return Player.Instance.HandleSystem.GetItem() as Tool;
        }
        else
        {
            // If player doesn't hold any tool, we use the tool holded by the station
            return _tool;
        }
    }
    
    public void InteractNext()
    {
        if (_recipes is null || _recipes.Length == 0) return;
        int index = Array.IndexOf(_recipes, _selectedRecipe);
        index++;
        if (index >= _recipes.Length)
        {
            index = 0;
        }
        _selectedRecipe = _recipes[index];
        OnRecipeSelected?.Invoke(this, new RecipeSelectedEventArgs(_selectedRecipe, _recipes.Length));
    }
    
    public void InteractPrevious()
    {
        if (_recipes is null || _recipes.Length == 0) return;
        int index = Array.IndexOf(_recipes, _selectedRecipe);
        index--;
        if (index < 0)
        {
            index = _recipes.Length - 1;
        }
        _selectedRecipe = _recipes[index];
        OnRecipeSelected?.Invoke(this, new RecipeSelectedEventArgs(_selectedRecipe, _recipes.Length));
    }
    
    public void StopFocus()
    {
        OnStopFocus?.Invoke(this, EventArgs.Empty);
    }
    public ToolRecipeSo[] CheckForRecipes(ToolSo tool, ProductSo product)
    {
        return recipesDictionarySo.toolRecipes.Where(r => r.tool == tool && r.input == product).ToArray();
    }


    public void AddItem<T>(Item item) where T : Item
    {
        if (typeof(T) != typeof(Product) && typeof(T) != typeof(Tool))
        {
            throw new Exception("This station can only hold products or tools!");
        }
        
        if (typeof(T) == typeof(Product))
        {
            _product = item as Product;
        }
        
        if (typeof(T) == typeof(Tool))
        {
            _tool = item as Tool;
        }
    }

    public Item[] GetItems<T>() where T : Item
    {
        if (typeof(T) == typeof(Product))
        {
            return new Item[] { _product };
        }

        if (typeof(T) == typeof(Tool))
        {
            return new Item[] { _tool };
        }

        Logger.LogWarning($"This station doesn't have items of the specified type : {typeof(T)}");
        return new Item[] { };
    }
    
    public void ClearItem(Item item)
    {
        if (item is Product)
        {
            _product = null;
        }
        if (item is Tool)
        {
            _tool = null;
        }
    }

    public bool HaveItems<T>() where T : Item
    {
        if (typeof(T) == typeof(Product))
        {
            return _product is not null;
        }
        
        if (typeof(T) == typeof(Tool))
        {
            return _tool is not null;
        }
        
        Logger.LogWarning($"This station doesn't have items of the specified type : {typeof(T)}");
        return false;
    }

    public bool HaveAnyItems()
    {
        return _product is not null || _tool is not null;
    }

    public Transform GetAvailableItemSlot<T>() where T : Item
    {
        if (typeof(T) == typeof(Product))
        {
            return productSlot;
        }

        if (typeof(T) == typeof(Tool))
        {
            return toolSlot;
        }

        Logger.LogWarning($"This station doesn't have slots for the specified item : {typeof(T)}");
        return null;
    }

    public bool HasAvailableSlot<T>() where T : Item
    {
        if (typeof(T) == typeof(Product))
        {
            return _product is null;
        }

        if (typeof(T) == typeof(Tool))
        {
            return _tool is null;
        }

        Logger.LogWarning($"This station doesn't have slots for the specified item : {typeof(T)}");
        return false;
    }
}

public class RecipeSelectedEventArgs : EventArgs
{
    public ToolRecipeSo Recipe { get; }
    public int AvailableRecipesCount { get; }

    public RecipeSelectedEventArgs(ToolRecipeSo recipe, int availableRecipesCount)
    {
        Recipe = recipe;
        AvailableRecipesCount = availableRecipesCount;
    }
}
