using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR.WSA;

public class SpatialMapping : MonoBehaviour
{
    public static SpatialMapping Instance { private set; get; }
    public static int PhysicsRaycastMask;

    [Tooltip("The material to use when rendering Spatial Mapping data.")]
    public Material DrawMaterial;
    [Tooltip("If true, the Spatial Mapping data will be rendered.")]
    public bool DrawVisualMeshes = false;

    // The frequency (in seconds) which to call Update() on our SMSurfaceObserver
    private float UpdateFrequency = 2.5f;
    // The physics layer where Spatial Mapping data will live.
    private int PhysicsLayer = 31;
    // The number of Triangles per m^3 for the SMSurfaceObserver to calculate.
    private float TrianglesPerCubicMeter = 500f;
    // The volume of space the SMSurfaceObserver should observe.
    private Vector3 BoundingVolume = Vector3.one * 10.0f;
    // Our Spatial Mapping Surface Observer object
    private SurfaceObserver Observer;
    // A dictionary of surfaces (Meshes) that our Surface Observer knows about.
    private Dictionary<int, GameObject> surfaces;
    // Queue of SurfaceData information. Enqueued in OnSurfaceChanged; Dequeued in Update.
    private Queue<SurfaceData> surfaceDataQueue;
    // If true, Spatial Mapping is enabled. 
    private bool mappingEnabled = true;

    /// <summary>
    /// To prevent too many meshes from being generated at the same time, we will
    /// only request one mesh to be created at a time.  This variable will track
    /// if a mesh creation request is in flight.
    /// </summary>
    private bool surfaceWorkOutstanding = false;

    void Awake()
    {
        Instance = this;
        // Convert the layer into a mask so it can be used to Raycast against.
        PhysicsRaycastMask = 1 << PhysicsLayer;
    }

    // Use this for initialization
    void Start()
    {
        Observer = new SurfaceObserver();
        Observer.SetVolumeAsAxisAlignedBox(Vector3.zero, BoundingVolume);
        surfaces = new Dictionary<int, GameObject>();
        surfaceDataQueue = new Queue<SurfaceData>();
        UpdateSurfaceObserver();
    }

    private void UpdateSurfaceObserver()
    {
        if (mappingEnabled)
        {
            Observer.Update(Observer_OnSurfaceChanged);
            Invoke("UpdateSurfaceObserver", UpdateFrequency);
        }
    }

    private void Observer_OnSurfaceChanged(SurfaceId surfaceId, SurfaceChange changeType, Bounds bounds, System.DateTime updateTime)
    {
        GameObject surface;

        switch (changeType)
        {
            case SurfaceChange.Updated:
            case SurfaceChange.Added:
                if (!surfaces.TryGetValue(surfaceId.handle, out surface))
                {
                    // If we are adding a new surface, construct a GameObject
                    // to represent its state and attach some Mesh-related
                    // components to it.
                    surface = new GameObject(string.Format("Surface-{0}", surfaceId));
                    surface.AddComponent<MeshFilter>();
                    surface.AddComponent<MeshRenderer>().sharedMaterial = DrawMaterial;
                    surface.AddComponent<MeshCollider>();
                    surface.AddComponent<WorldAnchor>();
                    // Set the layer that this SpatialMapping surface is a part of
                    surface.layer = PhysicsLayer;
                    // Add the surface to our dictionary of known surfaces so
                    // we can interact with it later.
                    surfaces[surfaceId.handle] = surface;
                }

                SurfaceData smsd = new SurfaceData(
                    surfaceId,
                    surface.GetComponent<MeshFilter>(),
                    surface.GetComponent<WorldAnchor>(),
                    surface.GetComponent<MeshCollider>(),
                    TrianglesPerCubicMeter,
                    true);
                surfaceDataQueue.Enqueue(smsd);
                break;

            case SurfaceChange.Removed:
                if (surfaces.TryGetValue(surfaceId.handle, out surface))
                {
                    surfaces.Remove(surfaceId.handle);
                    Destroy(surface);
                }
                break;
        }
    }

    void Update()
    {
        if (mappingEnabled)
        {
            foreach (GameObject surface in surfaces.Values)
            {
                surface.GetComponent<MeshRenderer>().enabled = DrawVisualMeshes;
            }

            if (surfaceWorkOutstanding == false && surfaceDataQueue.Count > 0)
            {
                SurfaceData smsd = surfaceDataQueue.Dequeue();
                surfaceWorkOutstanding = Observer.RequestMeshAsync(smsd, Observer_OnDataReady);
            }
        }
    }

    /// <summary>
    /// Handles the SurfaceObserver's OnDataReady event.
    /// </summary>
    /// <param name="cookedData">Struct containing output data.</param>
    /// <param name="outputWritten">Set to true if output has been written.</param>
    /// <param name="elapsedCookTimeSeconds">Seconds between mesh cook request and propagation of this event.</param>
    private void Observer_OnDataReady(SurfaceData bakedData, bool outputWritten, float elapsedBakeTimeSeconds)
    {
        surfaceWorkOutstanding = false;
    }

    public void SetMappingEnabled(bool isEnabled)
    {
        mappingEnabled = isEnabled;
        foreach (GameObject surface in surfaces.Values)
        {
            surface.SetActive(isEnabled);
        }
        UpdateSurfaceObserver();
    }
}