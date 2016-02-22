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

        /* TODO: DEVELOPER CODE ALONG 2.a */

        // 2.a: Register for SourceManager.SourcePressed event.
        SourceManager.SourcePressed += SourceManager_SourcePressed;

        // 2.a: Register for SourceManager.SourceReleased event.
        SourceManager.SourceReleased += SourceManager_SourceReleased;

        // 2.a: Initialize FocusedGameObject as null.
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

        // 2.a: Reset FocusedGameObject.
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

            // 2.a: Cache InteractibleManager's FocusedGameObject in FocusedGameObject.
            FocusedGameObject = InteractibleManager.Instance.FocusedGameObject;
        }
    }

    private void SourceManager_SourceReleased(SourceState hand)
    {
        // 2.a: Reset FocusedGameObject.
        ResetFocusedGameObject();
    }

    private void ResetFocusedGameObject()
    {
        // 2.a: Set FocusedGameObject to be null.
        FocusedGameObject = null;

        // 2.a: On GestureManager call ResetGestureRecognizers
        // to complete any currently active gestures.
        GestureManager.Instance.ResetGestureRecognizers();
    }

    void OnDestroy()
    {
        SourceManager.SourceDetected -= SourceManager_SourceDetected;
        SourceManager.SourceLost -= SourceManager_SourceLost;

        // 2.a: Unregister the SourceManager.SourceReleased event.
        SourceManager.SourceReleased -= SourceManager_SourceReleased;

        // 2.a: Unregister for SourceManager.SourcePressed event.
        SourceManager.SourcePressed -= SourceManager_SourcePressed;
    }
}