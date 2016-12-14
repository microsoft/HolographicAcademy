using Academy.HoloToolkit.Unity;
using UnityEngine;

public class PolyChargerRotation : MonoBehaviour
{
    [Tooltip("How rapidly should the charger rotate.")]
    [Range(2.0f, 100.0f)]
    public float RotationSensitivity = 10.0f;

    private void PerformRotation()
    {
        // This will help control the amount of rotation.
        float rotationFactor = GestureManager.Instance.NavigationPosition.x * RotationSensitivity;

        // Rotate along the Y axis using rotationFactor.
        transform.parent.Rotate(new Vector3(0, -1 * rotationFactor, 0));
    }

    /// <summary>
    /// Handles navigation start messages.
    /// </summary>
    public void OnNavigationStarted()
    {
        PerformRotation();
    }

    /// <summary>
    /// Handles navigation update messages.
    /// </summary>
    public void OnNavigationUpdated()
    {
        PerformRotation();
    }
}