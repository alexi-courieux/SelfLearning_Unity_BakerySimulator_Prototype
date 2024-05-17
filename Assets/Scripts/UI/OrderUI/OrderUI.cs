using UnityEngine;

public class OrderUI : MonoBehaviour
{
    [SerializeField] private Transform itemUITemplate;
    [SerializeField] private Customer customer;
    [SerializeField] private Transform itemUIContainer;

    private void Start()
    {
        customer.OnStateChange += Customer_OnStateChange;
        itemUITemplate.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    private void Customer_OnStateChange(object sender, CustomerState state)
    {
        if (state is CustomerState.CollectingRequestOrder || state is CustomerState.WaitingForOrderCompletion && customer.Order.Type is OrderType.Direct)
        {
            UpdateVisual();
            Show();
        }
        
        if (state == CustomerState.Leaving)
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
        
        foreach ((ProductSo item, int quantity) in customer.Order.Items)
        {
            Transform itemUI = Instantiate(itemUITemplate, itemUIContainer);
            itemUI.gameObject.SetActive(true);
            OrderItemUI orderItemUI = itemUI.GetComponent<OrderItemUI>();
            orderItemUI.SetItem(item, quantity);
        }
    }
}
