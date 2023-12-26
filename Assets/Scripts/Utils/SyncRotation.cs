using System;
using UnityEngine;

public class SyncRotation : MonoBehaviour
{
    public enum RotationMode
    {
        Copy,
        Around
    }
    
    [SerializeField] private Transform targetToSyncWith;
    [SerializeField] private RotationMode rotationMode = RotationMode.Around;
    [SerializeField] private bool syncXAxis = true;
    [SerializeField] private bool syncYAxis = true;
    [SerializeField] private bool syncZAxis = true;
    private void Update()
    {
        switch (rotationMode)
        {
            case RotationMode.Copy:
                CopyRotation();
                break;
            case RotationMode.Around:
                RotateAround();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void CopyRotation()
    {
        Vector3 targetRotation = targetToSyncWith.rotation.eulerAngles;
        Vector3 currentRotation = transform.rotation.eulerAngles;
        Vector3 newRotation = new Vector3(
            syncXAxis ? targetRotation.x : currentRotation.x,
            syncYAxis ? targetRotation.y : currentRotation.y,
            syncZAxis ? targetRotation.z : currentRotation.z
        );
        transform.rotation = Quaternion.Euler(newRotation);
    }
    
    private void RotateAround()
    {
        Vector3 targetRotation = targetToSyncWith.rotation.eulerAngles;
        Vector3 currentRotation = transform.rotation.eulerAngles;
        Vector3 newRotation = new Vector3(
            syncXAxis ? targetRotation.x : currentRotation.x,
            syncYAxis ? targetRotation.y : currentRotation.y,
            syncZAxis ? targetRotation.z : currentRotation.z
        );
        Vector3 targetPosition = targetToSyncWith.position;
        transform.RotateAround(targetPosition, Vector3.up, newRotation.y - currentRotation.y);
        transform.RotateAround(targetPosition, Vector3.left, newRotation.x - currentRotation.x);
        transform.RotateAround(targetPosition, Vector3.forward, newRotation.z - currentRotation.z);
    }
}
