using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class CustomerManager : MonoBehaviour
{
    public static CustomerManager Instance { get; private set; }

    private const float SpawnTimeMin = 10f;
    private const float SpawnTimeMax = 30f;
    
    [SerializeField] private Transform customerPrefab;
    [SerializeField] private CustomerVisualDictionarySo customerVisualVisualDictionarySo;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform despawnPoint;

    private List<Customer> _requestCustomerPool;
    public List<CheckoutStation> CheckoutStations { get; private set; }
    private float _spawnTimer;
    
    public Transform DespawnPoint => despawnPoint;

    private void Awake()
    {
        Instance = this;
        _requestCustomerPool = new List<Customer>();
        CheckoutStations = new List<CheckoutStation>();
    }

    private void Start() {
        ResetSpawnTimer();
    }

    private void Update()
    {
        _spawnTimer -= Time.deltaTime;
        if (_spawnTimer <= 0f)
        {
            SpawnCustomer();
            ResetSpawnTimer();
        }
    }

    private void SpawnCustomer()
    {
        CreateCustomer();
    }

    private void CreateCustomer()
    {
        Transform customerVisualPrefab = customerVisualVisualDictionarySo.GetRandomCustomerVisual();
        Transform customerTransform = Instantiate(customerPrefab, spawnPoint.position, spawnPoint.rotation);
        Customer customer = customerTransform.GetComponent<Customer>();
        Transform visualTransform = Instantiate(customerVisualPrefab, customerTransform.position, customerTransform.rotation, customerTransform);
        CustomerVisual visual = visualTransform.GetComponent<CustomerVisual>();
        visual.Customer = customer;
    }

    public void Despawn(Customer customer)
    {
        customer.gameObject.SetActive(false);
        bool haveRequests = OrderManager.Instance.HaveRequests(customer);
        if (haveRequests)
        {
            _requestCustomerPool.Add(customer);
        }
        else
        {
            customer.DestroySelf();
        }
    }

    public void SpawnForRequest(Customer customer)
    {
        _requestCustomerPool.Remove(customer);
        customer.transform.position = spawnPoint.position;
        customer.IsCollectingRequestOrder = true;
        customer.gameObject.SetActive(true);
    }

    private void ResetSpawnTimer()
    {
        _spawnTimer = Random.Range(SpawnTimeMin, SpawnTimeMax);
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