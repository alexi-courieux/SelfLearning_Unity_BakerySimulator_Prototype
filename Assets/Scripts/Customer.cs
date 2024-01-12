using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
public enum CustomerState
{
    CollectingRequestOrder,
    WaitingInQueue,
    WaitingToOrder,
    WaitingForOrderCompletion,
    Leaving
}

[RequireComponent(typeof(NavMeshAgent))]
public class Customer : MonoBehaviour, IHandleItems
{
    public EventHandler<CustomerState> OnStateChange;
    
    [SerializeField] private Transform itemSlot;
    private HandleableItem _item;
    private bool _isCollectingRequestOrder;

    private CustomerState _state;
    private CustomerState CurrentState
    {
        get => _state;
        set
        {
            _state = value;
            OnStateChange?.Invoke(this, _state);
            HandleStateChange();
        }
    }

    private NavMeshAgent _agent;
    private CheckoutStation _checkoutStation;
    private const float PatienceMax = 10f;
    private const float PatienceGainOnOrder = 20f;
    private const float PatienceLossOnWaitingForOrderCompletion = 1f;
    private const float PatienceLossOnWaitingToOrder = 1.2f;
    private const float PatienceLossOnWaitingInQueue = 0.8f;
    private float _patience;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        _patience = PatienceMax;
        if (_isCollectingRequestOrder)
        {
            CurrentState = CustomerState.CollectingRequestOrder;
        }
        else
        {
            CheckoutStation checkout = CustomerManager.Instance.TryGetCheckoutStation(this);
            if (checkout is not null)
            {
                _checkoutStation = checkout;
                checkout.AddCustomer(this);
                MoveTo(checkout.GetCustomerPosition(this));
                CurrentState = CustomerState.WaitingInQueue;
                _checkoutStation.OnCustomerLeave += CheckoutStation_OnCustomerLeave;
            }
            else
            {
                CurrentState = CustomerState.Leaving;
            }
        }
    }

    private void CheckoutStation_OnCustomerLeave(object sender, EventArgs e)
    {
        if(CurrentState is CustomerState.Leaving) return;
        MoveTo(_checkoutStation.GetCustomerPosition(this));
        if (_checkoutStation.GetCustomerPositionIndex(this) == 0)
        {
            CurrentState = CustomerState.WaitingToOrder;
        }
    }

    private void Update()
    {
        switch (CurrentState)
        {
            case CustomerState.CollectingRequestOrder:
                
                break;
            case CustomerState.WaitingInQueue:
                _patience -= Time.deltaTime * PatienceLossOnWaitingInQueue;
                break;
            case CustomerState.WaitingToOrder:
                _patience -= Time.deltaTime * PatienceLossOnWaitingToOrder;
                break;
            case CustomerState.WaitingForOrderCompletion:
                _patience -= Time.deltaTime * PatienceLossOnWaitingForOrderCompletion;
                break;
            case CustomerState.Leaving:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        if (_patience <= 0f)
        {
            CurrentState = CustomerState.Leaving;
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    public void AddItem(HandleableItem item)
    {
        _item = item;
    }

    public HandleableItem[] GetItems()
    {
        return new[] {_item};
    }

    public void ClearItem(HandleableItem item)
    {
        _item = null;
    }

    public bool HaveItems()
    {
        return _item is not null;
    }

    public Transform GetAvailableItemSlot()
    {
        return itemSlot;
    }

    public bool HasAvailableSlot()
    {
        return _item is null;
    }

    private void DestroySelf()
    {
        CustomerManager.Instance.RemoveCustomer(this);
        Destroy(gameObject);
    }
    
    private void HandleStateChange()
    {
        switch (CurrentState)
        {
            case CustomerState.CollectingRequestOrder:
                break;
            case CustomerState.WaitingInQueue:
                break;
            case CustomerState.WaitingToOrder:
                // TODO Wait for the checkout station to call the order
                // TODO Choose if the order is a request or a normal order
                // TODO Wait for call
                break;
            case CustomerState.WaitingForOrderCompletion:
                /*
                 TODO Once called, create an order to the OrderManager
                 - If the order is a request
                    activate the request UI
                    wait for the checkout station to tell if the request is accepted or not
                 - If the order is a normal order
                    activate the order UI
                    wait for the checkout station to tell if the order is completed, failed or refused
                */
                _patience += PatienceGainOnOrder;
                break;
            case CustomerState.Leaving:
                MoveTo(CustomerManager.Instance.DespawnPoint.position, DestroySelf);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    private void MoveTo(Vector3 position, Action onDestinationReached = null)
    {
        _agent.SetDestination(position);
        StartCoroutine(WaitForDestinationReached(onDestinationReached));
    }
    
    private IEnumerator WaitForDestinationReached(Action onDestinationReached)
    {
        while (_agent.pathPending)
        {
            yield return new WaitForSeconds(0.5f);
        }
        while (_agent.remainingDistance > _agent.stoppingDistance)
        {
            yield return new WaitForSeconds(0.5f);
        }
        onDestinationReached?.Invoke();
    }

    public void Checkout()
    {
        CurrentState = CustomerState.Leaving;
    }
}