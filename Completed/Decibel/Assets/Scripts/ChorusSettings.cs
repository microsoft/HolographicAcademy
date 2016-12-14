using System;
using System.Collections;
using UnityEngine;

[Serializable]

public class ChorusSettings
{
    [Tooltip("Time delay, in ms, of the chorus effect.")]
    [Range(0.1f, 100f)]
    public float Delay = 40f;

    [Tooltip("The depth of the chorus effect.")]
    [Range(0f, 1f)]
    public float Depth = 0.03f;

    [Tooltip("The rate, in Hz, of the chorus effect.")]
    [Range(0f, 20f)]
    public float Rate = 0.8f;

    [Tooltip("Volume level of the first chorus tab.")]
    [Range(0f, 1f)]
    public float Tap1Volume = 0.5f;

     [Tooltip("Volume level of the second chorus tab.")]
    [Range(0f, 1f)]
    public float Tap2Volume = 0.5f;

    [Tooltip("Volume level of the third chorus tab.")]
    [Range(0f, 1f)]
    public float Tap3Volume = 0.5f;

    [Tooltip("Volume level of the original sound source.")]
    [Range(0f, 1f)]
    public float OriginalSoundVolume = 1.0f;

}
