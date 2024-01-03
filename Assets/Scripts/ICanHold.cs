using UnityEngine;

public interface ICanHold
{
    /// Must be called by the holdable object to set the reference in the parent
    public void SetHoldable(HoldableObject holdableObject);
    public HoldableObject GetHoldable();
    /// Must be called by the holdable object to clear the reference in the parent
    public void ClearHoldable();
    public bool HaveHoldable();
    public Transform GetHoldPoint();
}