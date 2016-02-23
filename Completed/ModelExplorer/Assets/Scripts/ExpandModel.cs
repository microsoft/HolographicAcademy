using HoloToolkit;
using UnityEngine;

/// <summary>
/// Placeholder script for exploding the model.
/// </summary>
public class ExpandModel : Singleton<ExpandModel>
{
    // We are using a different model for the expanded view.  Set it here so we can swap it out when we expand.
    [Tooltip("Game object for the exploded model.")]
    public GameObject ExpandedModel;

    [Tooltip("Audio clip to play when expanding the model.")]
    public AudioClip ExpandModelSound;
    private AudioSource audioSource;
    private GameObject audioGameObject;

    public bool IsModelExpanded { get; private set; }

    void Awake()
    {
        IsModelExpanded = false;
    }

    public void Expand()
    {
        EnableAudioHapticFeedback();

        if (audioGameObject != null)
        {
            Destroy(audioGameObject, audioSource.clip.length);
        }

        IsModelExpanded = true;
    }

    public void Reset()
    {
        IsModelExpanded = false;
    }

    private void EnableAudioHapticFeedback()
    {
        // If this hologram has an audio clip, add an AudioSource with this clip.
        if (ExpandModelSound != null)
        {
            audioGameObject = new GameObject();
            audioGameObject.transform.position = gameObject.transform.position;

            audioSource = audioGameObject.GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = audioGameObject.AddComponent<AudioSource>();
            }

            audioSource.clip = ExpandModelSound;
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1;
            audioSource.dopplerLevel = 0;

            audioSource.Play();
        }
    }
}