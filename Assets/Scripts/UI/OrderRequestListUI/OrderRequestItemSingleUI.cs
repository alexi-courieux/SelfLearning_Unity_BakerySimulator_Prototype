using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AshLight.BakerySim.UI.OrderRequestListUI
{
    public class OrderRequestItemSingleUI : MonoBehaviour
    {
        [SerializeField] private Image itemIcon;
        [SerializeField] private TextMeshProUGUI quantityText;
        public void SetItem(ProductSo product, int quantity) {
            itemIcon.sprite = product.sprite;
            quantityText.text = quantity.ToString();
        }
    }
}
