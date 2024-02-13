using System;
using UnityEngine;

public class DisplayStationVisual : MonoBehaviour
{
    private static readonly int Door = Animator.StringToHash("Door");
        
    [SerializeField] private DisplayStation station;
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        station.OnPutIn += OvenStation_OnPutIn;
        station.OnTakeOut += OvenStation_OnTakeOut;
    }

    private void OvenStation_OnTakeOut(object sender, EventArgs e)
    {
        _animator.SetTrigger(Door);
    }

    private void OvenStation_OnPutIn(object sender, EventArgs e)
    {
        _animator.SetTrigger(Door);
    }
}
