using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Player : MonoBehaviour
{
    private const string LayerStation = "Station";
    private const string LayerHandleableitem = "HandleableItem";
    public static Player Instance { get; private set; }

    public PlayerItemHandlingSystem HandleSystem { get; private set; }

    private const float MaxInteractionDistance = 2f;

    [SerializeField] private float movementSpeed = 6f;

    [SerializeField] private float characterRotationSpeed = 5f;
    [SerializeField] private float idleCharacterRotationSpeed = 1f;
    [SerializeField] private Transform visualTransform;
    [SerializeField] private Transform playerPositionForRaycast;

    private CharacterController _characterController;

    private void Awake()
    {
        Instance = this;
        _characterController = GetComponent<CharacterController>();
        HandleSystem = GetComponent<PlayerItemHandlingSystem>();
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
        Vector3 moveDirection =
            Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized * movementInput.y +
            Vector3.ProjectOnPlane(cameraTransform.right, Vector3.up).normalized * movementInput.x;
        const float minimalMovementMagnitude = 0.1f;
        if (movementInput.magnitude >= minimalMovementMagnitude)
        {
            // Translate towards the camera direction
            moveDirection.y = 0;
            finalMovement += moveDirection * (movementSpeed * Time.fixedDeltaTime);

            // Rotate towards the movement direction
            Quaternion targetRotation =
                Quaternion.LookRotation(Vector3.ProjectOnPlane(moveDirection, Vector3.up), Vector3.up);
            visualTransform.rotation = Quaternion.Lerp(visualTransform.rotation, targetRotation,
                characterRotationSpeed * Time.fixedDeltaTime);
        }
        else
        {
            // Slowly rotate towards the camera direction
            Quaternion targetRotation =
                Quaternion.LookRotation(Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up), Vector3.up);
            visualTransform.rotation = Quaternion.Lerp(visualTransform.rotation, targetRotation,
                idleCharacterRotationSpeed * Time.fixedDeltaTime);
        }

        // Apply simplified gravity
        finalMovement += Physics.gravity * Time.fixedDeltaTime;

        _characterController.Move(finalMovement);
    }

    private void InputManager_OnInteract(object sender, EventArgs e)
    {
        if (!CheckForRaycastHit(out RaycastHit hitInfo)) return;

        if (hitInfo.transform.TryGetComponent(out IInteractable interactableComponent))
        {
            interactableComponent.Interact();
        }
    }

    private void InputManager_OnInteractAlt(object sender, EventArgs e)
    {
        if (!CheckForRaycastHit(out RaycastHit hitInfo)) return;
        if (hitInfo.transform.TryGetComponent(out IInteractableAlt interactableComponent))
        {
            interactableComponent.InteractAlt();
        }
    }

    private bool CheckForRaycastHit(out RaycastHit hitInfo)
    {
        Transform cameraTransform = Camera.main!.transform;
        int stationMask = LayerMask.GetMask(LayerStation);
        int handleableItemMask = LayerMask.GetMask(LayerHandleableitem);
        Ray ray = Camera.main!.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        float playerDistanceFromCamera = Vector3.Distance(playerPositionForRaycast.position, cameraTransform.position);
        ray.origin += cameraTransform.forward * playerDistanceFromCamera;
        bool hit = Physics.Raycast(ray, out hitInfo, MaxInteractionDistance);
        if (!hit) return false;
        int targetLayer = 1 << hitInfo.transform.gameObject.layer;
        return targetLayer == stationMask || targetLayer == handleableItemMask;
    }
}
