using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    private const string LayerStation = "Station";
    private const string LayerHandleableitem = "HandleableItem";
    private const string LayerCustomer = "Customer";

    private int _stationMask;
    private int _handleableItemMask;
    private int _customerMask;
    public static Player Instance { get; private set; }

    public EventHandler<float> OnPlayerMove;
    public EventHandler OnPlayerInteract;
    public EventHandler OnPlayerInteractAlt;
    
    public PlayerItemHandlingSystem HandleSystem { get; private set; }

    private const float MaxInteractionDistance = 4f;

    [SerializeField] private float movementSpeed = 6f;

    [SerializeField] private float characterRotationSpeed = 5f;
    [SerializeField] private float idleCharacterRotationSpeed = 1f;
    [SerializeField] private Transform visualTransform;
    [SerializeField] private Transform playerPositionForRaycast;

    private CharacterController _characterController;
    private Camera _camera;
    private IFocusable _focusedObject;

    private void Awake()
    {
        _stationMask = LayerMask.GetMask(LayerStation);
        _handleableItemMask = LayerMask.GetMask(LayerHandleableitem);
        _customerMask = LayerMask.GetMask(LayerCustomer);
        
        Instance = this;
        _characterController = GetComponent<CharacterController>();
        HandleSystem = GetComponent<PlayerItemHandlingSystem>();
        _camera = Camera.main;
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

    private void Update()
    {
        if (CheckForRaycastHit(out RaycastHit hitInfo, new[] {_stationMask}))
        {
            if (hitInfo.transform.TryGetComponent(out IFocusable focusable))
            {
                if (focusable == _focusedObject) return;
                
                _focusedObject?.StopFocus();
                _focusedObject = focusable;
                focusable.Focus();
                return;
            }
        }
        _focusedObject?.StopFocus();
        _focusedObject = null;
    }

    private void Move()
    {
        Vector3 finalMovement = Vector3.zero;
        Vector2 movementInput = InputManager.Instance.GetMovementVectorNormalized();
        OnPlayerMove?.Invoke(this, movementInput.magnitude);
        Transform cameraTransform = _camera.transform;
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
        if (!CheckForRaycastHit(out RaycastHit hitInfo, new [] {_stationMask, _handleableItemMask, _customerMask})) return;
        if (hitInfo.transform.TryGetComponent(out IInteractable interactableComponent))
        {
            interactableComponent.Interact();
            OnPlayerInteract?.Invoke(this, EventArgs.Empty);
        }
    }

    private void InputManager_OnInteractAlt(object sender, EventArgs e)
    {
        if (!CheckForRaycastHit(out RaycastHit hitInfo, new [] {_stationMask, _handleableItemMask, _customerMask})) return;
        if (hitInfo.transform.TryGetComponent(out IInteractableAlt interactableComponent))
        {
            interactableComponent.InteractAlt();
            OnPlayerInteractAlt?.Invoke(this, EventArgs.Empty);
        }
    }

    private bool CheckForRaycastHit(out RaycastHit hitInfo, int[] layers)
    {
        Transform cameraTransform = _camera.transform;
        Ray ray = _camera!.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        float playerDistanceFromCamera = Vector3.Distance(playerPositionForRaycast.position, cameraTransform.position);
        ray.origin += cameraTransform.forward * playerDistanceFromCamera;
        bool hit = Physics.Raycast(ray, out hitInfo, MaxInteractionDistance);
        if (!hit) return false;
        int targetLayer = 1 << hitInfo.transform.gameObject.layer;
        return Array.Exists(layers, layer => layer == targetLayer);
    }
}
