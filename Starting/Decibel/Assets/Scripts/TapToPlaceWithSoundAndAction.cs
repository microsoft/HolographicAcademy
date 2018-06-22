// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity.InputModule;
using UnityEngine;
using UnityEngine.Events;

namespace Academy
{
    public class TapToPlaceWithSoundAndAction : TapToPlaceWithSound
    {
        [SerializeField]
        private UnityEvent onPickupAction;

        [SerializeField]
        private UnityEvent onPlacementAction;

        public override void OnInputClicked(InputClickedEventData eventData)
        {
            base.OnInputClicked(eventData);

            if (IsBeingPlaced)
            {
                onPickupAction.Invoke();
            }
            else
            {
                onPlacementAction.Invoke();
            }
        }

    }
}