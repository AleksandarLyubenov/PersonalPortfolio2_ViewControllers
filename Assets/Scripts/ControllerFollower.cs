using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerDriver : MonoBehaviour
{
    [Header("Controller Tracking Input Actions")]
    public InputActionProperty controllerPosition;
    public InputActionProperty controllerRotation;

    [Header("Tracking Origin")]

    public Transform trackingOrigin;

    [Header("Optional Local Offset")]
    public Vector3 localOffset;

    private void OnEnable()
    {
        if (controllerPosition != null && controllerPosition.action != null)
            controllerPosition.action.Enable();

        if (controllerRotation != null && controllerRotation.action != null)
            controllerRotation.action.Enable();
    }

    private void OnDisable()
    {
        if (controllerPosition != null && controllerPosition.action != null)
            controllerPosition.action.Disable();

        if (controllerRotation != null && controllerRotation.action != null)
            controllerRotation.action.Disable();
    }

    private void Update()
    {
        // Read the tracked world-space position and rotation from the Input Actions.
        Vector3 worldPos = controllerPosition.action.ReadValue<Vector3>();
        Quaternion worldRot = controllerRotation.action.ReadValue<Quaternion>();

        if (trackingOrigin != null)
        {
            // Convert the world-space position into local space relative to the trackingOrigin.
            Vector3 localPos = trackingOrigin.InverseTransformPoint(worldPos);
            localPos += localOffset;
            transform.localPosition = localPos;

            Quaternion localRot = Quaternion.Inverse(trackingOrigin.rotation) * worldRot;
            transform.localRotation = localRot;
        }
        else
        {
            transform.SetPositionAndRotation(worldPos + localOffset, worldRot);
        }
    }
}
