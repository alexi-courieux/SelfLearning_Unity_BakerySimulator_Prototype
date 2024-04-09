using System;
using TMPro;
using UnityEngine;

public class OrderRequestSingleUI : MonoBehaviour
{
    [SerializeField] private Transform itemUITemplate;
    [SerializeField] private Transform itemUIContainer;
    [SerializeField] private TextMeshProUGUI deadlineText;

    private Order _order;
    
    private void Start()
    {
        itemUITemplate.gameObject.SetActive(false);
    }

    private void Update()
    {
        deadlineText.text = GetTimerText(_order.TimeLimit);
    }

    public void SetOrder(Order order)
    {
        _order = order;
        foreach (Transform child in itemUIContainer)
        {
            if (child == itemUITemplate) continue;
            Destroy(child.gameObject);
        }
        
        foreach ((HandleableItemSo item, int quantity) in order.Items)
        {
            Transform itemUI = Instantiate(itemUITemplate, itemUIContainer);
            itemUI.gameObject.SetActive(true);
            OrderRequestItemSingleUI orderItemUI = itemUI.GetComponent<OrderRequestItemSingleUI>();
            orderItemUI.SetItem(item, quantity);
        }
        
        deadlineText.text = GetTimerText(order.TimeLimit);
    }

    private string GetTimerText(float time)
    {
        if(time < 0) return "00:00";
        TimeSpan timeSpan = TimeSpan.FromSeconds(time);
        return $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
    }
}
