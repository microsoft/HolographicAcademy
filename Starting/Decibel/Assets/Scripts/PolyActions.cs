using Academy.HoloToolkit.Unity;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Interpolator))]
public class PolyActions : MonoBehaviour
{
    [Tooltip("The speed at which POLY is to move.")]
    [Range(5.0f, 60.0f)]
    public float MoveSpeed = 60.0f;

    [Tooltip("How rapidly should POLY rotate.")]
    [Range(2.0f, 100.0f)]
    public float RotationSensitivity = 10.0f;

    public GameObject Teleporticles;

    private Interpolator interpolator;

    private void Awake()
    {
        interpolator = gameObject.GetComponent<Interpolator>();

        interpolator.PositionPerSecond = MoveSpeed;
        interpolator.SmoothLerpToTarget = true;

        interpolator.InterpolationDone += Interpolator_InterpolationDone;
    }

    private void Interpolator_InterpolationDone()
    {
        if (PolyStateManager.Instance.State != PolyStateManager.PolyStates.Charging)
        {
            transform.LookAt(Camera.main.transform.position);
        }

        PolyStateManager.Instance.SetState(PolyStateManager.PolyStates.Idle);
    }

    private void Start()
    {
        PolyStateManager.Instance.SetState(PolyStateManager.PolyStates.Idle);
    }

    /// <summary>
    /// Calls P0ly to the point at which the user is gazing.
    /// </summary>
    public void ComeBack()
    {
        PolyStateManager.Instance.SetState(PolyStateManager.PolyStates.Returning);
        
        interpolator.SetTargetPosition(PolyStateManager.Instance.Destination);
    }

    /// <summary>
    /// Sends P0ly to the charging box.
    /// </summary>
    public void GoCharge()
    {
        if (PolyStateManager.Instance.State == PolyStateManager.PolyStates.Charging) { return; }

        PolyStateManager.Instance.SetState(PolyStateManager.PolyStates.Charging);
        
        interpolator.SetTargetPosition(PolyStateManager.Instance.Destination);
    }

    /// <summary>
    /// Called when the Select Gesture is detected. Instructs P0ly to hide.
    /// </summary>
    public void OnSelect()
    {
        if (PolyStateManager.Instance.State == PolyStateManager.PolyStates.Hiding) { return; }

        // Hide P0ly.  It will be shown again when idle.
        PolyStateManager.Instance.SetState(PolyStateManager.PolyStates.Hiding);
        
        interpolator.PositionPerSecond = MoveSpeed;
        interpolator.SetTargetPosition(PolyStateManager.Instance.Destination);
    }
}
