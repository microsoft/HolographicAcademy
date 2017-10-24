using UnityEngine;
using UnityEngine.XR.WSA.Input;

namespace Academy.HoloToolkit.Unity
{
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

            InteractionManager.InteractionSourceDetected += InteractionManager_InteractionSourceDetected;
            InteractionManager.InteractionSourceLost += InteractionManager_InteractionSourceLost;

            /* TODO: DEVELOPER CODE ALONG 2.a */

            // 2.a: Register for SourceManager.SourcePressed event.
            InteractionManager.InteractionSourcePressed += InteractionManager_InteractionSourcePressed;

            // 2.a: Register for SourceManager.SourceReleased event.
            InteractionManager.InteractionSourceReleased += InteractionManager_InteractionSourceReleased;

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

        private void InteractionManager_InteractionSourceDetected(InteractionSourceDetectedEventArgs obj)
        {
            HandDetected = true;
        }

        private void InteractionManager_InteractionSourceLost(InteractionSourceLostEventArgs obj)
        {
            HandDetected = false;

            // 2.a: Reset FocusedGameObject.
            ResetFocusedGameObject();
        }

        private void InteractionManager_InteractionSourcePressed(InteractionSourcePressedEventArgs hand)
        {
            if (InteractibleManager.Instance.FocusedGameObject != null)
            {
                // Play a select sound if we have an audio source and are not targeting an asset with a select sound.
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

        private void InteractionManager_InteractionSourceReleased(InteractionSourceReleasedEventArgs hand)
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
            InteractionManager.InteractionSourceDetected -= InteractionManager_InteractionSourceDetected;
            InteractionManager.InteractionSourceLost -= InteractionManager_InteractionSourceLost;

            // 2.a: Unregister the SourceManager.SourceReleased event.
            InteractionManager.InteractionSourceReleased -= InteractionManager_InteractionSourceReleased;

            // 2.a: Unregister for SourceManager.SourcePressed event.
            InteractionManager.InteractionSourcePressed -= InteractionManager_InteractionSourcePressed;
        }
    }
}