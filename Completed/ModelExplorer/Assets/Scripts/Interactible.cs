// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity.InputModule;
using UnityEngine;

namespace Academy
{
    /// <summary>
    /// The Interactible class flags a Game Object as being "Interactible".
    /// Determines what happens when an Interactible is being gazed at.
    /// </summary>
    public class Interactible : MonoBehaviour, IFocusable, IInputClickHandler
    {
        [Tooltip("Audio clip to play when interacting with this hologram.")]
        public AudioClip TargetFeedbackSound;
        private AudioSource audioSource;

        private Material[] defaultMaterials;

        [SerializeField]
        private InteractibleAction interactibleAction;

        private void Start()
        {
            defaultMaterials = GetComponent<Renderer>().materials;

            // Add a BoxCollider if the interactible does not contain one.
            Collider collider = GetComponentInChildren<Collider>();
            if (collider == null)
            {
                gameObject.AddComponent<BoxCollider>();
            }

            EnableAudioHapticFeedback();
        }

        private void EnableAudioHapticFeedback()
        {
            // If this hologram has an audio clip, add an AudioSource with this clip.
            if (TargetFeedbackSound != null)
            {
                audioSource = GetComponent<AudioSource>();
                if (audioSource == null)
                {
                    audioSource = gameObject.AddComponent<AudioSource>();
                }

                audioSource.clip = TargetFeedbackSound;
                audioSource.playOnAwake = false;
                audioSource.spatialBlend = 1;
                audioSource.dopplerLevel = 0;
            }
        }

        /* TODO: DEVELOPER CODING EXERCISE 2.d */

        void IFocusable.OnFocusEnter()
        {
            for (int i = 0; i < defaultMaterials.Length; i++)
            {
                // 2.d: Uncomment the below line to highlight the material when gaze enters.
                defaultMaterials[i].EnableKeyword("_ENVIRONMENT_COLORING");
            }
        }

        void IFocusable.OnFocusExit()
        {
            for (int i = 0; i < defaultMaterials.Length; i++)
            {
                // 2.d: Uncomment the below line to remove highlight on material when gaze exits.
                defaultMaterials[i].DisableKeyword("_ENVIRONMENT_COLORING");
            }
        }

        void IInputClickHandler.OnInputClicked(InputClickedEventData eventData)
        {
            // Play the audioSource feedback when we gaze and select a hologram.
            if (audioSource != null && !audioSource.isPlaying)
            {
                audioSource.Play();
            }

            /* TODO: DEVELOPER CODING EXERCISE 6.a */
            // 6.a: Uncomment the lines below to perform a Tagalong action.
            if (interactibleAction != null)
            {
                interactibleAction.PerformAction();
            }
        }

        private void OnDestroy()
        {
            foreach (Material material in defaultMaterials)
            {
                Destroy(material);
            }
        }
    }
}