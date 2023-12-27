using System;
using UnityEngine;

public class ContainerStation : MonoBehaviour, ICanBeInteracted
{
    [SerializeField] private HoldableObjectSo holdableObjectSo;
    [SerializeField] private SpriteRenderer containerSprite;
    private void Start()
    {
        containerSprite.sprite = holdableObjectSo.sprite;
    }

    public void Interact()
    {
        if (Player.Instance.HoldSystem.IsHoldingItem()) return;
        HoldableObject.SpawnHoldableObject(holdableObjectSo, Player.Instance.HoldSystem);
    }
}