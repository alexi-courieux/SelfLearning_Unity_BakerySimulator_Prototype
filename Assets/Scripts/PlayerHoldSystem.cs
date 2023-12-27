using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHoldSystem: MonoBehaviour, ICanHold
{
    [SerializeField] private Transform holdPoint;
    private HoldableObject _holdableObject;

    private void Start()
    {
        InputManager.Instance.OnDrop += InputManager_Drop;
    }

    private void InputManager_Drop(object sender, EventArgs e)
    {
        Drop();
    }

    private void Drop()
    {
        if (_holdableObject == null) return;
        _holdableObject.SetParent(null);
    }

    public void SetHeldItem(HoldableObject holdableObject)
    {
        _holdableObject = holdableObject;
    }

    public HoldableObject GetHeldItem()
    {
        return _holdableObject;
    }

    public void ClearHeldItem()
    {
        _holdableObject = null;
    }

    public bool IsHoldingItem()
    {
        return _holdableObject != null;
    }
    
    public Transform GetHoldPoint()
    {
        return holdPoint;
    }
}