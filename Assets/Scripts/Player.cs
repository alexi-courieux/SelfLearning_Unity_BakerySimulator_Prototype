using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    public PlayerHoldSystem HoldSystem { get; private set; }
    
    private const float MaxInteractionDistance = 2f;
    
    [SerializeField] private float movementSpeed = 6f;
   
    [SerializeField] private float characterRotationSpeed = 5f;
    [SerializeField] private float idleCharacterRotationSpeed = 1f;
    [SerializeField] private Transform visualTransform;
    [SerializeField] private Transform interactionRaycastSpawnPoint;
    
    private CharacterController _characterController;

    private void Awake()
    {
        Instance = this;
        _characterController = GetComponent<CharacterController>();
        HoldSystem = GetComponent<PlayerHoldSystem>();
    }

    private void Start()
    {
        InputManager.Instance.OnInteract += InputManager_OnInteract;
        InputManager.Instance.OnInteractAlt += InputManager_OnInteractAlt;
    }

    private void OnDestroy()
    {
        InputManager.Instance.OnInteract -= InputManager_OnInteract;
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        Vector3 finalMovement = Vector3.zero;
        Vector2 movementInput = InputManager.Instance.GetMovementVectorNormalized();
        Transform cameraTransform = Camera.main!.transform;
        Vector3 moveDirection = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized * movementInput.y +
                                Vector3.ProjectOnPlane(cameraTransform.right, Vector3.up).normalized * movementInput.x;
        const float minimalMovementMagnitude = 0.1f;
        if (movementInput.magnitude >= minimalMovementMagnitude)
        {
            // Translate towards the camera direction
            moveDirection.y = 0;
            finalMovement += moveDirection * (movementSpeed * Time.fixedDeltaTime);
            
            // Rotate towards the movement direction
            Quaternion targetRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(moveDirection, Vector3.up), Vector3.up);
            visualTransform.rotation = Quaternion.Lerp(visualTransform.rotation, targetRotation, characterRotationSpeed * Time.fixedDeltaTime);
        }
        else
        {
            // Slowly rotate towards the camera direction
            Quaternion targetRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up), Vector3.up);
            visualTransform.rotation = Quaternion.Lerp(visualTransform.rotation, targetRotation, idleCharacterRotationSpeed * Time.fixedDeltaTime);
        }
        
        // Apply simplified gravity
        finalMovement += Physics.gravity * Time.fixedDeltaTime;
        
        _characterController.Move(finalMovement);
    }
    
    private void InputManager_OnInteract(object sender, EventArgs e)
    {
        if (!CheckForRaycastHit(out RaycastHit hitInfo)) return;

        if(hitInfo.transform.TryGetComponent(out ICanBeInteracted interactableComponent))
        {
            interactableComponent.Interact();
        }
    }
    
    private void InputManager_OnInteractAlt(object sender, EventArgs e)
    {
        if (!CheckForRaycastHit(out RaycastHit hitInfo)) return;
        if(hitInfo.transform.TryGetComponent(out ICanBeInteractedAlt interactableComponent))
        {
            interactableComponent.InteractAlt();
        }
    }

    private bool CheckForRaycastHit(out RaycastHit hitInfo)
    {
        const string layerStation = "Station";
        const string layerHoldable = "Holdable";
        
        Transform cameraTransform = Camera.main!.transform;
        int stationMask = LayerMask.GetMask(layerStation);
        int holdableMask = LayerMask.GetMask(layerHoldable);
        return Physics.Raycast(interactionRaycastSpawnPoint.position, cameraTransform.forward, out hitInfo,
            MaxInteractionDistance, stationMask | holdableMask);
    }
}
