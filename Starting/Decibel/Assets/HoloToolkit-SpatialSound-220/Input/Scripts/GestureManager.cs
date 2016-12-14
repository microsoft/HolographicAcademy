// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.VR.WSA.Input;

namespace Academy.HoloToolkit.Unity
{
    /// <summary>
    /// GestureManager creates a gesture recognizer and signs up for a tap gesture.
    /// When a tap gesture is detected, GestureManager uses GazeManager to find the game object.
    /// GestureManager then sends a message to that game object.
    /// </summary>
    [RequireComponent(typeof(GazeManager))]
    public partial class GestureManager : Singleton<GestureManager>
    {
        /// <summary>
        /// To select even when a hologram is not being gazed at,
        /// set the override focused object.
        /// If its null, then the gazed at object will be selected.
        /// </summary>
        public GameObject OverrideFocusedObject
        {
            get; set;
        }

        /// <summary>
        /// Gets the currently focused object, or null if none.
        /// </summary>
        public GameObject FocusedObject
        {
            get { return focusedObject; }
        }

        public bool IsNavigating { get; private set; }
        public Vector3 NavigationPosition { get; private set; }

        private GameObject focusedObject;
        private GestureRecognizer gestureRecognizer;
        private GestureSoundManager gestureSoundManager;

        void Start()
        {
            // Get the GestureSoundManager, so our gestures have positive confirmation via sound.
            gestureSoundManager = GestureSoundManager.Instance;

            // Create a new GestureRecognizer. Sign up for tapped events.
            gestureRecognizer = new GestureRecognizer();
            gestureRecognizer.SetRecognizableGestures(GestureSettings.Tap | GestureSettings.NavigationX);

            gestureRecognizer.TappedEvent += GestureRecognizer_TappedEvent;

            gestureRecognizer.NavigationStartedEvent += GestureRecognizer_NavigationStartedEvent;

            gestureRecognizer.NavigationUpdatedEvent += GestureRecognizer_NavigationUpdatedEvent;

            gestureRecognizer.NavigationCompletedEvent += GestureRecognizer_NavigationCompletedEvent;

            gestureRecognizer.NavigationCanceledEvent += GestureRecognizer_NavigationCanceledEvent;

            // Start looking for gestures.
            gestureRecognizer.StartCapturingGestures();
        }

        private void GestureRecognizer_TappedEvent(InteractionSourceKind source, int tapCount, Ray headRay)
        {
            if (focusedObject != null)
            {
                focusedObject.SendMessage("OnSelect");
            }
        }

        private void GestureRecognizer_NavigationStartedEvent(InteractionSourceKind source, Vector3 relativePosition, Ray ray)
        {
            if (focusedObject != null)
            {
                IsNavigating = true;

                NavigationPosition = relativePosition;
                gestureSoundManager.OnGesture(GestureSoundManager.GestureTypes.NavigationStarted, focusedObject);
                focusedObject.SendMessageUpwards("OnNavigationStarted");
            }
        }

        private void GestureRecognizer_NavigationUpdatedEvent(InteractionSourceKind source, Vector3 relativePosition, Ray ray)
        {
            if (focusedObject != null)
            {
                IsNavigating = true;

                NavigationPosition = relativePosition;
                gestureSoundManager.OnGesture(GestureSoundManager.GestureTypes.NavigationUpdated, focusedObject);
                focusedObject.SendMessageUpwards("OnNavigationUpdated");
            }
        }

        private void GestureRecognizer_NavigationCompletedEvent(InteractionSourceKind source, Vector3 relativePosition, Ray ray)
        {
            IsNavigating = false;
            gestureSoundManager.OnGesture(GestureSoundManager.GestureTypes.NavigationCompleted, null);
        }

        private void GestureRecognizer_NavigationCanceledEvent(InteractionSourceKind source, Vector3 relativePosition, Ray ray)
        {
            IsNavigating = false;
            gestureSoundManager.OnGesture(GestureSoundManager.GestureTypes.NavigationCancelled, null);
        }

        void LateUpdate()
        {
            GameObject oldFocusedObject = focusedObject;

            if (GazeManager.Instance.Hit &&
                OverrideFocusedObject == null &&
                GazeManager.Instance.HitInfo.collider != null)
            {
                // If gaze hits a hologram, set the focused object to that game object.
                // Also if the caller has not decided to override the focused object.
                focusedObject = GazeManager.Instance.HitInfo.collider.gameObject;
            }
            else
            {
                // If our gaze doesn't hit a hologram, set the focused object to null or override focused object.
                focusedObject = OverrideFocusedObject;
            }

            if (focusedObject != oldFocusedObject)
            {
                // If the currently focused object doesn't match the old focused object, cancel the current gesture.
                // Start looking for new gestures.  This is to prevent applying gestures from one hologram to another.
                gestureRecognizer.CancelGestures();
                gestureRecognizer.StartCapturingGestures();
            }
        }

        void OnDestroy()
        {
            gestureRecognizer.StopCapturingGestures();
            gestureRecognizer.TappedEvent -= GestureRecognizer_TappedEvent;
        }
    }
}