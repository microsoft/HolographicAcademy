// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity.InputModule;
using UnityEngine;

namespace Academy
{
    /// <summary>
    /// Destroys the GameObject when it receives the OnSelect message.
    /// </summary>
    public class DismissOnSelect : MonoBehaviour, IInputClickHandler
    {
        [Tooltip("Audio clip to play when interacting with this hologram.")]
        public AudioClip TargetFeedbackSound;

        private AudioSource audioSource;
        private GameObject audioGameObject;

        private void Awake()
        {
            // If this hologram has an audio clip, add an AudioSource with this clip.
            if (TargetFeedbackSound != null)
            {
                audioGameObject = new GameObject();
                audioGameObject.transform.position = gameObject.transform.position;
                audioSource = audioGameObject.AddComponent<AudioSource>();

                audioSource.playOnAwake = false;
                audioSource.clip = TargetFeedbackSound;
                audioSource.spatialBlend = 1;
                audioSource.dopplerLevel = 0;
            }
        }

        private void OnDestroy()
        {
            Destroy(audioGameObject);
        }

        void IInputClickHandler.OnInputClicked(InputClickedEventData eventData)
        {
            PlayAudioHapticFeedback();

            gameObject.SetActive(false);
        }

        private void PlayAudioHapticFeedback()
        {
            if (audioSource != null)
            {
                audioSource.Play();
            }
        }
    }
}