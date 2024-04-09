using System;
using UnityEngine;

public class PhoneStation : MonoBehaviour, IInteractable, IInteractableAlt
{
    public enum State
    {
        Idle,
        Ringing,
        OnCall
    }
    
    private const float RingTimeout = 20f;
    private const float CallTimeout = 30f;
    public Order Order { get; private set; }
    
    
    public EventHandler<State> OnStateChange;
    
    private float _timeoutTimer;
    private State _state;

    private State CurrentState
    {
        get => _state;
        set
        {
            _state = value;
            OnStateChange?.Invoke(this, _state);
        }
    }

    private void Start()
    {
        OrderManager.Instance.OnPhoneCall += OrderManager_OnPhoneCall;
    }

    private void Update()
    {
        if (CurrentState is State.Idle) return;
        
        _timeoutTimer -= Time.deltaTime;
        if (_timeoutTimer <= 0f)
        {
            CurrentState = State.Idle;
        }
    }

    public void Interact()
    {
        switch (CurrentState)
        {
            case State.Idle:
                break;
            case State.Ringing:
                GetOrder();
                break;
            case State.OnCall:
                AcceptOrder();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    public void InteractAlt()
    {
        switch (CurrentState)
        {
            case State.Idle:
                break;
            case State.Ringing:
                CurrentState = State.Idle;
                break;
            case State.OnCall:
                CurrentState = State.Idle;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    private void GetOrder()
    {
        Order = OrderManager.Instance.CreateOrder(OrderType.Request);
        CurrentState = State.OnCall;
        _timeoutTimer = CallTimeout;
    }
    
    private void AcceptOrder()
    {
        CurrentState = State.Idle;
        OrderManager.Instance.AcceptRequest(Order);
    }

    private void OrderManager_OnPhoneCall(object sender, EventArgs e)
    {
        if (CurrentState is State.Idle) Ring();
    }
    
    private void Ring()
    {
        CurrentState = State.Ringing;
        _timeoutTimer = RingTimeout;
    }
}