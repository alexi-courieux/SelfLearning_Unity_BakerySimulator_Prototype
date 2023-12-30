using UnityEngine;

public interface ICanHold
{
    /// Must be called by the holdable object to set the reference in the parent
    public void SetHeldItem(HoldableObject holdableObject);
    public HoldableObject GetHeldItem();
    /// Must be called by the holdable object to clear the reference in the parent
    public void ClearHeldItem();
    public bool IsHoldingItem();
    public Transform GetHoldPoint();
}