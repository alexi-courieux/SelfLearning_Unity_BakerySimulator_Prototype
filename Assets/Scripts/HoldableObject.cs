using UnityEngine;

public class HoldableObject : MonoBehaviour, ICanBeInteracted
{
    private ICanHold _parent;
    
    public void SetParent(ICanHold targetParent)
    {
        if (targetParent != null && targetParent.IsHoldingItem() && targetParent.GetHeldItem() != this)
        {
            Debug.Log("Target is already holding an item!");
            return;
        }

        _parent?.ClearHeldItem();
        
        _parent = targetParent;
        _parent?.SetHeldItem(this);

        transform.parent = _parent?.GetHoldPoint();
        if (_parent != null)
        {
            transform.localPosition = Vector3.zero;
        }
        
        TryGetComponent(out Rigidbody rb);
        if (rb != null)
        {
            rb.isKinematic = _parent != null;
        }
    }

    public void DestroySelf()
    {
        SetParent(null);
        Destroy(gameObject);
    }

    public void Interact()
    {
        SetParent(Player.Instance.HoldSystem);
    }
}