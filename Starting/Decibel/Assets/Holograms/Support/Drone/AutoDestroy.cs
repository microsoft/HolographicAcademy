// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Academy
{
    public class AutoDestroy : MonoBehaviour
    {
        public GameObject Poly;

        void Start()
        {
            Invoke("DestroyMe", 2.0f);
        }

        void DestroyMe()
        {
            Destroy(this.gameObject);
        }

        void Update()
        {
            transform.position = Poly.transform.position;
        }
    }
}