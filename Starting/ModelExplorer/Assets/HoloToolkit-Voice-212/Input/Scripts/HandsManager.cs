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

            InteractionManager.InteractionSourcePressed += InteractionManager_InteractionSourcePressed;
            InteractionManager.InteractionSourceReleased += InteractionManager_InteractionSourceReleased;

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

                // Cache InteractibleManager's FocusedGameObject in FocusedGameObject.
                FocusedGameObject = InteractibleManager.Instance.FocusedGameObject;
            }
        }

        private void InteractionManager_InteractionSourceReleased(InteractionSourceReleasedEventArgs hand)
        {
            ResetFocusedGameObject();
        }

        private void ResetFocusedGameObject()
        {
            FocusedGameObject = null;

            // Call ResetGestureRecognizers to complete any currently active gestures.
            GestureManager.Instance.ResetGestureRecognizers();
        }

        void OnDestroy()
        {
            InteractionManager.InteractionSourceDetected -= InteractionManager_InteractionSourceDetected;
            InteractionManager.InteractionSourceLost -= InteractionManager_InteractionSourceLost;

            InteractionManager.InteractionSourceReleased -= InteractionManager_InteractionSourceReleased;
            InteractionManager.InteractionSourcePressed -= InteractionManager_InteractionSourcePressed;
        }
    }
}