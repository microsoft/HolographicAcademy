// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity.InputModule;
using System.Collections;
using UnityEngine;

namespace Academy
{
    public class FriendlyDrone : MonoBehaviour, IInputClickHandler
    {
        public Color EmissiveColor = new Vector4(0f, .87f, 1f, .3f);
        public long OwningUserId { get; set; }

        [SerializeField]
        private PolyActions polyActions;

        void Start()
        {
            // Change emissive color on two meshes
            var materialBody = transform.GetComponent<Renderer>().materials;
            materialBody[1].SetColor("_EmissionColor", EmissiveColor);
            materialBody[1].SetColor("_Color", EmissiveColor);
            var materialBottom = transform.Find("SpinningBottom").GetComponent<Renderer>().materials;
            materialBottom[1].SetColor("_EmissionColor", EmissiveColor);
            materialBottom[1].SetColor("_Color", EmissiveColor);

            // Change color on child particles
            foreach (Transform child in transform)
            {
                ParticleSystem childParticleSystem = child.GetComponent<ParticleSystem>();
                if (childParticleSystem != null)
                {
                    ParticleSystem.MainModule ps = childParticleSystem.main;
                    ps.startColor = EmissiveColor;
                    ps.startSize = transform.parent.localScale.magnitude * ps.startSizeMultiplier;
                }
            }

            // Change the color alpha to 1 so stars show up more.
            EmissiveColor.a = 1.0f;

            // Change color on stars
            foreach (Transform child in transform.Find("StarArray"))
            {
                ParticleSystem childParticleSystem = child.GetComponent<ParticleSystem>();
                if (childParticleSystem != null)
                {
                    ParticleSystem.MainModule ps = childParticleSystem.main;
                    ps.startColor = EmissiveColor;
                    ps.startSize = transform.parent.localScale.magnitude * ps.startSizeMultiplier;
                }
            }
        }

        /// <summary>
        /// Plays the 'Hit' animation.
        /// </summary>
        public void PlayHit()
        {
            // Start playing hit animation
            GetComponent<Animator>().CrossFadeInFixedTime("Hit", 0.1f);

            // Get drone's eyes and enable rendering
            GameObject eyes = transform.Find("friendly_droneEyes").gameObject;
            eyes.GetComponent<MeshRenderer>().enabled = true;

            // Hack for current bug on stopping/playing particles so we will instantiate the stars prefab instead
            GameObject stars = transform.Find("StarArray").gameObject;
            var starsClone = Instantiate(stars, stars.transform.position, stars.transform.rotation) as GameObject;
            starsClone.transform.parent = transform;
            foreach (Transform child in starsClone.transform)
            {
                child.GetComponent<ParticleSystem>().Play();
            }

            // Coroutine to destroy stars and mesh after animation
            StartCoroutine(RemoveStars(starsClone, eyes));
        }

        IEnumerator RemoveStars(GameObject starArray, GameObject eyes)
        {
            yield return new WaitForSeconds(4);
            Destroy(starArray);
            eyes.GetComponent<MeshRenderer>().enabled = false;
            yield return null;
        }

        public void Happy()
        {
            GetComponent<Animator>().CrossFadeInFixedTime("Happy", 0.1f);
            GameObject eyes = transform.Find("friendly_droneSmile").gameObject;
            eyes.GetComponent<MeshRenderer>().enabled = true;
            Invoke("Normal", 3);
        }

        public void Normal()
        {
            GetComponent<Animator>().CrossFadeInFixedTime("Idle", 0.1f);
            GameObject smile = transform.Find("friendly_droneSmile").gameObject;
            smile.GetComponent<MeshRenderer>().enabled = false;
        }

        public void GoHide()
        {
            polyActions.GoHide();
        }

        void IInputClickHandler.OnInputClicked(InputClickedEventData eventData)
        {
            GoHide();
        }
    }
}