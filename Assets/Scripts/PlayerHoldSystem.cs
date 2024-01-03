using System;
using UnityEngine;

public class PlayerHoldSystem: MonoBehaviour, ICanHold
{
    private const int IgnoreRaycastLayer = 1 << 1;
    [SerializeField] private Transform holdPoint;
    private HoldableObject _holdableObject;
    private int _defaultHoldableObjectsLayer;

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
        if (!HaveHoldable()) return;
        _holdableObject.SetParent(null);
    }

    public void SetHoldable(HoldableObject holdableObject)
    {
        _holdableObject = holdableObject;
        _defaultHoldableObjectsLayer = _holdableObject.gameObject.layer;
        holdableObject.gameObject.layer = IgnoreRaycastLayer;
    }

    public HoldableObject GetHoldable()
    {
        return _holdableObject;
    }

    public void ClearHoldable()
    {
        _holdableObject.gameObject.layer = _defaultHoldableObjectsLayer;
        _holdableObject = null;
        _defaultHoldableObjectsLayer = 0;
    }

    public bool HaveHoldable()
    {
        return _holdableObject != null;
    }
    
    public Transform GetHoldPoint()
    {
        return holdPoint;
    }
}