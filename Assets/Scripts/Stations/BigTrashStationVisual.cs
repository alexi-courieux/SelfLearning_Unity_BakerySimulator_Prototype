using System;
using UnityEngine;

public class BigTrashStationVisual : MonoBehaviour
{
    private static readonly int Door = Animator.StringToHash("Door");
    
    [SerializeField] private BigTrashStation trashStation;
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        trashStation.OnUse += TrashStation_OnUse;
    }

    private void OnDestroy()
    {
        trashStation.OnUse -= TrashStation_OnUse;
    }

    private void TrashStation_OnUse(object sender, EventArgs e)
    {
        _animator.SetTrigger(Door);
    }
}