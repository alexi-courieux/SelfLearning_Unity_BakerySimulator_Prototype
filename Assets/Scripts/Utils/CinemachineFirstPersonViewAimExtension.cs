using Cinemachine;

public class CinemachineFirstPersonViewAimExtension : CinemachineExtension
{
    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (stage == CinemachineCore.Stage.Aim)
        {
            state.RawOrientation = vcam.Follow.rotation;
        }
    }
}
