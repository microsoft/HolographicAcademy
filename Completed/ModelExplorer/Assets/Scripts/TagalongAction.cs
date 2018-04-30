// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity;
using UnityEngine;

namespace Academy
{
    public class TagalongAction : InteractibleAction
    {
        [SerializeField]
        [Tooltip("Drag the Tagalong prefab asset you want to display.")]
        private GameObject objectToTagalong;

        private void Awake()
        {
            if (objectToTagalong != null)
            {
                objectToTagalong = Instantiate(objectToTagalong);
                objectToTagalong.SetActive(false);

                /* TODO: DEVELOPER CODING EXERCISE 6.b */

                // 6.b: AddComponent Billboard to objectToTagAlong,
                // so it's always facing the user as they move.
                Billboard billboard = objectToTagalong.AddComponent<Billboard>();

                // 6.b: AddComponent SimpleTagalong to objectToTagAlong,
                // so it's always following the user as they move.
                objectToTagalong.AddComponent<SimpleTagalong>();

                // 6.b: Set any public properties you wish to experiment with.
                billboard.PivotAxis = PivotAxis.XY; // Already the default, but provided in case you want to edit
            }
        }

        public override void PerformAction()
        {
            // Recommend having only one tagalong.
            if (objectToTagalong == null || objectToTagalong.activeSelf)
            {
                return;
            }

            objectToTagalong.SetActive(true);
        }
    }
}