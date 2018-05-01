// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Academy
{
    public class ShrinkAway : MonoBehaviour
    {
        Renderer[] renderers;

        void Start()
        {
            renderers = GetComponentsInChildren<Renderer>();
        }

        void Update()
        {
            transform.localScale = transform.localScale - Vector3.one * 0.01f;
            if (transform.localScale.magnitude < 0.1f)
            {
                for (int index = 0; index < renderers.Length; index++)
                {
                    renderers[index].enabled = false;
                }
                Destroy(this);
            }
        }
    }
}