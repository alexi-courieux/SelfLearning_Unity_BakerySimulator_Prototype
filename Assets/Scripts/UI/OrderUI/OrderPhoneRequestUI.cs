using System;
using TMPro;
using UnityEngine;

public class OrderPhoneRequestUI : MonoBehaviour
{
    [SerializeField] private Transform itemUITemplate;
    [SerializeField] private PhoneStation phoneStation;
    [SerializeField] private Transform itemUIContainer;
    [SerializeField] private TextMeshProUGUI timerText;

    private Order _order;
    private void Start()
    {
        phoneStation.OnStateChange += PhoneStation_OnStateChange;
        itemUITemplate.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    private void PhoneStation_OnStateChange(object sender, PhoneStation.State state)
    {
        if (state is PhoneStation.State.OnCall)
        {
            _order = phoneStation.Order;
            UpdateVisual();
            Show();
        }
        else
        {
            Hide();
        }
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
        
        foreach ((ProductSo item, int quantity) in _order.Items)
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
