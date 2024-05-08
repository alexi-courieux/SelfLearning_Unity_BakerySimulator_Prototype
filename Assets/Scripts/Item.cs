using System;
using UnityEngine;

public abstract class Item : MonoBehaviour, IInteractable
{
    private IHandleItemsBase _parent;
    private Rigidbody _rb;
    
    public static void SpawnItem<T>(Transform itemPrefab, IHandleItems<T> parent) where T : Item
    {
        Transform itemTransform = Instantiate(itemPrefab);
        T item = itemTransform.GetComponent<T>();
        item.SetParent(parent);
    }

    private void Awake()
    {
       _rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Try to change the parent of this item to another one, it must have an available slot or be null
    /// </summary>
    /// <param name="targetParent">new parent</param>
    /// <returns>true if the parent have changed</returns>
    public void SetParent<T>(IHandleItems<T> targetParent) where T : Item
    {
        if (targetParent?.HasAvailableSlot() is false)
            throw new ArgumentException("The parent must have an available slot or be null");

        _parent?.ClearItem(this);
        
        _parent = targetParent;

        if (_parent is not null)
        {
            transform.parent = _parent.GetAvailableItemSlot();
            _parent.AddItem(this);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
        else
        {
            transform.parent = null;
        }
        
        if (_rb is not null)
        {
            _rb.isKinematic = _parent is not null;
        }
    }

    public void DestroySelf()
    {
        SetParent<Item>(null);
        Destroy(gameObject);
    }

    public void Interact()
    {
        SetParent(Player.Instance.HandleSystem);
    }
}