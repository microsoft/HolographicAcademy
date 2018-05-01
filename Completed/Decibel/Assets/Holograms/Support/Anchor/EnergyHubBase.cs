// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using UnityEngine;
using UnityEngine.XR.WSA;

namespace Academy
{
    [RequireComponent(typeof(AudioSource))]
    public class EnergyHubBase : Singleton<EnergyHubBase>, IInputClickHandler
    {
        public AudioClip AnchorLanding;
        public Material PlacingMaterial;
        public Material PlacedMaterial;

        private AudioSource audioSource;
        private Renderer blobOutsideRenderer;
        private Animator animator;

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            blobOutsideRenderer = transform.Find("BlobOutside").gameObject.GetComponent<Renderer>();
            animator = GetComponent<Animator>();

            OnSelect();
        }

        private void Update()
        {
            if (IsTargetVisible())
            {
                HolographicSettings.SetFocusPointForFrame(gameObject.transform.position, -Camera.main.transform.forward);
            }
        }

        private bool IsTargetVisible()
        {
            // This will return true if the target's mesh is within the Main Camera's view frustums.
            Vector3 targetViewportPosition = Camera.main.WorldToViewportPoint(gameObject.transform.position);
            return (targetViewportPosition.x > 0.0 && targetViewportPosition.x < 1 &&
                targetViewportPosition.y > 0.0 && targetViewportPosition.y < 1 &&
                targetViewportPosition.z > 0);
        }

        void IInputClickHandler.OnInputClicked(InputClickedEventData eventData)
        {
            OnSelect();
        }

        public void OnSelect()
        {
            ResetAnimation();
            foreach (Transform child in transform)
            {
                MaterialSwap(child, PlacingMaterial, PlacedMaterial);
                foreach (Transform childnested in child.transform)
                {
                    MaterialSwap(childnested, PlacingMaterial, PlacedMaterial);
                }
            }
            blobOutsideRenderer.enabled = true;
            animator.speed = 1;

            audioSource.clip = AnchorLanding;
            audioSource.loop = false;
            audioSource.Play();
        }

        public void ResetAnimation()
        {
            animator.Rebind();
            animator.speed = 0;

            // Setup Placing Object
            foreach (Transform child in transform)
            {
                MaterialSwap(child, PlacedMaterial, PlacingMaterial);
                foreach (Transform childnested in child.transform)
                {
                    MaterialSwap(childnested, PlacedMaterial, PlacingMaterial);
                }
            }

            blobOutsideRenderer.enabled = false;

            audioSource.Stop();
        }

        void LightShieldsOpen()
        {
        }

        void LandingDone()
        {
        }

        void MaterialSwap(Transform mesh, Material currentMaterial, Material newMaterial)
        {
            Renderer meshRenderer = mesh.GetComponent<Renderer>();
            if (meshRenderer != null)
            {
                if (meshRenderer.sharedMaterial == currentMaterial)
                {
                    meshRenderer.material = newMaterial;
                }
            }
        }
    }
}