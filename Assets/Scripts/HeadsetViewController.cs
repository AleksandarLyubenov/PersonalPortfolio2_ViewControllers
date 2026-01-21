using UnityEngine;
using UnityEngine.InputSystem;

public class HeadsetViewController : MonoBehaviour
{
    [Header("Headset Rotation Input")]
    public InputActionProperty headRotationAction;

    private void OnEnable()
    {
        if (headRotationAction != null && headRotationAction.action != null)
        {
            headRotationAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (headRotationAction != null && headRotationAction.action != null)
        {
            headRotationAction.action.Disable();
        }
    }

    private void Update()
    {
        Quaternion headRotation = headRotationAction.action.ReadValue<Quaternion>();
        // Debug.Log("Head Rotation: " + headRotation);

        transform.localRotation = headRotation;
    }
}
