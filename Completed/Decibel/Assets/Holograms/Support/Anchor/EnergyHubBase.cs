// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity;
using UnityEngine;
using UnityEngine.XR.WSA;

namespace Academy
{
    public class EnergyHubBase : MonoBehaviour
    {
        [SerializeField]
        private AudioClip anchorLanding;

        [SerializeField]
        private Material placingMaterial = null;

        [SerializeField]
        private Material placedMaterial = null;

        private AudioSource audioSource;
        private Renderer blobOutsideRenderer;
        private Animator animator;

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.clip = anchorLanding;
            audioSource.loop = false;

            blobOutsideRenderer = transform.Find("BlobOutside").gameObject.GetComponent<Renderer>();
            animator = GetComponent<Animator>();

            OnSelect();
        }

        private void Update()
        {
            if (IsTargetVisible())
            {
                HolographicSettings.SetFocusPointForFrame(gameObject.transform.position, -CameraCache.Main.transform.forward);
            }
        }

        private bool IsTargetVisible()
        {
            // This will return true if the target's mesh is within the Main Camera's view frustums.
            Vector3 targetViewportPosition = CameraCache.Main.WorldToViewportPoint(gameObject.transform.position);
            return (targetViewportPosition.x > 0.0 && targetViewportPosition.x < 1 &&
                targetViewportPosition.y > 0.0 && targetViewportPosition.y < 1 &&
                targetViewportPosition.z > 0);
        }

        public void OnSelect()
        {
            foreach (Transform child in transform)
            {
                MaterialSwap(child, placingMaterial, placedMaterial);
                foreach (Transform childnested in child.transform)
                {
                    MaterialSwap(childnested, placingMaterial, placedMaterial);
                }
            }

            blobOutsideRenderer.enabled = true;
            animator.speed = 1;

            audioSource.Play();
        }

        public void ResetAnimation()
        {
            animator.Rebind();
            animator.speed = 0;

            // Setup Placing Object
            foreach (Transform child in transform)
            {
                MaterialSwap(child, placedMaterial, placingMaterial);
                foreach (Transform childnested in child.transform)
                {
                    MaterialSwap(childnested, placedMaterial, placingMaterial);
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

        private void MaterialSwap(Transform mesh, Material currentMaterial, Material newMaterial)
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