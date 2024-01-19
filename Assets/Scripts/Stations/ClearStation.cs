using UnityEngine;

public class ClearStation : MonoBehaviour, IInteractable, IHandleItems
{
    [SerializeField] private Transform itemSlot;
    private HandleableItem _item;
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

  
   public void AddItem(HandleableItem handleableItem)
    {
        _item = handleableItem;
    }

    public HandleableItem[] GetItems()
    {
        return new[] {_item};
    }
    
    public void ClearItem(HandleableItem item)
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
