// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity.InputModule;
using UnityEngine;

namespace Academy
{
    /// <summary>
    /// GestureAction performs custom actions based on 
    /// which gesture is being performed.
    /// </summary>
    public class GestureAction : MonoBehaviour, INavigationHandler, IManipulationHandler, ISpeechHandler
    {
        [Tooltip("Rotation max speed controls amount of rotation.")]
        [SerializeField]
        private float RotationSensitivity = 10.0f;

        private bool isNavigationEnabled = true;
        public bool IsNavigationEnabled
        {
            get { return isNavigationEnabled; }
            set { isNavigationEnabled = value; }
        }

        private Vector3 manipulationOriginalPosition = Vector3.zero;

        void INavigationHandler.OnNavigationStarted(NavigationEventData eventData)
        {
            InputManager.Instance.PushModalInputHandler(gameObject);
        }

        void INavigationHandler.OnNavigationUpdated(NavigationEventData eventData)
        {
            if (isNavigationEnabled)
            {
                /* TODO: DEVELOPER CODING EXERCISE 2.c */

                // 2.c: Calculate a float rotationFactor based on eventData's NormalizedOffset.x multiplied by RotationSensitivity.
                // This will help control the amount of rotation.


                // 2.c: transform.Rotate around the Y axis using rotationFactor.

            }
        }

        void INavigationHandler.OnNavigationCompleted(NavigationEventData eventData)
        {
            InputManager.Instance.PopModalInputHandler();
        }

        void INavigationHandler.OnNavigationCanceled(NavigationEventData eventData)
        {
            InputManager.Instance.PopModalInputHandler();
        }

        void IManipulationHandler.OnManipulationStarted(ManipulationEventData eventData)
        {
            if (!isNavigationEnabled)
            {
                InputManager.Instance.PushModalInputHandler(gameObject);

                manipulationOriginalPosition = transform.position;
            }
        }

        void IManipulationHandler.OnManipulationUpdated(ManipulationEventData eventData)
        {
            if (!isNavigationEnabled)
            {
                /* TODO: DEVELOPER CODING EXERCISE 4.a */

                // 4.a: Make this transform's position be the manipulationOriginalPosition + eventData.CumulativeDelta

            }
        }

        void IManipulationHandler.OnManipulationCompleted(ManipulationEventData eventData)
        {
            InputManager.Instance.PopModalInputHandler();
        }

        void IManipulationHandler.OnManipulationCanceled(ManipulationEventData eventData)
        {
            InputManager.Instance.PopModalInputHandler();
        }

        void ISpeechHandler.OnSpeechKeywordRecognized(SpeechEventData eventData)
        {
            if (eventData.RecognizedText.ToLower().Equals("move astronaut"))
            {
                isNavigationEnabled = false;
            }
            else if (eventData.RecognizedText.ToLower().Equals("rotate astronaut"))
            {
                isNavigationEnabled = true;
            }
            else
            {
                return;
            }

            eventData.Use();
        }
    }
}