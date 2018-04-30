// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity;
using UnityEngine;

namespace Academy
{
    /// <summary>
    /// Placeholder script for exploding the model.
    /// </summary>
    public class ExpandModel : Singleton<ExpandModel>
    {
        // We are using a different model for the expanded view.  Set it here so we can swap it out when we expand.
        [Tooltip("Game object for the exploded model.")]
        [SerializeField]
        private GameObject expandedModel;

        public GameObject ExpandedModel
        {
            get { return expandedModel; }
            set { expandedModel = value; }
        }

        [Tooltip("Audio clip to play when expanding the model.")]
        [SerializeField]
        private AudioClip expandModelSound;

        private AudioSource audioSource;

        public bool IsModelExpanded { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            IsModelExpanded = false;

            EnableAudioHapticFeedback();
        }

        public void Expand()
        {
            if (IsModelExpanded)
            {
                return;
            }

            if (audioSource != null)
            {
                audioSource.Play();
            }

            IsModelExpanded = true;
        }

        public void Reset()
        {
            IsModelExpanded = false;
        }

        private void EnableAudioHapticFeedback()
        {
            // If this hologram has an audio clip, add an AudioSource with this clip.
            if (expandModelSound != null)
            {
                GameObject audioGameObject = new GameObject
                {
                    name = "ExpandModelSoundEffect"
                };
                audioGameObject.transform.position = gameObject.transform.position;

                audioSource = audioGameObject.AddComponent<AudioSource>();

                audioSource.clip = expandModelSound;
                audioSource.playOnAwake = false;
                audioSource.spatialBlend = 1;
                audioSource.dopplerLevel = 0;
            }
        }
    }
}