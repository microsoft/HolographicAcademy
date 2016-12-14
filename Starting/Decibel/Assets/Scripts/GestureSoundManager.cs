using System;
using UnityEngine;

namespace Academy.HoloToolkit.Unity
{
    public class GestureSoundManager : Singleton<GestureSoundManager>
    {
        public enum GestureTypes
        {
            NavigationCancelled,
            NavigationCompleted,
            NavigationStarted,
            NavigationUpdated,

            Count
        }

        // A game object that will be used to contain the gesture audio source.
        // This object will be moved to the location of the object responding to the gesture.
        private GameObject audioSourceContainer;

        private AudioSource audioSource;

        private void Start()
        {
            audioSourceContainer = new GameObject("AudioSourceContainer", new Type[] { typeof(AudioSource) });
            audioSource = audioSourceContainer.GetComponent<AudioSource>();

            // Set the spatialize field of the audioSource to true.
            audioSource.spatialize = true;
            // Set the spatialBlend field of the audioSource to 1.0f.
            audioSource.spatialBlend = 1.0f;
            // Set the dopplerLevel field of the audioSource to 0.0f.
            audioSource.dopplerLevel = 0.0f;
            // Set the rolloffMode field of the audioSource to the Logarithmic AudioRolloffMode.
            audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
        }

        public void OnGesture(GestureTypes gestureType,
                            GameObject focusedGameObject)
        {
            AudioClip audioClip = null;
            GestureSoundHandler gestureSoundHandler = null;

            if (focusedGameObject != null)
            {
                gestureSoundHandler = focusedGameObject.GetComponent<GestureSoundHandler>();
            }

            if (gestureSoundHandler != null)
            {
                // Fetch the appropriate audio clip from the GestureSoundHandler's AudioClips array.
                audioClip = gestureSoundHandler.AudioClips[(int)gestureType];
            }

            if (audioClip != null)
            {
                // Move the audio source container to the location of the focused object so that
                // the gesture sound is properly spatialized with the focused object.
                audioSourceContainer.transform.position = focusedGameObject.transform.position;

                // Set the AudioSource clip field to the audioClip
                audioSource.clip = audioClip;

                // Play the AudioSource
                audioSource.Play();
            }
            else
            {
                // Stop the AudioSource
                audioSource.Stop();
            }
        }
    }
}