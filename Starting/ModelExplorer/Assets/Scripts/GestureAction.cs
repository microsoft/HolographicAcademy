using Academy.HoloToolkit.Unity;
using UnityEngine;

/// <summary>
/// GestureAction performs custom actions based on 
/// which gesture is being performed.
/// </summary>
public class GestureAction : MonoBehaviour
{
    [Tooltip("Rotation max speed controls amount of rotation.")]
    public float RotationSensitivity = 10.0f;

    private Vector3 manipulationPreviousPosition;

    private float rotationFactor;

    void Update()
    {
        PerformRotation();
    }

    private void PerformRotation()
    {
        if (GestureManager.Instance.IsNavigating &&
            (!ExpandModel.Instance.IsModelExpanded ||
            (ExpandModel.Instance.IsModelExpanded && HandsManager.Instance.FocusedGameObject == gameObject)))
        {
            // This will help control the amount of rotation.
            rotationFactor = GestureManager.Instance.NavigationPosition.x * RotationSensitivity;

            transform.Rotate(new Vector3(0, -1 * rotationFactor, 0));
        }
    }

    void PerformManipulationStart(Vector3 position)
    {
        manipulationPreviousPosition = position;
    }

    void PerformManipulationUpdate(Vector3 position)
    {
        if (GestureManager.Instance.IsManipulating)
        {
            Vector3 moveVector = Vector3.zero;
            moveVector = position - manipulationPreviousPosition;
            manipulationPreviousPosition = position;

            transform.position += moveVector;
        }
    }
}