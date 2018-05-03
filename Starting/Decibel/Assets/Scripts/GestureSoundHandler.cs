// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity.InputModule;
using UnityEngine;

namespace Academy
{
    public class GestureSoundHandler : MonoBehaviour, INavigationHandler
    {
        [SerializeField]
        private AudioClip NavigationStartedClip;

        [SerializeField]
        private AudioClip NavigationUpdatedClip;

        public AudioClip[] AudioClips
        {
            get; private set;
        }

        private enum GestureTypes
        {
            NavigationCanceled,
            NavigationCompleted,
            NavigationStarted,
            NavigationUpdated,

            Count
        }

        private AudioSource audioSource;

        private void Awake()
        {
            // Make it super convenient for designers to specify sounds in the UI 
            // and developers to access specific sounds in code.
            AudioClips = new AudioClip[(int)GestureTypes.Count];
            AudioClips[(int)GestureTypes.NavigationStarted] = NavigationStartedClip;
            AudioClips[(int)GestureTypes.NavigationUpdated] = NavigationUpdatedClip;
        }

        private void Start()
        {
            audioSource = gameObject.AddComponent<AudioSource>();

            // Set the spatialize field of the audioSource to true.
            audioSource.spatialize = true;
            // Set the spatialBlend field of the audioSource to 1.0f.
            audioSource.spatialBlend = 1.0f;
            // Set the dopplerLevel field of the audioSource to 0.0f.
            audioSource.dopplerLevel = 0.0f;
            // Set the rolloffMode field of the audioSource to the Logarithmic AudioRolloffMode.
            audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
        }

        void INavigationHandler.OnNavigationStarted(NavigationEventData eventData)
        {
            AudioClip audioClip = AudioClips[(int)GestureTypes.NavigationStarted];

            if (audioClip != null)
            {
                // Set the AudioSource clip field to the audioClip
                audioSource.clip = audioClip;

                // Play the AudioSource
                audioSource.Play();
            }
        }

        void INavigationHandler.OnNavigationUpdated(NavigationEventData eventData)
        {
            AudioClip audioClip = AudioClips[(int)GestureTypes.NavigationUpdated];

            if (audioClip != null)
            {
                // Set the AudioSource clip field to the audioClip
                audioSource.clip = audioClip;

                // Play the AudioSource
                audioSource.Play();
            }
        }

        void INavigationHandler.OnNavigationCompleted(NavigationEventData eventData)
        {
            // Stop the AudioSource
            audioSource.Stop();
        }

        void INavigationHandler.OnNavigationCanceled(NavigationEventData eventData)
        {
            // Stop the AudioSource
            audioSource.Stop();
        }
    }
}