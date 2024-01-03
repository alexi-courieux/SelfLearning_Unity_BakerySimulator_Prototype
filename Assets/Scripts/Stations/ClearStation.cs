using UnityEngine;

public class ClearStation : MonoBehaviour, ICanBeInteracted, ICanHold
{
    [SerializeField] private Transform holdPoint;
    private HoldableObject _holdItem;
   public void Interact()
    {
        if (HaveHoldable())
        {
            if (Player.Instance.HoldSystem.HaveHoldable())
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
            if (Player.Instance.HoldSystem.HaveHoldable())
            {
                Player.Instance.HoldSystem.GetHoldable().SetParent(this);
            }
        }
    }

  
   public void SetHoldable(HoldableObject holdableObject)
    {
        _holdItem = holdableObject;
    }

    public HoldableObject GetHoldable()
    {
        return _holdItem;
    }
    
    public void ClearHoldable()
    {
        _holdItem = null;
    }

    public bool HaveHoldable()
    {
        return _holdItem != null;
    }
    
    public Transform GetHoldPoint()
    {
        return holdPoint;
    }
}
