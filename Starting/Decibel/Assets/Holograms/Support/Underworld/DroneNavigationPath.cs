// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;

namespace Academy
{
    public class DroneNavigationPath : MonoBehaviour
    {
        [Tooltip("List of navigation points for the drone to move between.")]
        public List<Transform> NavigationPoints;

        [Tooltip("Indicates if the navigation path includes a point outside of the underworld.")]
        public bool HasOutPoint = false;
    }
}