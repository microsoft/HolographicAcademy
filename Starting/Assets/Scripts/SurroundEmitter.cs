using HoloToolkit.Unity;
using UnityEngine;

public class SurroundEmitter : MonoBehaviour
{
    public Material defaultMaterial;
    public Material disabledMaterial;

    [SerializeField]
    private AudioSource emitter;
    [SerializeField]
    private string eventName;
    [SerializeField]
    private Light toggleLight;

    private Renderer meshRenderer;

    void Start()
    {
        meshRenderer = gameObject.GetComponent<Renderer>();
        defaultMaterial = meshRenderer.material;
    }

    public void PlayEvent()
    {
        UAudioManager.Instance.PlayEvent(this.eventName, this.emitter);
    }

    private void OnSelected()
    {
        if (this.emitter.mute)
        {
            this.emitter.mute = false;
            this.toggleLight.enabled = true;
            meshRenderer.material = defaultMaterial;
        }
        else
        {
            this.emitter.mute = true;
            this.toggleLight.enabled = false;
            meshRenderer.material = disabledMaterial;
        }
    }
}