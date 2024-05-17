using System;
using UnityEngine;

namespace Utils
{
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
                    transform.LookAt(Camera.main!.transform);
                    break;
                case Mode.LookAtInverted:
                    Vector3 dirFromCamera = transform.position - Camera.main!.transform.position;
                    transform.LookAt(transform.position + dirFromCamera);
                    break;
                case Mode.CameraFacing:
                    transform.forward = Camera.main!.transform.forward;
                    break;
                case Mode.CameraFacingInverted:
                    transform.forward = -Camera.main!.transform.forward;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
