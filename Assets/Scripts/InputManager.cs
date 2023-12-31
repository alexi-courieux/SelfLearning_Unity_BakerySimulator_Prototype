using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    
    public EventHandler OnInteract;
    public EventHandler OnInteractAlt;
    public EventHandler OnDrop;
    public EventHandler OnCameraSwitch;
    public EventHandler OnPause;
    
    private InputActions _inputActions;

    private void Awake()
    {
        Instance = this;
        _inputActions = new InputActions();
        _inputActions.Enable();
        
        _inputActions.Player.Interact.performed += Interact_OnPerformed;
        _inputActions.Player.InteractAlt.performed += InteractAlt_OnPerformed;
        _inputActions.Player.Pause.performed += Pause_OnPerformed;
        _inputActions.Player.Camera.performed += Camera_OnPerformed;
        _inputActions.Player.Drop.performed += Drop_OnPerformed;
    }

    private void OnDestroy()
    {
        _inputActions.Player.Interact.performed -= Interact_OnPerformed;
        _inputActions.Player.InteractAlt.performed -= InteractAlt_OnPerformed;
        _inputActions.Player.Pause.performed -= Pause_OnPerformed;
        _inputActions.Player.Camera.performed -= Camera_OnPerformed;
        _inputActions.Player.Drop.performed -= Drop_OnPerformed;
        
        _inputActions.Dispose();
    }

    public Vector2 GetMovementVectorNormalized()
    {
        return _inputActions.Player.Move.ReadValue<Vector2>().normalized;
    }
    
    public Vector2 GetRotationVectorNormalized()
    {
        return _inputActions.Player.Rotate.ReadValue<Vector2>().normalized;
    }
    
    private void Interact_OnPerformed(InputAction.CallbackContext obj)
    {
        OnInteract?.Invoke(this, EventArgs.Empty);
    }
    
    private void InteractAlt_OnPerformed(InputAction.CallbackContext obj)
    {
        OnInteractAlt?.Invoke(this, EventArgs.Empty);
    }
    
    private void Pause_OnPerformed(InputAction.CallbackContext obj)
    {
        OnPause?.Invoke(this, EventArgs.Empty);
    }
    
    private void Camera_OnPerformed(InputAction.CallbackContext obj)
    {
        OnCameraSwitch?.Invoke(this, EventArgs.Empty);
    }
    
    private void Drop_OnPerformed(InputAction.CallbackContext obj)
    {
        OnDrop?.Invoke(this, EventArgs.Empty);
    }
}
