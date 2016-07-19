using UnityEngine;
using UnityEngine.VR.WSA;

public class SpatialMapping : MonoBehaviour {

    public static SpatialMapping Instance { private set; get; }

    [HideInInspector]
    public static int PhysicsRaycastMask;

    [Tooltip("The material to use when rendering Spatial Mapping data.")]
    public Material DrawMaterial;

    [Tooltip("If true, the Spatial Mapping data will be rendered.")]
    public bool drawVisualMeshes = false;
    
    // If true, Spatial Mapping will be enabled. 
    private bool mappingEnabled = true;

    // The layer to use for spatial mapping collisions.
    private int physicsLayer = 31;

    // Handles rendering of spatial mapping meshes.
    private SpatialMappingRenderer spatialMappingRenderer;

    // Creates/updates environment colliders to work with physics.
    private SpatialMappingCollider spatialMappingCollider;

    /// <summary>
    /// Determines if the spatial mapping meshes should be rendered.
    /// </summary>
    public bool DrawVisualMeshes
    {
        get
        {
            return drawVisualMeshes;
        }
        set
        {
            drawVisualMeshes = value;

            if (drawVisualMeshes)
            {
                spatialMappingRenderer.currentRenderSetting = SpatialMappingRenderer.RenderSetting.CustomMaterial;
                spatialMappingRenderer.customMaterial = DrawMaterial;
            }
            else
            {
                spatialMappingRenderer.currentRenderSetting = SpatialMappingRenderer.RenderSetting.None;
            }
        }
    }
 
    /// <summary>
    /// Enables/disables spatial mapping rendering and collision.
    /// </summary>
    public bool MappingEnabled
    {
        get
        {
            return mappingEnabled;
        }
        set
        {
            mappingEnabled = value;
            spatialMappingCollider.freezeUpdates = !mappingEnabled;
            spatialMappingRenderer.freezeUpdates = !mappingEnabled;
            gameObject.SetActive(mappingEnabled);
        }
    }

    void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start()
    {
        spatialMappingRenderer = gameObject.GetComponent<SpatialMappingRenderer>();
        spatialMappingRenderer.surfaceParent = this.gameObject;
        spatialMappingCollider = gameObject.GetComponent<SpatialMappingCollider>();
        spatialMappingCollider.surfaceParent = this.gameObject;
        spatialMappingCollider.layer = physicsLayer;
        PhysicsRaycastMask = 1 << physicsLayer;
        DrawVisualMeshes = drawVisualMeshes;
        MappingEnabled = mappingEnabled;
    }
}