using Academy.HoloToolkit.Unity;
using UnityEngine;

[RequireComponent(typeof(KeywordManager))]
public class AstronautWatch : Singleton<AstronautWatch>
{
    [Tooltip("Drag the Communicator prefab asset.")]
    public GameObject CommunicatorPrefab;
    private GameObject communicatorGameObject;

    [Tooltip("Drag the Message Received prefab asset.")]
    public GameObject MessagePrefab;
    private GameObject messageGameObject;

    [Tooltip("Drag the Voice Tooltip prefab asset.")]
    public GameObject OpenCommunicatorTooltip;
    private GameObject openCommunicatorTooltipGameObject;

    public AudioClip DismissSound;

    public bool CommunicatorOpen { get; private set; }

    private KeywordManager keywordManager;

    void Awake()
    {
        CommunicatorOpen = false;

        openCommunicatorTooltipGameObject = Instantiate(OpenCommunicatorTooltip);

        openCommunicatorTooltipGameObject.transform.position = new Vector3(
            gameObject.transform.position.x + 0.1f,
            gameObject.transform.position.y + 0.05f,
            gameObject.transform.position.z - 0.05f);

        openCommunicatorTooltipGameObject.transform.parent = gameObject.transform;
        openCommunicatorTooltipGameObject.SetActive(false);

        keywordManager = GetComponent<KeywordManager>();
    }

    public void OpenCommunicator()
    {
        // When a voice command is heard, change the text color on the tooltip.
        // This gives feedback to the user that the voice command has been heard.
        openCommunicatorTooltipGameObject.GetComponent<VoiceTooltip>().VoiceCommandHeard();

        CommunicatorOpen = true;

        communicatorGameObject = Instantiate(CommunicatorPrefab);

        communicatorGameObject.transform.position = transform.position;
        communicatorGameObject.transform.Translate(0.4f, 0.0f, 0.0f, Camera.main.transform);
    }

    public void CloseCommunicator()
    {
        CommunicatorOpen = false;

        GameObject soundPlayer = new GameObject("MessageSentSound");
        AudioSource soundSource = soundPlayer.AddComponent<AudioSource>();
        soundSource.clip = DismissSound;
        soundSource.Play();

        messageGameObject = (GameObject)Instantiate(MessagePrefab, communicatorGameObject.transform.position, MessagePrefab.transform.rotation);

        Destroy(communicatorGameObject);
        Destroy(messageGameObject, 1.0f);
        Destroy(soundPlayer, DismissSound.length);
    }

    void GazeEntered()
    {
        // If communicator is not open, show the voice command tooltip.
        if(!CommunicatorOpen)
        {
            openCommunicatorTooltipGameObject.SetActive(true);

            keywordManager.StartKeywordRecognizer();
        }
    }

    void GazeExited()
    {
        // Hide tooltip when user looks away.
        openCommunicatorTooltipGameObject.SetActive(false);

        keywordManager.StopKeywordRecognizer();

        // Reset tooltip to its original state.
        openCommunicatorTooltipGameObject.GetComponent<VoiceTooltip>().ResetTooltip();
    }
}