using System;
using UnityEngine;

public class CustomerVisual : MonoBehaviour
{
    private static readonly int Velocity = Animator.StringToHash("velocity");
    private static readonly int Talk = Animator.StringToHash("talk");
    private static readonly int LowPatience = Animator.StringToHash("lowPatience");
    private static readonly int OutOfPatience = Animator.StringToHash("outOfPatience");
    private static readonly int OrderFailed = Animator.StringToHash("orderFailed");
    private static readonly int OrderCompleted = Animator.StringToHash("orderCompleted");

    private Customer _customer;
    public Customer Customer
    {
        set
        {
            _customer = value;
            _customer.OnPassingOrder += Customer_OnPassingOrder;
            _customer.OnOrderFailed += Customer_OnOrderFailed;
            _customer.OnOrderCompleted += Customer_OnOrderCompleted;
            _customer.OnLowPatience += Customer_OnLowPatience;
            _customer.OnOutOfPatience += Customer_OnOutOfPatience;
        }
    }

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (_customer is null) return;
        _animator.SetFloat(Velocity, _customer.Velocity);
    }

    private void OnDestroy()
    {
        _customer.OnPassingOrder -= Customer_OnPassingOrder;
    }
    
    private void Customer_OnPassingOrder(object sender, EventArgs e)
    {
        _animator.SetTrigger(Talk);
    }
    
    private void Customer_OnOrderFailed(object sender, EventArgs e)
    {
        _animator.SetTrigger(OrderFailed);
    }
    
    private void Customer_OnOrderCompleted(object sender, EventArgs e)
    {
        _animator.SetTrigger(OrderCompleted);
    }
    
    private void Customer_OnLowPatience(object sender, EventArgs e)
    {
        _animator.SetTrigger(LowPatience);
    }
    
    private void Customer_OnOutOfPatience(object sender, EventArgs e)
    {
        _animator.SetTrigger(OutOfPatience);
    }
}