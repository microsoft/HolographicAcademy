using UnityEngine;
using System.Collections;

public class GrenadeRotate : MonoBehaviour
{

    public Color EmissiveColor = new Vector4(0f, .87f, 1f, 1f);

    void Start()
    {
        var materials = GetComponent<Renderer>().materials;
        materials[1].SetColor("_EmissionColor", EmissiveColor);
        GameObject childParticle = transform.Find("ParticleTrail").gameObject;
        if (childParticle != null)
        {
            //Find the child named "ammo" of the gameobject "magazine" (magazine is a child of "gun").
            childParticle.GetComponent<ParticleSystem>().startColor = EmissiveColor;
        }
    }
}
