using System;
using UnityEngine;

public class TrashStation : MonoBehaviour, IInteractable, IInteractableAlt
{
    public EventHandler<int> OnTrashAmountChanged;
    
    [SerializeField] private int trashAmountMax;
    [SerializeField] private ProductSo trashbagProduct;
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
        if (!Player.Instance.HandleSystem.HaveItems<Product>()) return;
        Product product = Player.Instance.HandleSystem.GetItem() as Product;
        if(product!.ProductSo == trashbagProduct) return;
        product.DestroySelf();
        TrashAmount++;
    }

    public void InteractAlt()
    {
        if (_trashAmount is 0) return;
        if (Player.Instance.HandleSystem.HaveItems<Product>()) return;
        TrashAmount = 0;
        Item.SpawnItem<Product>(trashbagProduct.prefab, Player.Instance.HandleSystem);
    }
    
    public int GetTrashAmountMax()
    {
        return trashAmountMax;
    }
}
