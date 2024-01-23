using System;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private enum Mode
    {
        LookAt,
        LookAtInverted,
        CameraFacing,
        CameraFacingInverted,
    }
    
    [SerializeField] private Mode mode = Mode.LookAt;

    private void LateUpdate()
    {
        switch (mode)
        {
            case Mode.LookAt:
                transform.LookAt(Camera.main.transform);
                break;
            case Mode.LookAtInverted:
                var dirFromCamera = transform.position - Camera.main.transform.position;
                transform.LookAt(transform.position + dirFromCamera);
                break;
            case Mode.CameraFacing:
                transform.forward = Camera.main.transform.forward;
                break;
            case Mode.CameraFacingInverted:
                transform.forward = -Camera.main.transform.forward;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
