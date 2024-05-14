using System;
using System.Linq;
using UnityEngine;

public class ClearStation : MonoBehaviour, IInteractable, IHandleItems, IInteractableAlt, IFocusable
{
    [SerializeField] private RecipesDictionarySo recipesDictionarySo;
    [SerializeField] private Transform productSlot;
    [SerializeField] private Transform toolSlot;
    private Product _product;
    private Tool _tool;
    private ToolRecipeSo[] _recipes;
    
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
                return;
            }
            
            if (HaveItems<Tool>())
            {
                _tool.SetParent<Tool>(Player.Instance.HandleSystem);
            }
        }
    }

    public void InteractAlt()
    {
        if (!HaveItems<Product>()) return;

        if (!HaveItems<Tool>() && !Player.Instance.HandleSystem.HaveItems<Tool>()) return;

        Tool tool;
        if (Player.Instance.HandleSystem.HaveItems<Tool>())
        {
            tool = Player.Instance.HandleSystem.GetItem() as Tool;
        }
        else
        {
            tool = _tool;
        }
        
        
    }

    public void Focus()
    {
        Tool tool;
        if(Player.Instance.HandleSystem.HaveItems<Tool>())
        {
            // We try to use tool holded by player in priority
            tool = Player.Instance.HandleSystem.GetItem() as Tool;
        }
        else
        {
            // If player doesn't hold any tool, we use the tool holded by the station
            tool = _tool;
        }

        if (tool is null || _product is null) return;
        
        _recipes = CheckForRecipes(tool.ToolSo, _product.ProductSo);
        // TODO : Show Recipe UI
    }
    
    public void StopFocus()
    {
        // TODO : Hide Recipe UI
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
