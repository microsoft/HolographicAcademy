using UnityEngine;

/// <summary>
/// Destroys the gameobject when it receives the OnSelect message.
/// </summary>
public class DismissOnSelect : MonoBehaviour
{
    [Tooltip("Audio clip to play when interacting with this hologram.")]
    public AudioClip TargetFeedbackSound;
    private AudioSource audioSource;
    private GameObject audioGameObject;

    void OnSelect()
    {
        EnableAudioHapticFeedback();

        Destroy(this.gameObject);
        if (audioGameObject != null)
        {
            Destroy(audioGameObject, audioSource.clip.length);
        }
    }

    private void EnableAudioHapticFeedback()
    {
        // If this hologram has an audio clip, add an AudioSource with this clip.
        if (TargetFeedbackSound != null)
        {
            audioGameObject = new GameObject();
            audioGameObject.transform.position = gameObject.transform.position;

            audioSource = audioGameObject.GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = audioGameObject.AddComponent<AudioSource>();
            }

            audioSource.clip = TargetFeedbackSound;
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1;
            audioSource.dopplerLevel = 0;

            audioSource.Play();
        }
    }
}