using Academy.HoloToolkit.Unity;
using UnityEngine;
using UnityEngine.VR.WSA;

[RequireComponent(typeof(AudioSource))]
public class EnergyHubBase : Singleton<EnergyHubBase>
{
    public AudioClip AnchorLanding;
    public Material PlacingMaterial;
    public Material PlacedMaterial;

    void Start()
    {
        OnSelect();
    }

    void Update()
    {
        if (IsTargetVisible())
        {
            HolographicSettings.SetFocusPointForFrame(gameObject.transform.position, -Camera.main.transform.forward);
        }
    }

    private bool IsTargetVisible()
    {
        // This will return true if the target's mesh is within the Main Camera's view frustums.
        Vector3 targetViewportPosition = Camera.main.WorldToViewportPoint(gameObject.transform.position);
        return (targetViewportPosition.x > 0.0 && targetViewportPosition.x < 1 &&
            targetViewportPosition.y > 0.0 && targetViewportPosition.y < 1 &&
            targetViewportPosition.z > 0);
    }

    void OnSelect()
    {
        ResetAnimation();
        foreach (Transform child in this.transform)
        {
            MaterialSwap(child, PlacingMaterial, PlacedMaterial);
            foreach (Transform childnested in child.transform)
            {
                MaterialSwap(childnested, PlacingMaterial, PlacedMaterial);
            }
        }
        this.transform.Find("BlobOutside").gameObject.GetComponent<Renderer>().enabled = true;
        Animator animator = GetComponent<Animator>();
        animator.speed = 1;

        GetComponent<AudioSource>().clip = AnchorLanding;
        GetComponent<AudioSource>().loop = false;
        GetComponent<AudioSource>().Play();
    }

    public void ResetAnimation()
    {
        Animator animator = GetComponent<Animator>();

        animator.Rebind();
        animator.speed = 0;

        // Setup Placing Object
        foreach (Transform child in this.transform)
        {
            MaterialSwap(child, PlacedMaterial, PlacingMaterial);
            foreach (Transform childnested in child.transform)
            {
                MaterialSwap(childnested, PlacedMaterial, PlacingMaterial);
            }
        }

        this.transform.Find("BlobOutside").gameObject.GetComponent<Renderer>().enabled = false;

        GetComponent<AudioSource>().Stop();
    }

    void LightShieldsOpen()
    {
    }

    void LandingDone()
    {
    }

    void MaterialSwap(Transform mesh, Material currentMaterial, Material newMaterial)
    {
        if (mesh.GetComponent<Renderer>() == true)
        {
            if (mesh.gameObject.GetComponent<Renderer>().sharedMaterial == currentMaterial)
            {
                mesh.gameObject.GetComponent<Renderer>().material = newMaterial;
            }
        }
    }
}