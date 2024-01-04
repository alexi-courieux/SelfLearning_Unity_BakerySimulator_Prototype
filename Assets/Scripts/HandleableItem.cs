using System;
using UnityEngine;

public class HandleableItem : MonoBehaviour, IInteractable
{
    public HandleableItemSo HandleableItemSo => handleableItemSo;

    [SerializeField] private HandleableItemSo handleableItemSo;
    private IHandleItems _parent;

    public static void SpawnItem(HandleableItemSo handleableItemSo, IHandleItems parent)
    {
        Transform itemTransform = Instantiate(handleableItemSo.prefab);
        HandleableItem item = itemTransform.GetComponent<HandleableItem>();
        item.SetParent(parent);
    }

    /// <summary>
    /// Try to change the parent of this item to another one, it must have an available slot or be null
    /// </summary>
    /// <param name="targetParent">new parent</param>
    /// <returns>true if the parent have changed</returns>
    public void SetParent(IHandleItems targetParent)
    {
        if (targetParent?.HasAvailableSlot() is false)
            throw new ArgumentException("The parent must have an available slot or be null");

        _parent?.ClearItem(this);
        
        _parent = targetParent;

        if (_parent is not null)
        {
            _parent.AddItem(this);
            transform.parent = _parent.GetAvailableItemSlot();
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
        else
        {
            transform.parent = null;
        }

        TryGetComponent(out Rigidbody rb);
        if (rb is not null)
        {
            rb.isKinematic = _parent is not null;
        }
    }

    public void DestroySelf()
    {
        SetParent(null);
        Destroy(gameObject);
    }

    public void Interact()
    {
        SetParent(Player.Instance.HandleSystem);
    }
}