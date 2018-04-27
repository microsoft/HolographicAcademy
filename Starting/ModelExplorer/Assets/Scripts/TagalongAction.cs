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

                // AddComponent Billboard to objectToTagAlong,
                // so it's always facing the user as they move.
                Billboard billboard = objectToTagalong.AddComponent<Billboard>();

                // AddComponent SimpleTagalong to objectToTagAlong,
                // so it's always following the user as they move.
                objectToTagalong.AddComponent<SimpleTagalong>();

                billboard.PivotAxis = PivotAxis.XY;
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