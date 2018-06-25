// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity.InputModule;
using UnityEngine;

namespace Academy
{
    public class PolyChargerRotation : MonoBehaviour, INavigationHandler
    {
        [Tooltip("How rapidly should the charger rotate.")]
        [Range(2.0f, 100.0f)]
        public float RotationSensitivity = 10.0f;

        private void PerformRotation(float rotationAmount)
        {
            // This will help control the amount of rotation.
            float rotationFactor = rotationAmount * RotationSensitivity;

            // Rotate along the Y axis using rotationFactor.
            transform.parent.Rotate(new Vector3(0, -1 * rotationFactor, 0));
        }

        /// <summary>
        /// Handles navigation start messages.
        /// </summary>
        void INavigationHandler.OnNavigationStarted(NavigationEventData eventData)
        {
            PerformRotation(eventData.NormalizedOffset.x);
        }

        /// <summary>
        /// Handles navigation update messages.
        /// </summary>
        void INavigationHandler.OnNavigationUpdated(NavigationEventData eventData)
        {
            PerformRotation(eventData.NormalizedOffset.x);
        }

        void INavigationHandler.OnNavigationCompleted(NavigationEventData eventData)
        {
            // Do nothing
        }

        void INavigationHandler.OnNavigationCanceled(NavigationEventData eventData)
        {
            // Do nothing
        }
    }
}