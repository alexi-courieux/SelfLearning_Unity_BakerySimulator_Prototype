using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHoldSystem: MonoBehaviour, ICanHold
{
    private const int ignoreRaycastLayer = 1 << 1;
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
        if (_holdableObject == null) return;
        _holdableObject.SetParent(null);
    }

    public void SetHeldItem(HoldableObject holdableObject)
    {
        _holdableObject = holdableObject;
        _defaultHoldableObjectsLayer = _holdableObject.gameObject.layer;
        holdableObject.gameObject.layer = ignoreRaycastLayer;
    }

    public HoldableObject GetHeldItem()
    {
        return _holdableObject;
    }

    public void ClearHeldItem()
    {
        _holdableObject.gameObject.layer = _defaultHoldableObjectsLayer;
        _holdableObject = null;
        _defaultHoldableObjectsLayer = 0;
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