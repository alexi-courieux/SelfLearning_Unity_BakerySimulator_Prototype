using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OrderRequestItemSingleUI : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI quantityText;
    public void SetItem(HandleableItemSo item, int quantity) {
        itemIcon.sprite = item.sprite;
        quantityText.text = quantity.ToString();
    }
}
