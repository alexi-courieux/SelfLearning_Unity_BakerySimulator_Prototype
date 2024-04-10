using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    public static CustomerManager Instance { get; private set; }

    private const float SpawnFrequency = 10f;
    private const float SpawnChance = 0.63f;
    
    [SerializeField] private Transform customerPrefab;
    [SerializeField] private CustomerVisualDictionarySo customerVisualVisualDictionarySo;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform despawnPoint;
    
    public List<CheckoutStation> CheckoutStations { get; private set; }
    private float _spawnTimer;
    
    public Transform DespawnPoint => despawnPoint;

    private void Awake()
    {
        Instance = this;
        CheckoutStations = new List<CheckoutStation>();
    }

    private void Start() {
        _spawnTimer = SpawnFrequency;
    }

    private void Update()
    {
        _spawnTimer -= Time.deltaTime;
        if (_spawnTimer <= 0f)
        {
            if (Random.value < SpawnChance)
            {
                SpawnCustomer();
            }
            _spawnTimer = SpawnFrequency;
        }
    }

    private void SpawnCustomer()
    {
        CreateCustomer();
    }

    private Customer CreateCustomer()
    {
        Transform customerVisualPrefab = customerVisualVisualDictionarySo.GetRandomCustomerVisual();
        Transform customerTransform = Instantiate(customerPrefab, spawnPoint.position, spawnPoint.rotation);
        Customer customer = customerTransform.GetComponent<Customer>();
        Transform visualTransform = Instantiate(customerVisualPrefab, customerTransform.position, customerTransform.rotation, customerTransform);
        CustomerVisual visual = visualTransform.GetComponent<CustomerVisual>();
        visual.Customer = customer;
        return customer;
    }

    public void Despawn(Customer customer)
    {
        customer.gameObject.SetActive(false);
        bool haveRequests = OrderManager.Instance.HaveRequests(customer);
        if (!haveRequests)
        {
            customer.DestroySelf();
        }
    }

    public void RespawnForRequest(Customer customer)
    {
        customer.transform.position = spawnPoint.position;
        customer.IsCollectingRequestOrder = true;
        customer.gameObject.SetActive(true);
    }

    public void SpawnForRequest(Order order)
    {
        Customer customer = CreateCustomer();
        customer.IsCollectingRequestOrder = true;
        customer.Order = order;
        order.Customer = customer;
    }

    public CheckoutStation TryGetCheckoutStation(Customer requestingCustomer)
    {
        CheckoutStation selectedCheckout = null;
        foreach (CheckoutStation checkout in CheckoutStations.Where(checkout => checkout.IsAvailable()))
        {
            if (selectedCheckout is null)
            {
                selectedCheckout = checkout;
            }
            else
            {
                if (checkout.CustomerCount() < selectedCheckout.CustomerCount())
                {
                    // station is less busy than selectedStation
                    selectedCheckout = checkout;
                } 
                else
                {
                    if (checkout.CustomerCount() > selectedCheckout.CustomerCount()) continue;
                    Vector3 customerPosition = requestingCustomer.transform.position;
                    float distanceToCheckout = Vector3.Distance(customerPosition, checkout.transform.position);
                    float distanceToSelectedCheckout = Vector3.Distance(customerPosition, selectedCheckout.transform.position);
                    if (distanceToCheckout < distanceToSelectedCheckout)
                    {
                        // station is closer than selectedStation
                        selectedCheckout = checkout;
                    }
                }
            }
        }
        return selectedCheckout;
    }
}