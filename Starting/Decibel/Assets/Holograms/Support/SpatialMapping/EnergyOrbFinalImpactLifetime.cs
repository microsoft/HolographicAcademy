// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Academy
{
    public class EnergyOrbFinalImpactLifetime : MonoBehaviour
    {
        void Start()
        {
            Invoke("DestroyGameObject", 0.5f);
        }

        void DestroyGameObject()
        {
            Destroy(gameObject);
        }
    }
}