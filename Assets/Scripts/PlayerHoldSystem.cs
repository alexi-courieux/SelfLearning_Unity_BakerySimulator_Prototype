using System;
using UnityEngine;

public class PlayerHoldSystem: MonoBehaviour
{
    [SerializeField] private Transform holdPoint;
    private Transform _heldObject;

    private void Start()
    {
        InputManager.Instance.OnDrop += InputManager_Drop;
    }

    private void InputManager_Drop(object sender, EventArgs e)
    {
        Drop();
    }

    public void Take(Transform target)
    {
        Debug.Log("Take");
        if (_heldObject != null) throw new Exception("Already holding something");
        _heldObject = target;
        _heldObject.SetParent(holdPoint);
        _heldObject.localPosition = Vector3.zero;
        _heldObject.localRotation = Quaternion.identity;
        
        _heldObject.TryGetComponent(out Rigidbody rb);
        if (rb != null)
        {
            rb.isKinematic = true;
        }
    }

    private void Drop()
    {
        if (_heldObject == null) return;
        _heldObject.SetParent(null);
        
        _heldObject.TryGetComponent(out Rigidbody rb);
        if (rb != null)
        {
            rb.isKinematic = false;
        }
        
        _heldObject = null;
    }

    public void Depose()
    {
        throw new System.NotImplementedException();
    }
    
    public bool IsHoldingSomething()
    {
        return _heldObject != null;
    }
}