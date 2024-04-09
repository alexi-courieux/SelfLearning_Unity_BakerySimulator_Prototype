using System;
using Cinemachine;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    private const int ActiveCameraPriority = 20;
    private const int InactiveCameraPriority = 10;
    
    [SerializeField] private float rotationSpeedH = 250f;
    [SerializeField] private float rotationSpeedV = 1.5f;
    [SerializeField] private CinemachineVirtualCameraBase thirdPersonCamera;
    [SerializeField] private CinemachineVirtualCameraBase firstPersonCamera;
    [SerializeField] private Transform cameraFollowTarget;
    private CinemachineVirtualCameraBase _activeCamera;
    private CinemachineVirtualCameraBase ActiveCamera
    {
        get => _activeCamera;
        set
        {
            _activeCamera.Priority = InactiveCameraPriority;
            _activeCamera = value;
            _activeCamera.Priority = ActiveCameraPriority;
        }
    }

    private void Start()
    {
        _activeCamera = GetCameraByPriority();
        InputManager.Instance.OnCameraSwitch += InputManager_OnCameraSwitch;
    }

    private void Update()
    {
        RotateCamera();
    }
    
    private CinemachineVirtualCameraBase GetCameraByPriority()
    {
        if (thirdPersonCamera.Priority > firstPersonCamera.Priority)
        {
            return thirdPersonCamera;
        }
        return firstPersonCamera;
    }

    private void InputManager_OnCameraSwitch(object sender, EventArgs e)
    {
        if (ActiveCamera == thirdPersonCamera)
        {
            ActiveCamera = firstPersonCamera;
        }
        else
        {
            ActiveCamera = thirdPersonCamera;
        }
    }

    private void RotateCamera()
    {
        Vector2 rotationVector = InputManager.Instance.GetRotationVectorNormalized();
        
        float rotationH = rotationVector.x * rotationSpeedH * Time.deltaTime;
        cameraFollowTarget.Rotate(Vector3.up, rotationH, Space.World);

        float rotationV = rotationVector.y * rotationSpeedV * Time.deltaTime;
        cameraFollowTarget.Rotate(Vector3.left, rotationV, Space.Self);
        
        // Clamp the vertical rotation
        Vector3 angles = cameraFollowTarget.localEulerAngles;
        angles.z = 0;
        float angle = angles.x;
        if (angle > 180)
        {
            angle -= 360;
        }
        angle = Mathf.Clamp(angle, -80, 80);
        angles.x = angle;
        cameraFollowTarget.localEulerAngles = angles;
    }
}
