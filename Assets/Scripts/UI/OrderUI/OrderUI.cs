using System;
using UnityEngine;

public class OrderUI : MonoBehaviour
{
    [SerializeField] private Transform itemUITemplate;
    [SerializeField] private Customer customer;
    [SerializeField] private Transform itemUIContainer;

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
        if (_order.Type is not OrderType.Direct) return;
        
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
    }
}
