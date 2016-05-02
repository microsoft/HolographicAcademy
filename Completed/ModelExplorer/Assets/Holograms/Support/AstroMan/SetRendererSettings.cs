using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;

public class SetRendererSettings : MonoBehaviour
{
    void Start()
    {
        // Get all the SkinnedMeshRenderer components and turn off 'Light Probes' and 'Reflection Probes' as we don't use them.
        foreach (SkinnedMeshRenderer skm in gameObject.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            skm.lightProbeUsage = LightProbeUsage.Off;
            skm.reflectionProbeUsage = ReflectionProbeUsage.Off;
            skm.shadowCastingMode = ShadowCastingMode.On;
            skm.receiveShadows = false;
        }

        // Get all the MeshRenderer components and turn off 'Light Probes' and 'Reflection Probes' as we don't use them.
        foreach (MeshRenderer mr in GetComponentsInChildren<MeshRenderer>())
        {
            mr.lightProbeUsage = LightProbeUsage.Off;
            mr.reflectionProbeUsage = ReflectionProbeUsage.Off;
            mr.shadowCastingMode = ShadowCastingMode.Off;
            mr.receiveShadows = false;
        }
    }
}