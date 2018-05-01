// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace Academy
{
    [Serializable]
    public class EchoSettings
    {
        [Tooltip("Time delay, in ms, between echoes.")]
        [Range(10f, 5000f)]
        public float Delay = 500f;

        [Tooltip("Echo decay between echoes. 0 == total decay.")]
        [Range(0f, 1f)]
        public float DecayRatio = 0.5f;

        [Tooltip("Volume level of the echoes.")]
        [Range(0f, 1f)]
        public float EchoVolume = 1.0f;

        [Tooltip("Volume level of the original sound source.")]
        [Range(0f, 1f)]
        public float OriginalSoundVolume = 1.0f;
    }
}