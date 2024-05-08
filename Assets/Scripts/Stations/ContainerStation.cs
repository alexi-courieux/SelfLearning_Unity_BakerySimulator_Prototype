using UnityEngine;

public class ContainerStation : MonoBehaviour, IInteractable
{
    [SerializeField] private ProductSo productSo;
    [SerializeField] private SpriteRenderer containerSprite;
    private void Start()
    {
        containerSprite.sprite = productSo.sprite;
    }

    public void Interact()
    {
        if (Player.Instance.HandleSystem.HaveItems()) return;
        Item.SpawnItem(productSo.prefab, Player.Instance.HandleSystem);
        EconomyManager.Instance.AddMoney(-productSo.buyPrice);
    }
}