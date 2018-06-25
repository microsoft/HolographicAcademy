// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.SpatialMapping;
using UnityEngine;

namespace Academy
{
    /// <summary>
    /// The TapToPlace class is a basic way to enable users to move objects 
    /// and place them on real world surfaces.
    /// Put this script on the object you want to be able to move. 
    /// Users will be able to tap objects, gaze elsewhere, and perform the
    /// tap gesture again to place.
    /// </summary>

    public class TapToPlaceWithSound : TapToPlace
    {
        [SerializeField]
        [Tooltip("Sound to play when game object is placed on spatial map.")]
        private AudioClip placementSound = null;

        [SerializeField]
        [Tooltip("Sound to play when game object is picked up.")]
        private AudioClip pickupSound = null;

        private AudioSource audioSource;

        protected override void Start()
        {
            base.Start();

            if (placementSound == null || pickupSound == null)
            {
                Debug.LogError("TapToPlaceWithSound script needs placement and pickup sound audio clips.");
                return;
            }

            // Add an AudioSource component and set up some defaults.
            audioSource = gameObject.EnsureComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialize = true;
            audioSource.spatialBlend = 1.0f;
            audioSource.dopplerLevel = 0.0f;
            audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
        }

        public override void OnInputClicked(InputClickedEventData eventData)
        {
            base.OnInputClicked(eventData);

            PlaySoundCue();
        }

        protected void PlaySoundCue()
        {
            // Play the right sound based on the current placement state,
            if (IsBeingPlaced)
            {
                audioSource.Stop();
                audioSource.clip = pickupSound;
                audioSource.Play();
            }
            // If the user is not in placing mode, hide the spatial mapping mesh.
            else
            {
                audioSource.Stop();
                audioSource.clip = placementSound;
                audioSource.Play();
            }
        }
    }
}
