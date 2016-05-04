using HoloToolkit.Unity;
using UnityEngine;
using UnityEngine.Audio;

public class SurroundController : MonoBehaviour
{
    public Material standardMaterial;
    public Material spatialMaterial;

    [SerializeField]
    private AudioSource twoDMusic;
    [SerializeField]
    private SurroundEmitter[] emitters;
    [SerializeField]
    private AudioMixer mixer;

    public void StartEmitters()
    {
        UAudioManager.Instance.PlayEvent("StereoMusic", this.twoDMusic);
        for (int i = 0; i < this.emitters.Length; i++)
        {
            SurroundEmitter em = this.emitters[i];
            em.PlayEvent();
        }
        DoStandard();
    }

    public void DoSpatial()
    {
        AudioMixerSnapshot[] snap = { this.mixer.FindSnapshot("SpatialMusic") };
        float[] weight = { 1 };
        this.mixer.TransitionToSnapshots(snap, weight, 1);
        SetMaterials(spatialMaterial);
    }

    public void DoStandard()
    {
        AudioMixerSnapshot[] snap = { this.mixer.FindSnapshot("StereoMusic") };
        float[] weight = { 1 };
        this.mixer.TransitionToSnapshots(snap, weight, 1);
        SetMaterials(standardMaterial);
    }

    private void SetMaterials(Material material)
    {
        SurroundEmitter[] emitters = gameObject.GetComponentsInChildren<SurroundEmitter>();

        foreach (SurroundEmitter emitter in emitters)
        {
            emitter.defaultMaterial = material;

            if (!emitter.GetComponent<AudioSource>().mute)
            {
                Renderer meshRenderer = emitter.gameObject.GetComponent<Renderer>();
                meshRenderer.material = material;
            }
        }
    }
}