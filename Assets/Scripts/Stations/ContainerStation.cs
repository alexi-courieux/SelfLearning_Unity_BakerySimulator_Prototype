using UnityEngine;

public class ContainerStation : MonoBehaviour, IInteractable
{
    [SerializeField] private HandleableItemSo handleableItemSo;
    [SerializeField] private SpriteRenderer containerSprite;
    private void Start()
    {
        containerSprite.sprite = handleableItemSo.sprite;
    }

    public void Interact()
    {
        if (Player.Instance.HandleSystem.HaveItems()) return;
        HandleableItem.SpawnItem(handleableItemSo, Player.Instance.HandleSystem);
        EconomyManager.Instance.AddMoney(-handleableItemSo.buyPrice);
    }
}