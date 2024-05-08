﻿using System;
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
public class Customer : MonoBehaviour, IInteractable, IInteractableAlt
{
    public EventHandler<CustomerState> OnStateChange;
    public EventHandler OnPassingOrder, OnOrderFailed, OnOrderCompleted, OnLowPatience, OnOutOfPatience;
    
    [SerializeField] private Transform itemSlot;
    
    private const float PatienceMax = 100f;
    private const float PatienceGainOnOrder = 20f;
    private const float PatienceLossOnWaitingForOrderCompletion = 1f;
    private const float PatienceLossOnWaitingToOrder = 1.2f;
    private const float PatienceLossOnWaitingInQueue = 0.8f;
    private const float PatienceLossOnOrderFail = 10f;
    private const float DirectOrderProbability = 0.8f;
    private const float LowPatienceThreshold = 10f;
    
    public bool IsCollectingRequestOrder { get; set; }
    public Order Order { get; set; }
    
    public float Velocity => _agent.velocity.magnitude;
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

    private Item _item;
    private CustomerState _state;
    private NavMeshAgent _agent;
    private CheckoutStation _checkoutStation;
    private float _patience;
    private bool _lowPatienceThresholdReached;
    
    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    private void OnEnable()
    {
        Initialize();
    }

    private void OnDisable()
    {
        if (_checkoutStation is not null)
        {
            _checkoutStation.OnAnyCustomerLeave -= CheckoutStation_OnAnyCustomerLeave;
        }
        StopAllCoroutines();
    }
    
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
    
    private void Initialize()
    {
        _patience = PatienceMax;
        CheckoutStation checkout = CustomerManager.Instance.TryGetCheckoutStation(this);
        if (checkout is not null)
        {
            _checkoutStation = checkout;
            checkout.AddCustomer(this);
            MoveInQueue(StartLosePatience);
            _checkoutStation.OnAnyCustomerLeave += CheckoutStation_OnAnyCustomerLeave;
        }
        else
        {
            CurrentState = CustomerState.Leaving;
        }
    }

    public void Interact()
    {
        switch (CurrentState)
        {
            case CustomerState.WaitingToOrder:
                if (IsCollectingRequestOrder)
                {
                    GetOrder();
                }
                else
                {
                    CreateOrder();
                }
                break;
            case CustomerState.WaitingForOrderCompletion:
                if (Order.Type is OrderType.Direct)
                {
                    _checkoutStation.CheckOrderCompletion(this);
                }
                else
                {
                    AcceptOrder();
                }
                break;
            case CustomerState.CollectingRequestOrder:
                _checkoutStation.CheckOrderCompletion(this);
                break;
            case CustomerState.WaitingInQueue:
            case CustomerState.Leaving:
            default:
                return;
        }
    }
    
    public void InteractAlt()
    {
        switch (CurrentState)
        {
            case CustomerState.CollectingRequestOrder:
            case CustomerState.WaitingToOrder:
            case CustomerState.WaitingForOrderCompletion:
                LeaveUnhappy();
                break;
            case CustomerState.WaitingInQueue:
            case CustomerState.Leaving:
            default:
                return;
        }
    }

    private void CheckoutStation_OnAnyCustomerLeave(object sender, EventArgs e)
    {
        if(CurrentState is CustomerState.Leaving) return;
        MoveInQueue();
    }

    private void MoveInQueue(Action onDestinationReached = null)
    {
        MoveTo(_checkoutStation.GetCustomerPosition(this), onDestinationReached);
        bool isFirst = _checkoutStation.GetCustomerPositionIndex(this) == 0;
        CurrentState = isFirst 
            ? CustomerState.WaitingToOrder
            : CustomerState.WaitingInQueue;
    }
    
    private void HandleStateChange()
    {
        switch (CurrentState)
        {
            case CustomerState.CollectingRequestOrder:
                _patience = Mathf.Min(PatienceMax, _patience + PatienceGainOnOrder);
                Logger.LogInfo(Order, this);
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
                if (IsCollectingRequestOrder)
                {
                    OrderManager.Instance.RemoveRequest(this);
                }
                MoveTo(CustomerManager.Instance.DespawnPoint.position, Despawn);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    private void Despawn()
    {
        CustomerManager.Instance.Despawn(this);
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
            ? Random.Range(0f, 1f) < DirectOrderProbability ? OrderType.Direct : OrderType.Request
            : OrderType.Request;
        Order = OrderManager.Instance.CreateOrder(orderType, this);
        OnPassingOrder?.Invoke(this, EventArgs.Empty);
        CurrentState = CustomerState.WaitingForOrderCompletion;
    }

    public void GetOrder()
    {
        OnPassingOrder?.Invoke(this, EventArgs.Empty);
        CurrentState = CustomerState.CollectingRequestOrder;
    }

    public void ReceiveFailedOrder()
    {
        _patience -= PatienceLossOnOrderFail;
        OnOrderFailed?.Invoke(this, EventArgs.Empty);
    }

    public void LeaveHappy()
    {
        OnOrderCompleted?.Invoke(this, EventArgs.Empty);
        CurrentState = CustomerState.Leaving;
    }
    
    public void LeaveUnhappy()
    {
        OnOrderFailed?.Invoke(this, EventArgs.Empty);
        CurrentState = CustomerState.Leaving;
    }

    private void StartLosePatience()
    {
        StartCoroutine(LosePatienceOverTime());    
    }
    
    private void AcceptOrder()
    {
        OrderManager.Instance.AcceptRequest(Order);
        LeaveHappy();
    }

    private IEnumerator LosePatienceOverTime()
    {
        while (_patience > 0f)
        {
            yield return new WaitForSeconds(1f);
            switch (CurrentState)
            {
                case CustomerState.CollectingRequestOrder:
                    _patience -= PatienceLossOnWaitingForOrderCompletion;
                    break;
                case CustomerState.WaitingInQueue:
                    _patience -= PatienceLossOnWaitingInQueue;
                    break;
                case CustomerState.WaitingToOrder:
                    _patience -= PatienceLossOnWaitingToOrder;
                    break;
                case CustomerState.WaitingForOrderCompletion:
                    _patience -= PatienceLossOnWaitingForOrderCompletion;
                    break;
                case CustomerState.Leaving:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            if (!_lowPatienceThresholdReached && _patience <= LowPatienceThreshold)
            {
                _lowPatienceThresholdReached = true;
                OnLowPatience?.Invoke(this, EventArgs.Empty);
            }
        }
        OnOutOfPatience?.Invoke(this, EventArgs.Empty);
        CurrentState = CustomerState.Leaving;
    }
}