// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.WSA.Input;

namespace Academy
{
    public class Fitbox : MonoBehaviour
    {
        [Tooltip("The collection of holograms to show when the Fitbox is dismissed.")]
        public GameObject HologramCollection;

        [Tooltip("Reposition the collection of holograms relative to where the Fitbox was dismissed.")]
        public bool MoveCollectionOnDismiss = false;

        [Tooltip("The material used to render the Fitbox border.")]
        public Material FitboxMaterial;

        // The offset from the Camera to the HologramCollection when
        // the app starts up. This is used to place the Collection
        // in the correct relative position after the Fitbox is
        // dismissed.
        private Vector3 collectionStartingOffsetFromCamera;

        private float Distance = 2.0f;

        private Interpolator interpolator;
        private GestureRecognizer recognizer;
        private bool isInitialized = false;

        private void Awake()
        {
            if (Application.isEditor && !XRDevice.isPresent)
            {
                // If we are running inside Unity's Editor, disable the Fitbox script
                // as there is no easy way to dismiss it to see our actual holograms.
                DestroyImmediate(gameObject);
            }
            else
            {
                // These are the holograms to show when the Fitbox is dismissed
                if (HologramCollection)
                {
                    collectionStartingOffsetFromCamera = HologramCollection.transform.localPosition;
                    HologramCollection.SetActive(false);
                }

                // Set up our GestureRecognizer to listen for the SelectEvent
                recognizer = new GestureRecognizer();
                recognizer.Tapped += (args) =>
                {
                    DismissFitbox();
                };
                recognizer.StartCapturingGestures();
            }
        }

        private void DismissFitbox()
        {
            // Destroy the GestureRecognizer ...
            recognizer.CancelGestures();
            recognizer.StopCapturingGestures();
            recognizer.Dispose();
            recognizer = null;

            // ... show the hologram collection ...
            if (HologramCollection)
            {
                HologramCollection.SetActive(true);

                if (MoveCollectionOnDismiss)
                {
                    // Update the Hologram Collection's position so it shows up
                    // where the Fitbox left off. Start with the camera's localRotation...
                    Quaternion camQuat = Camera.main.transform.localRotation;

                    // ... ignore pitch by disabling rotation around the x axis
                    camQuat.x = 0;

                    // Rotate the vector and factor y back into the position
                    Vector3 newPosition = camQuat * collectionStartingOffsetFromCamera;
                    newPosition.y = collectionStartingOffsetFromCamera.y;

                    // Position was "Local Position" so add that to where the camera is now
                    HologramCollection.transform.position = Camera.main.transform.position + newPosition;

                    // Rotate the Hologram Collection to face the user.
                    Quaternion toQuat = Camera.main.transform.localRotation * HologramCollection.transform.rotation;
                    toQuat.x = 0;
                    toQuat.z = 0;
                    HologramCollection.transform.rotation = toQuat;
                }
            }

            // ... and Destroy the Fitbox
            Destroy(gameObject);
        }

        private void Start()
        {
            if (interpolator == null)
            {
                interpolator = gameObject.AddComponent<Interpolator>();
            }

            // Screen-lock the Fitbox to match the OOBE Fitbox experience
            interpolator.PositionPerSecond = 0.0f;
        }

