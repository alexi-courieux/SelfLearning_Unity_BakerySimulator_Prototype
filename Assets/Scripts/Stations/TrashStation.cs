using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashStation : MonoBehaviour, ICanBeInteracted, ICanBeInteractedAlt
{
    public EventHandler<int> OnTrashAmountChanged;
    
    [SerializeField] private int trashAmountMax;
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
        if (!Player.Instance.HoldSystem.IsHoldingItem()) return;
        Player.Instance.HoldSystem.GetHeldItem().DestroySelf();
        TrashAmount++;
    }

    public void InteractAlt()
    {
        if (_trashAmount == 0) return;
        if (Player.Instance.HoldSystem.IsHoldingItem()) return;
        TrashAmount = 0;
    }
    
    public int GetTrashAmountMax()
    {
        return trashAmountMax;
    }
}
