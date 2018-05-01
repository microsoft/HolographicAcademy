// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Academy
{
    public class StarArray : MonoBehaviour
    {
        public Vector3 RotationVector = new Vector3(1, 0, 0);
        public int RotationSpeed = 300;

        void Update()
        {
            transform.Rotate(RotationVector * Time.deltaTime * RotationSpeed);
        }
    }
}