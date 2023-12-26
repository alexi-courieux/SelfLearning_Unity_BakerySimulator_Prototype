using System;
using Cinemachine;
using UnityEngine;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private float rotationSpeedH;
    [SerializeField] private float rotationSpeedV;
    [SerializeField] private float characterRotationSpeed;
    
    private CharacterController _characterController;
    private CinemachineFreeLook _cameraFreelook;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _cameraFreelook = FindObjectOfType<CinemachineFreeLook>();
    }

    private void Update()
    {
        Rotate();
        SearchForInteraction();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        Vector2 movementInput = InputManager.Instance.GetMovementVectorNormalized();
        if(movementInput.magnitude < 0.1f) return;
        
        // Translate towards the camera direction
        Transform cameraTransform = Camera.main!.transform;
        Vector3 moveDirection = (cameraTransform.forward * movementInput.y) + (cameraTransform.right * movementInput.x);
        moveDirection.y = 0;
        _characterController.Move(moveDirection * (movementSpeed * Time.fixedDeltaTime));
        
        // Rotate towards the camera direction
        Quaternion targetRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(moveDirection, Vector3.up), Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, characterRotationSpeed * Time.fixedDeltaTime);
    }

    private void Rotate()
    {
        Vector2 rotationVector = InputManager.Instance.GetRotationVectorNormalized();
        _cameraFreelook.m_XAxis.Value += rotationVector.x * rotationSpeedH * Time.deltaTime;
        _cameraFreelook.m_YAxis.Value += -rotationVector.y * rotationSpeedV * Time.deltaTime;
    }
    
    private void SearchForInteraction()
    {
        
    }
}
