using System;
using UnityEngine;

public class TrashStation : MonoBehaviour, IInteractable, IInteractableAlt
{
    public EventHandler<int> OnTrashAmountChanged;
    
    [SerializeField] private int trashAmountMax;
    [SerializeField] private HandleableItemSo trashbagItem;
    private int _trashAmount;
    private int TrashAmount
    {
        get => _trashAmount;
        set
        {
            _trashAmount = value;
            OnTrashAmountChanged?.Invoke(this, value);
        }
    }

    private void Start()
    {
        _trashAmount = 0;
    }

    public void Interact()
    {
        if (_trashAmount >= trashAmountMax) return;
        if (!Player.Instance.HandleSystem.HaveItems()) return;
        HandleableItem item = Player.Instance.HandleSystem.GetItem();
        if(item.HandleableItemSo == trashbagItem) return;
        item.DestroySelf();
        TrashAmount++;
    }

    public void InteractAlt()
    {
        if (_trashAmount is 0) return;
        if (Player.Instance.HandleSystem.HaveItems()) return;
        TrashAmount = 0;
        HandleableItem.SpawnItem(trashbagItem, Player.Instance.HandleSystem);
    }
    
    public int GetTrashAmountMax()
    {
        return trashAmountMax;
    }
}
