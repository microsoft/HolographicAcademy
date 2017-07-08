using Academy.HoloToolkit.Unity;
using System.Collections;
using UnityEngine;
using UnityEngine.Video;

/// <summary>
/// This keeps track of the various parts of the recording and text display process.
/// </summary>

[RequireComponent(typeof(AudioSource), typeof(MicrophoneManager), typeof(KeywordManager))]
public class Communicator : MonoBehaviour
{
    [Tooltip("The button to be selected when the user wants to record audio and dictation.")]
    public Button RecordButton;
    [Tooltip("The button to be selected when the user wants to stop recording.")]
    public Button RecordStopButton;
    [Tooltip("The button to be selected when the user wants to play audio.")]
    public Button PlayButton;
    [Tooltip("The button to be selected when the user wants to stop playing.")]
    public Button PlayStopButton;

    [Tooltip("The sound to be played when the recording session starts.")]
    public AudioClip StartListeningSound;
    [Tooltip("The sound to be played when the recording session ends.")]
    public AudioClip StopListeningSound;

    [Tooltip("The icon to be displayed while recording is happening.")]
    public GameObject MicIcon;

    [Tooltip("A message to help the user understand what to do next.")]
    public Renderer MessageUIRenderer;

    [Tooltip("The waveform animation to be played while the microphone is recording.")]
    public Transform Waveform;
    [Tooltip("The meter animation to be played while the microphone is recording.")]
    public VideoPlayer SoundMeter;

    private AudioSource dictationAudio;
    private AudioSource startAudio;
    private AudioSource stopAudio;

    private float origLocalScale;
    private bool animateWaveform;

    public enum Message
    {
        PressMic,
        PressStop,
        SendMessage
    };

    private MicrophoneManager microphoneManager;

    void Start()
    {
        dictationAudio = gameObject.GetComponent<AudioSource>();

        startAudio = gameObject.AddComponent<AudioSource>();
        stopAudio = gameObject.AddComponent<AudioSource>();

        startAudio.playOnAwake = false;
        startAudio.clip = StartListeningSound;
        stopAudio.playOnAwake = false;
        stopAudio.clip = StopListeningSound;

        microphoneManager = GetComponent<MicrophoneManager>();

        origLocalScale = Waveform.localScale.y;
        animateWaveform = false;
    }

    void Update()
    {
        if (animateWaveform)
        {
            Vector3 newScale = Waveform.localScale;
            newScale.y = Mathf.Sin(Time.time * 2.0f) * origLocalScale;
            Waveform.localScale = newScale;
        }

        // If the audio has stopped playing and the PlayStop button is still active,  reset the UI.
        if (!dictationAudio.isPlaying && PlayStopButton.enabled)
        {
            PlayStop();
        }
    }

    public void Record()
    {
        if (RecordButton.IsOn())
        {
            // Turn the microphone on, which returns the recorded audio.
            dictationAudio.clip = microphoneManager.StartRecording();

            // Set proper UI state and play a sound.
            SetUI(true, Message.PressStop, startAudio);

            RecordButton.gameObject.SetActive(false);
            RecordStopButton.gameObject.SetActive(true);
        }
    }

    public void RecordStop()
    {
        if (RecordStopButton.IsOn())
        {
            // Turn off the microphone.
            microphoneManager.StopRecording();
            // Restart the PhraseRecognitionSystem and KeywordRecognizer
            microphoneManager.StartCoroutine("RestartSpeechSystem", GetComponent<KeywordManager>());

            // Set proper UI state and play a sound.
            SetUI(false, Message.SendMessage, stopAudio);

            PlayButton.SetActive(true);
            RecordStopButton.SetActive(false);
        }
    }

    public void Play()
    {
        if (PlayButton.IsOn())
        {
            PlayButton.gameObject.SetActive(false);
            PlayStopButton.gameObject.SetActive(true);

            dictationAudio.Play();
        }
    }

    public void PlayStop()
    {
        if (PlayStopButton.IsOn())
        {
            PlayStopButton.gameObject.SetActive(false);
            PlayButton.gameObject.SetActive(true);

            dictationAudio.Stop();
        }
    }

    public void SendCommunicatorMessage()
    {
        AstronautWatch.Instance.CloseCommunicator();
    }

    void ResetAfterTimeout()
    {
        // Set proper UI state and play a sound.
        SetUI(false, Message.PressMic, stopAudio);

        RecordStopButton.gameObject.SetActive(false);
        RecordButton.gameObject.SetActive(true);
    }

    private void SetUI(bool enabled, Message newMessage, AudioSource soundToPlay)
    {
        animateWaveform = enabled;
        if (enabled)
        {
            SoundMeter.Play();
        }
        else
        {
            SoundMeter.Stop();
        }
        MicIcon.SetActive(enabled);

        StartCoroutine(ChangeLabel(newMessage));

        soundToPlay.Play();
    }

    private IEnumerator ChangeLabel(Message newMessage)
    {
        switch (newMessage)
        {
            case Message.PressMic:
                for (float i = 0.0f; i < 1.0f; i += 0.1f)
                {
                    MessageUIRenderer.material.SetFloat("_BlendTex01", Mathf.Lerp(1.0f, 0.0f, i));
                    yield return null;
                }
                break;
            case Message.PressStop:
                for (float i = 0.0f; i < 1.0f; i += 0.1f)
                {
                    MessageUIRenderer.material.SetFloat("_BlendTex01", Mathf.Lerp(0.0f, 1.0f, i));
                    yield return null;
                }
                break;
            case Message.SendMessage:
                for (float i = 0.0f; i < 1.0f; i += 0.1f)
                {
                    MessageUIRenderer.material.SetFloat("_BlendTex02", Mathf.Lerp(0.0f, 1.0f, i));
                    yield return null;
                }
                break;
        }
    }
}
