using System;
using Cinemachine;
using UnityEngine;

public class Player : MonoBehaviour
{
    public EventHandler<ICanBeInteracted> OnFocusInteractableItem;
    
    private const float MaxInteractionDistance = 2f;
    
    [SerializeField] private float movementSpeed = 6f;
    [SerializeField] private float rotationSpeedH = 250f;
    [SerializeField] private float rotationSpeedV = 1.5f;
    [SerializeField] private float characterRotationSpeed = 5f;
    [SerializeField] private float idleCharacterRotationSpeed = 1f;
    [SerializeField] private Transform interactionRaycastSpawnPoint;
    
    private CharacterController _characterController;
    private CinemachineFreeLook _cameraFreelook;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _cameraFreelook = FindObjectOfType<CinemachineFreeLook>();
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

    private void Update()
    {
        Rotate();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        Vector2 movementInput = InputManager.Instance.GetMovementVectorNormalized();
        Transform cameraTransform = Camera.main!.transform;
        Vector3 moveDirection = (cameraTransform.forward * movementInput.y) + (cameraTransform.right * movementInput.x);
        const float minimalMovementMagnitude = 0.1f;
        if (movementInput.magnitude >= minimalMovementMagnitude)
        {
            // Translate towards the camera direction
            moveDirection.y = 0;
            _characterController.Move(moveDirection * (movementSpeed * Time.fixedDeltaTime));
            
            // Rotate towards the movement direction
            Quaternion targetRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(moveDirection, Vector3.up), Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, characterRotationSpeed * Time.fixedDeltaTime);
        }
        else
        {
            // Slowly rotate towards the camera direction
            Quaternion targetRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up), Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, idleCharacterRotationSpeed * Time.fixedDeltaTime);
        }
        
    }

    private void Rotate()
    {
        Vector2 rotationVector = InputManager.Instance.GetRotationVectorNormalized();
        _cameraFreelook.m_XAxis.Value += rotationVector.x * rotationSpeedH * Time.deltaTime;
        _cameraFreelook.m_YAxis.Value += -rotationVector.y * rotationSpeedV * Time.deltaTime;
    }
    
    // TODO Might be useful later during interactable highlights, use on Update function    
    /*private void SearchForInteraction()
    {
        Transform cameraTransform = Camera.main!.transform;
        
        if (!Physics.Raycast(transform.position, cameraTransform.forward, out RaycastHit hitInfo,
                MaxInteractionDistance)) return;
        
        if(hitInfo.transform.TryGetComponent(out ICanBeInteracted interactableComponent))
        {
                OnFocusInteractableItem?.Invoke(this, interactableComponent);
        }
    }*/
    
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
        Transform cameraTransform = Camera.main!.transform;
        return Physics.Raycast(interactionRaycastSpawnPoint.position, cameraTransform.forward, out hitInfo,
            MaxInteractionDistance);
    }
}
