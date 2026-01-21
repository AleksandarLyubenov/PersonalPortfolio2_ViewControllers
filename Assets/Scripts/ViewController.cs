using UnityEngine;
using UnityEngine.InputSystem;

public enum ViewMode
{
    Smooth,
    Snap,
    FOV
}

public class ViewController : MonoBehaviour
{
    [Header("General View Mode Settings")]
    public ViewMode currentViewMode = ViewMode.Smooth;

    [Header("Common Input Settings")]
    public InputActionProperty rotationAction;

    [Header("Smooth View Settings")]
    public float smoothRotationSpeed = 90f;

    [Header("Snap View Settings")]
    public float snapAngle = 45f;
    public float snapThreshold = 0.8f;
    private bool snapTurnReady = true;

    [Header("FOV View Settings")]
    public Transform headTransform;
    public float fovThreshold = 30f;
    public float fovRecenterSpeed = 5f;

    #region Input Enabling
    private void OnEnable()
    {
        if (rotationAction != null && rotationAction.action != null)
            rotationAction.action.Enable();
    }

    private void OnDisable()
    {
        if (rotationAction != null && rotationAction.action != null)
            rotationAction.action.Disable();
    }
    #endregion

    private void Update()
    {
        switch (currentViewMode)
        {
            case ViewMode.Smooth:
                HandleSmoothView();
                break;
            case ViewMode.Snap:
                HandleSnapView();
                break;
            case ViewMode.FOV:
                HandleFOVView();
                break;
        }
    }

    /// <summary>
    /// Smoothly rotates the rig based on thumbstick input.
    /// </summary>
    private void HandleSmoothView()
    {
        Vector2 rotationInput = rotationAction.action.ReadValue<Vector2>();
        float horizontalRotation = rotationInput.x * smoothRotationSpeed * Time.deltaTime;
        transform.Rotate(0f, horizontalRotation, 0f);
    }

    /// <summary>
    /// Instantly snaps the rig’s rotation by a fixed angle when the thumbstick is pushed past a threshold.
    /// </summary>
    private void HandleSnapView()
    {
        Vector2 rotationInput = rotationAction.action.ReadValue<Vector2>();

        if (Mathf.Abs(rotationInput.x) >= snapThreshold && snapTurnReady)
        {
            float direction = Mathf.Sign(rotationInput.x);
            transform.Rotate(0f, direction * snapAngle, 0f);
            snapTurnReady = false;
        }
        else if (Mathf.Abs(rotationInput.x) < snapThreshold)
        {
            snapTurnReady = true;
        }
    }

    /// <summary>
    /// In FOV view, if the headset’s yaw deviates too far from the rig’s forward direction,
    /// the rig smoothly rotates to recenter the view.
    /// </summary>
    private void HandleFOVView()
    {
        if (headTransform == null)
        {
            if (Camera.main != null)
                headTransform = Camera.main.transform;
            else
            {
                Debug.LogError("No headTransform assigned and no Main Camera found.");
                return;
            }
        }

        Vector3 rigForward = transform.forward;
        Vector3 headForward = headTransform.forward;
        rigForward.y = 0f;
        headForward.y = 0f;
        rigForward.Normalize();
        headForward.Normalize();

        float angleDifference = Vector3.SignedAngle(rigForward, headForward, Vector3.up);

        if (Mathf.Abs(angleDifference) > fovThreshold)
        {
            float recenterAmount = Mathf.Lerp(0f, angleDifference, Time.deltaTime * fovRecenterSpeed);
            transform.Rotate(0f, recenterAmount, 0f);
        }
    }

    #region Public Accessors
    /// <summary>
    /// Set the current view mode.
    /// </summary>
    public void SetViewMode(ViewMode mode)
    {
        currentViewMode = mode;
        Debug.Log("View mode changed to: " + currentViewMode);
    }

    /// <summary>
    /// Get the current view mode.
    /// </summary>
    public ViewMode GetViewMode()
    {
        return currentViewMode;
    }
    #endregion
}