        private void InitializeComponents()
        {
            // Return early if we've already been initialized...
            if (isInitialized || Camera.main == null)
            {
                return;
            }

            // Calculate Hologram Dimensions and Positions based on RedLine drawing
            const int drawingPixelWidth = 1440;
            const int drawingPixelHeight = 818;
            const int drawingBoxLeftEdgeScreen = 240;
            const int drawingBoxBottomEdgeScreen = 139;
            const int drawingBoxWidthScreen = 960;
            const int drawingBoxHeightScreen = 540;
            const int drawingQuadWidthScreen = 8;
            const int drawingQuadHeightScreen = 8;
            // Calculate a ratio between the actual screen dimensions and those of the RedLine drawing
            var xRatio = (float)Camera.main.pixelWidth / (float)drawingPixelWidth;
            var yRatio = (float)Camera.main.pixelHeight / (float)drawingPixelHeight;
            // Factor the real dimensions in screen space
            var realBoxLeftEdgeScreen = drawingBoxLeftEdgeScreen * xRatio;
            var realBoxBottomEdgeScreen = drawingBoxBottomEdgeScreen * yRatio;
            var realBoxWidthScreen = drawingBoxWidthScreen * xRatio;
            var realBoxHeightScreen = drawingBoxHeightScreen * yRatio;
            var realQuadWidthScreen = drawingQuadWidthScreen * xRatio;
            var realQuadHeightScreen = drawingQuadHeightScreen * yRatio;
            // Calculate the real width of the top/bottom (horizontal) quads
            var hQuadLeftEdgeScreen = new Vector3(realBoxLeftEdgeScreen, realBoxBottomEdgeScreen + (realQuadHeightScreen / 2.0f), Distance);
            var hQuadLeftEdge = Camera.main.ScreenToWorldPoint(hQuadLeftEdgeScreen);
            var hQuadRightEdgeScreen = new Vector3(realBoxLeftEdgeScreen + realBoxWidthScreen, hQuadLeftEdgeScreen.y, Distance);
            var hQuadRightEdge = Camera.main.ScreenToWorldPoint(hQuadRightEdgeScreen);
            var hQuadWid = Vector3.Distance(hQuadRightEdge, hQuadLeftEdge);
            // Calculate the real height of the top/bottom (horizontal) quads
            var hQuadBottomEdgeScreen = new Vector3(0, hQuadLeftEdge.y, Distance);
            var hQuadBottomEdge = Camera.main.ScreenToWorldPoint(hQuadBottomEdgeScreen);
            var hQuadTopEdgeScreen = new Vector3(0, hQuadBottomEdgeScreen.y + realQuadHeightScreen, Distance);
            var hQuadTopEdge = Camera.main.ScreenToWorldPoint(hQuadTopEdgeScreen);
            var hQuadHgt = Vector3.Distance(hQuadTopEdge, hQuadBottomEdge);
            // Calculate the real height of the left/right (vertical) quads
            var vQuadBottomEdgeScreen = new Vector3(realBoxLeftEdgeScreen + (realQuadWidthScreen / 2.0f), realBoxBottomEdgeScreen, Distance);
            var vQuadBottomEdge = Camera.main.ScreenToWorldPoint(vQuadBottomEdgeScreen);
            var vQuadTopEdgeScreen = new Vector3(vQuadBottomEdgeScreen.x, realBoxBottomEdgeScreen + realBoxHeightScreen, Distance);
            var vQuadTopEdge = Camera.main.ScreenToWorldPoint(vQuadTopEdgeScreen);
            var vQuadHgt = Vector3.Distance(vQuadTopEdge, vQuadBottomEdge);
            // Calculate the real width of the left/right quads...
            // ...just use the height of the horizontal quad for the width of the vertical quad
            var vQuadWid = hQuadHgt;

            // Create the Quads for our FitBox
            /*  leftQuad*/
            CreateFitboxQuad(transform, (-hQuadWid / 2.0f) + (vQuadWid / 2.0f), 0f, vQuadWid, vQuadHgt);
            /* rightQuad*/
            CreateFitboxQuad(transform, (hQuadWid / 2.0f) - (vQuadWid / 2.0f), 0f, vQuadWid, vQuadHgt);
            /*bottomQuad*/
            CreateFitboxQuad(transform, 0f, (vQuadHgt / 2.0f) - (hQuadHgt / 2.0f), hQuadWid, hQuadHgt);
            /*   topQuad*/
            CreateFitboxQuad(transform, 0f, (-vQuadHgt / 2.0f) + (hQuadHgt / 2.0f), hQuadWid, hQuadHgt);

            isInitialized = true;
        }

        private void CreateFitboxQuad(Transform parent, float xPos, float yPos, float width, float height)
        {
            var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            quad.transform.parent = parent;
            quad.transform.localPosition = new Vector3(xPos, yPos, 0);
            quad.transform.localScale = new Vector3(width, height, quad.transform.localScale.z);
            quad.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
            quad.GetComponent<MeshRenderer>().material = FitboxMaterial;
        }

        int foo = 0;
        private void LateUpdate()
        {
            foo++;
            if (foo < 2) return;
            InitializeComponents();

            Transform cameraTransform = Camera.main.transform;

            interpolator.SetTargetPosition(cameraTransform.position + (cameraTransform.forward * Distance));
            interpolator.SetTargetRotation(Quaternion.LookRotation(-cameraTransform.forward, -cameraTransform.up));
        }
    }
}