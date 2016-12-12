using Academy.HoloToolkit.Unity;
using UnityEngine;

public class PolyStateManager : Singleton<PolyStateManager>
{
    public enum PolyStates
    {
        // Hovering in place.
        Idle = 0,

        // Charging in the station.
        Charging,

        // Returning to the location in front of the user.
        Returning,

        // Hiding behind the user.
        Hiding
    }

    public GameObject ChargingStation;
    public GameObject Poly;

    [Tooltip("The maximum distance, from the user, that P0ly moves when hiding.")]
    [Range(1.5f, 5.0f)]
    public float HideDistance = 1.5f;

    [Tooltip("The maximum distance that P0ly will look to find the floor.")]
    [Range(1.0f, 20.0f)]
    public float MaxRaycastDistance = 3.0f;

    /// <summary>
    /// The point towards which P0ly is moving.
    /// </summary>
    public Vector3 Destination { get; private set; }

    /// <summary>
    /// P0ly's current position.
    /// </summary>
    public Vector3 Position
    {
        get
        {
            return Poly.transform.position;
        }
    }

    /// <summary>
    /// P0ly's current state (charging, etc).
    /// </summary>
    public PolyStates State 
    { get; private set; }

    private void Awake()
    {
        GoIdle();
    }

    /// <summary>
    /// Instruct P0ly to enter the idle state.
    /// </summary>
    private void GoIdle()
    {
        Destination = Position;
        State = PolyStates.Idle;
    }

    /// <summary>
    /// Instruct P0ly to enter the charging state.
    /// </summary>
    private void GoCharge()
    {
        Vector3 polyHalfExtents = Poly.GetComponentInChildren<Collider>().bounds.extents / 2f;

        Destination = ChargingStation.GetComponent<Collider>().bounds.center -
            new Vector3(polyHalfExtents.x / 2.5f, polyHalfExtents.y * 2.5f, polyHalfExtents.z / 4f);

        State = PolyStates.Charging;
    }

    /// <summary>
    /// Instruct P0ly to enter the hiding state.
    /// </summary>
    private void GoHide()
    {
        Transform userTransform = Camera.main.transform;
        Vector3 destination = userTransform.position + (-userTransform.forward * HideDistance);
        destination += (userTransform.right * Random.Range(-HideDistance / 2, HideDistance / 2));
        Destination = destination;
        State = PolyStates.Hiding;
    }

    /// <summary>
    /// Instruct P0ly to enter the returning state.
    /// </summary>
    private void ReturnToUser()
    {
        Destination = Camera.main.transform.position + ((Camera.main.transform.forward) * 1.5f);
        State = PolyStates.Returning;
    }

    /// <summary>
    /// Places P0ly into the requested state.
    /// </summary>
    public void SetState(PolyStates state)
    {
        switch (state)
        {
            case PolyStates.Charging:
                GoCharge();
                break;

            case PolyStates.Hiding:
                GoHide();
                break;
            
            case PolyStates.Returning:
                ReturnToUser();
                break;

            default:
                GoIdle();
                break;
        }
    }
}
