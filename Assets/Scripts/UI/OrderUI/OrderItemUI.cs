using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OrderItemUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI quantityText;

    public void SetItem(HandleableItemSo item, int quantity)
    {
        icon.sprite = item.sprite;
        quantityText.text = quantity.ToString();
    }
}