using Academy.HoloToolkit.Unity;
using System.Collections.Generic;
using UnityEngine;

public class UnderworldBase : Singleton<UnderworldBase>
{
    public List<GameObject> ObjectsToHide = new List<GameObject>();

    // Use this for initialization.
    void Start()
    {
        if (GazeManager.Instance)
        {
            // Exclude the NavPath layer (used for UnderDrone navigation) from GazeManager raycasts.
            var navLayer = LayerMask.NameToLayer("NavPath");
            var ignoreNavLayerMask = ~(1 << navLayer);
            GazeManager.Instance.RaycastLayerMask = GazeManager.Instance.RaycastLayerMask & ignoreNavLayerMask;
        }
    }

    // Called every frame.
    void Update()
    {
    }

    // Called whenever the underworld is enabled.
    public void OnEnable()
    {
        if (!gameObject.activeSelf)
        {
            // Place the underworld on the surface mesh.
            PlaceUnderworld();
        }
    }

    // Called whenever the underworld is disabled.
    public void OnDisable()
    {
        ResetUnderworld();
    }
    
    /// <summary>
    /// Places the underworld at the user's gaze and makles it visible.
    /// </summary>
    private void PlaceUnderworld()
    {
        RaycastHit hitInfo;

        bool hit = Physics.Raycast(Camera.main.transform.position,
                                Camera.main.transform.forward,
                                out hitInfo,
                                20f,
                                SpatialMappingManager.Instance.LayerMask);

        if (hit)
        {
            // Disable the objects that should be hidden when the underworld is displayed.
            foreach (GameObject go in ObjectsToHide)
            {
                go.SetActive(false);
            }

            // Place and enable the underworld.
            gameObject.transform.position = hitInfo.point;
            gameObject.transform.up = hitInfo.normal;
            gameObject.SetActive(true);

            // Turn off spatial mapping meshes.
            SpatialMappingManager.Instance.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Hides the underworld.
    /// </summary>
    private void ResetUnderworld()
    {
        // Unhide the previously hidden objects.
        foreach (GameObject go in ObjectsToHide)
        {
            go.SetActive(true);
        }

        // Disable the underworld.
        gameObject.SetActive(false);

        // Turn spatial mapping meshes back on.
        SpatialMappingManager.Instance.gameObject.SetActive(true);
    }

    /// <summary>
    /// Checks to see if the target's mesh is visible within the Main Camera's view frustum.
    /// </summary>
    /// <returns>True, if the target's mesh is visible.</returns>
    bool IsTargetVisible()
    {
        Vector3 targetViewportPosition = Camera.main.WorldToViewportPoint(gameObject.transform.position);
        return (targetViewportPosition.x > 0.0 && targetViewportPosition.x < 1 &&
            targetViewportPosition.y > 0.0 && targetViewportPosition.y < 1 &&
            targetViewportPosition.z > 0);
    }
}