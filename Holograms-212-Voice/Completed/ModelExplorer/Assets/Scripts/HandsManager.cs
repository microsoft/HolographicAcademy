using HoloToolkit;
using UnityEngine.VR.WSA.Input;
using UnityEngine;

/// <summary>
/// HandsManager keeps track of when a hand is detected.
/// </summary>
public class HandsManager : Singleton<HandsManager>
{
    [Tooltip("Audio clip to play when Finger Pressed.")]
    public AudioClip FingerPressedSound;
    private AudioSource audioSource;

    /// <summary>
    /// Tracks the hand detected state.
    /// </summary>
    public bool HandDetected
    {
        get;
        private set;
    }

    // Keeps track of the GameObject that the hand is interacting with.
    public GameObject FocusedGameObject { get; private set; }

    void Awake()
    {
        EnableAudioHapticFeedback();

        SourceManager.SourceDetected += SourceManager_SourceDetected;
        SourceManager.SourceLost += SourceManager_SourceLost;

        SourceManager.SourcePressed += SourceManager_SourcePressed;
        SourceManager.SourceReleased += SourceManager_SourceReleased;

        FocusedGameObject = null;
    }

    private void EnableAudioHapticFeedback()
    {
        // If this hologram has an audio clip, add an AudioSource with this clip.
        if (FingerPressedSound != null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }

            audioSource.clip = FingerPressedSound;
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1;
            audioSource.dopplerLevel = 0;
        }
    }

    private void SourceManager_SourceDetected(SourceState hand)
    {
        HandDetected = true;
    }

    private void SourceManager_SourceLost(SourceState hand)
    {
        HandDetected = false;

        ResetFocusedGameObject();
    }

    private void SourceManager_SourcePressed(SourceState hand)
    {
        if (InteractibleManager.Instance.FocusedGameObject != null)
        {
            // Play a select sound if we have an audio source and are not targetting an asset with a select sound.
            if (audioSource != null && !audioSource.isPlaying &&
                (InteractibleManager.Instance.FocusedGameObject.GetComponent<Interactible>() != null &&
                InteractibleManager.Instance.FocusedGameObject.GetComponent<Interactible>().TargetFeedbackSound == null))
            {
                audioSource.Play();
            }

            FocusedGameObject = InteractibleManager.Instance.FocusedGameObject;
        }
    }

    private void SourceManager_SourceReleased(SourceState hand)
    {
        ResetFocusedGameObject();
    }

    private void ResetFocusedGameObject()
    {
        FocusedGameObject = null;

        GestureManager.Instance.ResetGestureRecognizers();
    }

    void OnDestroy()
    {
        SourceManager.SourceDetected -= SourceManager_SourceDetected;
        SourceManager.SourceLost -= SourceManager_SourceLost;

        SourceManager.SourceReleased -= SourceManager_SourceReleased;
        SourceManager.SourcePressed -= SourceManager_SourcePressed;
    }
}