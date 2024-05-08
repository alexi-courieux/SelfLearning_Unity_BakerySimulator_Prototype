using UnityEngine;

public class ClearStation : MonoBehaviour, IInteractable, IHandleItems<Item>
{
    [SerializeField] private Transform itemSlot;
    private Item _item;
   public void Interact()
    {
        if (HaveItems())
        {
            if (Player.Instance.HandleSystem.HaveItems())
            {
                Logger.LogWarning("Player can't hold more than one item at a time!");
            }
            else
            {
                _item.SetParent(Player.Instance.HandleSystem);
            }
        }
        else
        {
            if (Player.Instance.HandleSystem.HaveItems())
            {
                Player.Instance.HandleSystem.GetItem().SetParent(this);
            }
        }
    }

  
   public void AddItem(Item item)
    {
        _item = item;
    }

    public Item[] GetItems()
    {
        return new[] {_item};
    }
    
    public void ClearItem(Item item)
    {
        _item = null;
    }

    public bool HaveItems()
    {
        return _item != null;
    }
    
    public Transform GetAvailableItemSlot()
    {
        return itemSlot;
    }

    public bool HasAvailableSlot()
    {
        return _item is null;
    }
}
