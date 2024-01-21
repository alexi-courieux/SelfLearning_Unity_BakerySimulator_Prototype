using System;
using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    private static readonly int Velocity = Animator.StringToHash("velocity");
    
    [SerializeField] private Player player;
    private Animator _animator;
    private static readonly int Grab = Animator.StringToHash("grab");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        player.OnPlayerMove += Player_OnPlayerMove;
        player.OnPlayerInteract += Player_OnPlayerInteract;
        player.OnPlayerInteractAlt += Player_OnPlayerInteractAlt;
    }

    private void Player_OnPlayerInteractAlt(object sender, EventArgs e)
    {
        _animator.SetTrigger(Grab);
    }

    private void Player_OnPlayerInteract(object sender, EventArgs e)
    {
        _animator.SetTrigger(Grab);
    }

    private void Player_OnPlayerMove(object sender, float velocity)
    {
        _animator.SetFloat(Velocity, velocity);
    }
}