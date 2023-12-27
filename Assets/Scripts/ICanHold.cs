using UnityEngine;

public interface ICanHold
{
    public void SetHeldItem(HoldableObject holdableObject);
    public HoldableObject GetHeldItem();
    public void ClearHeldItem();
    public bool IsHoldingItem();
    public Transform GetHoldPoint();
}