using System;
using System.Linq;
using UnityEngine;

public class OrderRequestListUI : MonoBehaviour
{
    [SerializeField] private Transform orderTemplate;
    [SerializeField] private Transform orderContainer;
    
    private void Start()
    {
        orderTemplate.gameObject.SetActive(false);
        OrderManager.Instance.OnRequestListChanged += OrderManager_OnRequestListChanged;
    }

    private void OrderManager_OnRequestListChanged(object sender, EventArgs e)
    {
        UpdateVisual();
    }
    
    private void UpdateVisual()
    {
        foreach (Transform child in orderContainer)
        {
            if (child == orderTemplate) continue;
            Destroy(child.gameObject);
        }
        
        foreach (Order order in OrderManager.Instance.Requests.OrderBy(r => r.TimeLimit))
        {
            Transform orderUI = Instantiate(orderTemplate, orderContainer);
            OrderRequestSingleUI orderRequestUI = orderUI.GetComponent<OrderRequestSingleUI>();
            orderRequestUI.SetOrder(order);
            orderUI.gameObject.SetActive(true);
        }
    }
}
