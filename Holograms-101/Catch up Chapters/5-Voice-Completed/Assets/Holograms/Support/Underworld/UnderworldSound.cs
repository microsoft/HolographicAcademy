using UnityEngine;

public class UnderworldSound : MonoBehaviour
{

    void OnEnable()
    {
        AudioSource source = GetComponent<AudioSource>();
        if (source)
        {
            source.Play();
        }
    }
}
