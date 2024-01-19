using System;
using UnityEngine;

public class CustomerVisual : MonoBehaviour
{
    private static readonly int Velocity = Animator.StringToHash("velocity");
    private static readonly int Talk = Animator.StringToHash("talk");

    private Customer _customer;
    public Customer Customer
    {
        set
        {
            _customer = value;
            _customer.OnPassingOrder += Customer_OnPassingOrder;
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
}