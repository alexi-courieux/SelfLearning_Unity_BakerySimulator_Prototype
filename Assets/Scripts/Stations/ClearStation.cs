using UnityEngine;

public class ClearStation : MonoBehaviour, ICanBeInteracted, ICanHold
{
    [SerializeField] private Transform holdPoint;
    private HoldableObject _holdItem;
   public void Interact()
    {
        if (IsHoldingItem())
        {
            if (Player.Instance.HoldSystem.IsHoldingItem())
            {
                Debug.Log("Player can't hold more than one item at a time!");
            }
            else
            {
                _holdItem.SetParent(Player.Instance.HoldSystem);
            }
        }
        else
        {
            if (Player.Instance.HoldSystem.IsHoldingItem())
            {
                Player.Instance.HoldSystem.GetHeldItem().SetParent(this);
            }
        }
    }

  
   public void SetHeldItem(HoldableObject holdableObject)
    {
        _holdItem = holdableObject;
    }

    public HoldableObject GetHeldItem()
    {
        return _holdItem;
    }
    
    public void ClearHeldItem()
    {
        _holdItem = null;
    }

    public bool IsHoldingItem()
    {
        return _holdItem != null;
    }
    
    public Transform GetHoldPoint()
    {
        return holdPoint;
    }
}
