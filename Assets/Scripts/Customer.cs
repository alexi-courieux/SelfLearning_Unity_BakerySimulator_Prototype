using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

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
    
    private const float PatienceMax = 100f;
    private const float PatienceGainOnOrder = 20f;
    private const float PatienceLossOnWaitingForOrderCompletion = 1f;
    private const float PatienceLossOnWaitingToOrder = 1.2f;
    private const float PatienceLossOnWaitingInQueue = 0.8f;
    private const float PatienceLossOnOrderFail = 10f;
    private const float DirectOrderProbability = 0.8f;
    private HandleableItem _item;
    
    private bool _isCollectingRequestOrder;

    private CustomerState _state;
    private NavMeshAgent _agent;
    private CheckoutStation _checkoutStation;
    private float _patience;
    
    public Order Order { get; private set; }
    
    public CustomerState CurrentState
    {
        get => _state;
        private set
        {
            _state = value;
            OnStateChange?.Invoke(this, _state);
            HandleStateChange();
        }
    }


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
                MoveInQueue();
                _checkoutStation.OnAnyCustomerLeave += CheckoutStation_OnAnyCustomerLeave;
            }
            else
            {
                CurrentState = CustomerState.Leaving;
            }
        }
    }

    private void CheckoutStation_OnAnyCustomerLeave(object sender, EventArgs e)
    {
        if(CurrentState is CustomerState.Leaving) return;
        MoveInQueue();
    }

    private void MoveInQueue()
    {
        MoveTo(_checkoutStation.GetCustomerPosition(this));
        CurrentState = _checkoutStation.GetCustomerPositionIndex(this) == 0 ? CustomerState.WaitingToOrder : CustomerState.WaitingInQueue;
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
        OrderManager.Instance.RemoveOrder(this);
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
                break;
            case CustomerState.WaitingForOrderCompletion:
                _patience = Mathf.Min(PatienceMax, _patience + PatienceGainOnOrder);
                Logger.LogInfo(Order, this);
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

    public void CreateOrder()
    {
        OrderType orderType = OrderManager.Instance.CanPerformDirectOrder()
            ? Random.Range(0, 1) < DirectOrderProbability ? OrderType.Direct : OrderType.Request
            : OrderType.Request;
        Order = OrderManager.Instance.CreateOrder(this, orderType);
        CurrentState = CustomerState.WaitingForOrderCompletion;
    }
    
    public void ReceiveFailedOrder()
    {
        _patience -= PatienceLossOnOrderFail;
    }

    public void Leave()
    {
        CurrentState = CustomerState.Leaving;
    }
}