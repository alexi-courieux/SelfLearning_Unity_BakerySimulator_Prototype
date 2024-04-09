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
    
    
    public EventHandler<State> OnStateChanged;
    
    private float _timeoutTimer;
    private State _state;

    private State CurrentState
    {
        get => _state;
        set
        {
            _state = value;
            OnStateChanged?.Invoke(this, _state);
        }
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
        CurrentState = State.OnCall;
        Order = OrderManager.Instance.CreateOrder(OrderType.Request);
        _timeoutTimer = CallTimeout;
    }
    
    private void AcceptOrder()
    {
        CurrentState = State.Idle;
        OrderManager.Instance.AcceptRequest(Order);
    }

    public void Ring()
    {
        _timeoutTimer = RingTimeout;
        CurrentState = State.Ringing;
    }
}