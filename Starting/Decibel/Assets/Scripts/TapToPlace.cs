// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Academy.HoloToolkit.Unity
{
    /// <summary>
    /// The TapToPlace class is a basic way to enable users to move objects 
    /// and place them on real world surfaces.
    /// Put this script on the object you want to be able to move. 
    /// Users will be able to tap objects, gaze elsewhere, and perform the
    /// tap gesture again to place.
    /// This script is used in conjunction with GazeManager, GestureManager,
    /// and SpatialMappingManager.
    /// </summary>

    public partial class TapToPlace : MonoBehaviour
    {
        [Tooltip("Material applied to the spatial map, while placing an object.")]
        public Material PlacementMaterial;
    
        [Tooltip("Sound to play when game object is placed on spatial map.")]
        public AudioClip placementSound;
        [Tooltip("Sound to play when game object is picked up.")]
        public AudioClip pickupSound;

        AudioSource audioSource = null;
        bool placing = false;
        Material nonPlacementMaterial;

        // Multiplier to offset Y value when placing object on surface.
        float offsetValue = 0.1f;

        void Start()
        {
            if (gameObject.GetComponent<Collider>() == null)
            {
                Debug.LogError("Ensure you have a collider so gaze and gesture works on this object.");
                return;
            }
            if (placementSound == null || pickupSound == null)
            {
                Debug.LogError("TapToPlace script needs placement and pickup sound audio clips.");
                return;
            }
            if (PlacementMaterial == null)
            {
                Debug.LogError("TapToPlace script needs a material to use while placing an object.");
                return;
            }

            // Add an AudioSource component and set up some defaults.
            audioSource = gameObject.GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
            audioSource.playOnAwake = false;
            audioSource.spatialize = true;
            audioSource.spatialBlend = 1.0f;
            audioSource.dopplerLevel = 0.0f;
            audioSource.rolloffMode = AudioRolloffMode.Logarithmic;

        }

        // Called by GazeGestureManager when the user performs a tap gesture.
        void OnSelect()
        {
            if (SpatialMappingManager.Instance == null)
            {
                Debug.LogError("TapToPlace requires spatial mapping.");
                return;
            }

            // On each tap gesture, toggle whether the user is in placing mode.
            placing = !placing;

            // If the user is in placing mode, display the spatial mapping mesh.
            if (placing)
            {
                GestureManager.Instance.OverrideFocusedObject = transform.gameObject;
                audioSource.clip = pickupSound;
                audioSource.Play();

                nonPlacementMaterial = SpatialMappingManager.Instance.surfaceMaterial;
                SpatialMappingManager.Instance.surfaceMaterial = PlacementMaterial;

                SpatialMappingManager.Instance.DrawVisualMeshes = true;
            }
            // If the user is not in placing mode, hide the spatial mapping mesh.
            else
            {
                GestureManager.Instance.OverrideFocusedObject = null;
                audioSource.clip = placementSound;
                audioSource.Play();

                SpatialMappingManager.Instance.surfaceMaterial = nonPlacementMaterial;

                SpatialMappingManager.Instance.DrawVisualMeshes = false;
            }
        }

        // Update is called once per frame.
        void Update()
        {
            // If the user is in placing mode,
            // update the placement to match the user's gaze.
            if (placing)
            {
                // Do a raycast into the world that will only hit the Spatial Mapping mesh.
                var headPosition = Camera.main.transform.position;
                var gazeDirection = Camera.main.transform.forward;

                RaycastHit hitInfo;
                if (Physics.Raycast(headPosition, gazeDirection, out hitInfo,
                    30.0f, SpatialMappingManager.Instance.LayerMask))
                {
                    // Move this object to where the raycast
                    // hit the Spatial Mapping mesh.
                    // Here is where you might consider adding intelligence
                    // to how the object is placed.  For example, consider
                    // placing based on the bottom of the object's
                    // collider so it sits properly on surfaces.

                    this.transform.parent.position = hitInfo.point + (Vector3.up * offsetValue);

                    // Rotate this object to face the user.
                    Quaternion toQuat = Camera.main.transform.localRotation;
                    toQuat.x = 0;
                    toQuat.z = 0;
                    this.transform.parent.rotation = toQuat;
                }
            }
        }
    }
}
