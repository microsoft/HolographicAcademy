using Academy.HoloToolkit.Unity;
using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class UserVoiceEffect : MonoBehaviour
{
    public GameObject ParentObject;
    
    [Tooltip("Sets the input volume from the microphone.")]
    [Range(0.1f, 2f)]
    public float InputGain = 1f;

    [Tooltip("Time, in seconds, between audio influence updates.  0 indicates to update every frame.")]
    [Range(0.0f, 1.0f)]
    public float UpdateInterval = 0.25f;

    [Tooltip("The user must be at this distance, or closer, for the echo effect to be enabled.")]
    [Range(0f, 20f)]
    public float MaxDistance = 0.2f;

    public bool UseChorus = true;
    public ChorusSettings ChorusParameters = new ChorusSettings();

    public bool UseEcho = true;
    public EchoSettings EchoParameters = new EchoSettings();

    private AudioChorusFilter chorusFilter;
    private AudioEchoFilter echoFilter;

    // Time of last audio processing update.
    private DateTime lastUpdate = DateTime.MinValue;

    private void Awake()
    {
        // Create and initialize the chorus filter.
        if (UseChorus)
        {
            chorusFilter = gameObject.AddComponent<AudioChorusFilter>();
            chorusFilter.enabled = true;
            UpdateChorusFilter();
        }

        // Create and initialize the echo filter.
        if (UseEcho)
        {
            echoFilter = gameObject.AddComponent<AudioEchoFilter>();
            echoFilter.enabled = true;
            UpdateEchoFilter();
        }

        // Confgure the microphone stream to use the high quality voice source
        // at the application's output sample rate.
        MicStream.MicInitializeCustomRate(
            (int)MicStream.StreamCategory.HIGH_QUALITY_VOICE, 
            AudioSettings.outputSampleRate);
        
        // Set the initial microphone gain.
        MicStream.MicSetGain(InputGain);
        
        // Start the stream.
        // Do not keep the data and do not preview.
        MicStream.MicStartStream(false, false);
        MicStream.MicPause();
    }

    private void Update()
    {
        DateTime now = DateTime.Now;

        // Enable / disable the echo as appropriate
        if ((UpdateInterval * 1000.0f) <= (now - lastUpdate).Milliseconds)
        {
            // Update the input gain.
            MicStream.MicSetGain(InputGain);

            // Update the filter properties.
            if (UseChorus)
            {
                UpdateChorusFilter();
            }
            if (UseEcho)
            {
                UpdateEchoFilter();
            }

            EnableMicrophone();
            lastUpdate = now;
        }
    }

    private void OnAudioFilterRead(float[] buffer, int numChannels)
    {
        MicStream.MicGetFrame(buffer, buffer.Length, numChannels);
    }

    private void OnDestroy()
    {
        MicStream.MicDestroy();
    }

    private void EnableMicrophone()
    {
        bool enable = false;

        // Check to see if the user is within MaxDistance.
        float distance = Mathf.Abs(
            Vector3.Distance(
                ParentObject.transform.position, 
                Camera.main.transform.position));
        if (distance <= MaxDistance)
        {
            RaycastHit hitInfo;

            // Check to see if the user is facing the object.
            // We raycast in the direction of the user's gaze and check for collision with the Echo layer. 
            enable = Physics.Raycast(Camera.main.transform.position,
                                    Camera.main.transform.forward,
                                    out hitInfo,
                                    20f,
                                    LayerMask.GetMask("Echoer"),
                                    QueryTriggerInteraction.Collide);
        }

        if (enable)
        {
            // Resume the microphone stream.
            MicStream.MicResume();
        }
        else 
        {
            // Pause the microphone stream.
            MicStream.MicPause();
        }
    }

    private void UpdateChorusFilter()
    {
        if (chorusFilter == null)
        {
            Debug.LogError("Chorus filter has not been created.");
            return;
        }

        chorusFilter.delay = ChorusParameters.Delay;
        chorusFilter.depth = ChorusParameters.Depth;
        chorusFilter.rate = ChorusParameters.Rate;
        chorusFilter.wetMix1 = ChorusParameters.Tap1Volume;
        chorusFilter.wetMix2 = ChorusParameters.Tap2Volume;
        chorusFilter.wetMix3 = ChorusParameters.Tap3Volume;
        chorusFilter.dryMix = ChorusParameters.OriginalSoundVolume;
    }

    private void UpdateEchoFilter()
    {
        if (echoFilter == null)
        {
            Debug.LogError("Echo filter has not been created.");
            return;
        }

        echoFilter.delay = EchoParameters.Delay;
        echoFilter.decayRatio = EchoParameters.DecayRatio;
        echoFilter.dryMix = EchoParameters.OriginalSoundVolume;
        echoFilter.wetMix = EchoParameters.EchoVolume;
    }
}
