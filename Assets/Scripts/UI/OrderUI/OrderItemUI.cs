using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AshLight.BakerySim.UI.OrderUI
{
    public class OrderItemUI : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI quantityText;

        public void SetItem(ProductSo product, int quantity)
        {
            icon.sprite = product.sprite;
            quantityText.text = quantity.ToString();
        }
    }
}