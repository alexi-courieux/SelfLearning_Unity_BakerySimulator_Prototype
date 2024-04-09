using System;
using UnityEngine;

public class PhoneStationVisual : MonoBehaviour
{
    private static readonly int Ring = Animator.StringToHash("Ring");
    
    [SerializeField] private PhoneStation phoneStation;
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        phoneStation.OnStateChanged += OnStateChanged;
    }
    
    private void OnStateChanged(object sender, PhoneStation.State state)
    {
        switch (state)
        {
            case PhoneStation.State.Idle:
                _animator.SetBool(Ring, false);
                break;
            case PhoneStation.State.Ringing:
                _animator.SetBool(Ring, true);
                break;
            case PhoneStation.State.OnCall:
                _animator.SetBool(Ring, false);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}