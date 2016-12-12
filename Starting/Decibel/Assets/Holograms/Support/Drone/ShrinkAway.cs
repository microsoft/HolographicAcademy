using UnityEngine;
using System.Collections;

public class ShrinkAway : MonoBehaviour
{
    Renderer[] renderers;

    void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();
    }

    void Update()
    {
        transform.localScale = transform.localScale - Vector3.one * 0.01f;
        if (transform.localScale.magnitude < 0.1f)
        {
            for (int index = 0; index < renderers.Length; index++)
            {
                renderers[index].enabled = false;
            }
            Destroy(this);
        }
    }
}