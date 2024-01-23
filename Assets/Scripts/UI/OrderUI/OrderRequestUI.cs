using System;
using TMPro;
using UnityEngine;

public class OrderRequestUI : MonoBehaviour
{
    [SerializeField] private Transform itemUITemplate;
    [SerializeField] private Customer customer;
    [SerializeField] private Transform itemUIContainer;
    [SerializeField] private TextMeshProUGUI timerText;

    private Order _order;
    private void Start()
    {
        customer.OnPassingOrder += Customer_OnPassingOrder;
        customer.OnStateChange += Customer_OnStateChange;
        itemUITemplate.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    private void Customer_OnStateChange(object sender, CustomerState e)
    {
        if (e == CustomerState.Leaving)
        {
            Hide();
        }
    }

    private void Customer_OnPassingOrder(object sender, EventArgs e)
    {
        _order = customer.Order;
        if (_order.Type is not OrderType.Request) return;
        
        UpdateVisual();
        Show();
    }
    
    private void Show()
    {
        gameObject.SetActive(true);
    }
    
    private void Hide()
    {
        gameObject.SetActive(false);
    }
    
    private void UpdateVisual()
    {
        foreach (Transform child in itemUIContainer)
        {
            if (child == itemUITemplate) continue;
            Destroy(child.gameObject);
        }
        
        foreach ((HandleableItemSo item, int quantity) in _order.Items)
        {
            Transform itemUI = Instantiate(itemUITemplate, itemUIContainer);
            itemUI.gameObject.SetActive(true);
            OrderItemUI orderItemUI = itemUI.GetComponent<OrderItemUI>();
            orderItemUI.SetItem(item, quantity);
        }
        
        timerText.text = GetTimerText();
    }
    
    private string GetTimerText()
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(_order.TimeLimit);
        return $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
    }
}
