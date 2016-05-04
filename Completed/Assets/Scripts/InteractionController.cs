using HoloToolkit.Unity;
using UnityEngine;
using UnityEngine.VR.WSA.Input;
using UnityEngine.Windows.Speech;

public class InteractionController : MonoBehaviour
{
    [SerializeField]
    private bool playHandEmitter = false;

    [SerializeField]
    private Transform handEmitter;

    private KeywordRecognizer keywords;
    private MarcoPoloEmitter astronaut;
    private SurroundController surround;
    private InteractionManager interactionManager;

    #region Unity Functions

    private void Start()
    {
        InteractionManager.SourceDetected += InteractionManager_SourceDetected;
        InteractionManager.SourceLost += InteractionManager_SourceLost;
        InteractionManager.SourcePressed += InteractionManager_SourcePressed;
        InteractionManager.SourceReleased += InteractionManager_SourceReleased;
        string[] myKeywords = new string[] { "Start", "Standard", "Spatial" };
        this.keywords = new KeywordRecognizer(myKeywords);
        this.keywords.OnPhraseRecognized += OnKeywordRecognized;
        this.keywords.Start();
        this.astronaut = FindObjectOfType<MarcoPoloEmitter>();
        this.surround = FindObjectOfType<SurroundController>();
    }

    private void Update()
    {
#if UNITY_EDITOR
        UnityControls();
#endif
    }

    private void OnDestroy()
    {
        this.keywords.Dispose();
        this.keywords = null;
    }

    #endregion Unity Functions

    #region Private Functions

    private void UnityControls()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            InteractionManager_SourcePressed(new InteractionSourceState());
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            StartMusic();
            StartAstronaut();
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            SetStandard();
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            SetSpatial();
        }
        this.transform.Translate(Input.GetAxis("Horizontal") * Time.deltaTime, 0, Input.GetAxis("Vertical") * Time.deltaTime);
    }

    private void SendTargetMessage(string message)
    {
        RaycastHit tempHit;
        if (Physics.Raycast(this.transform.position, this.transform.forward, out tempHit))
        {
            tempHit.collider.gameObject.SendMessage(message);
        }
    }

    private void StartAstronaut()
    {
        if (this.astronaut == null)
        {
            return;
        }
        this.astronaut.StartSounds();
    }

    private void StartMusic()
    {
        if (this.surround == null)
        {
            return;
        }
        this.surround.StartEmitters();
    }

    private void SetSpatial()
    {
        if (this.surround == null)
        {
            return;
        }
        this.surround.DoSpatial();
    }

    private void SetStandard()
    {
        if (this.surround == null)
        {
            return;
        }
        this.surround.DoStandard();
    }

    #endregion Private Functions

    private void OnKeywordRecognized(PhraseRecognizedEventArgs args)
    {
        if (args.text == "Start")
        {
            StartAstronaut();
            StartMusic();
        }
        else if (args.text == "Standard")
        {
            SetStandard();
        }
        else if (args.text == "Spatial")
        {
            SetSpatial();
        }
    }

    #region Hand Messages

    private void InteractionManager_SourceDetected(InteractionSourceState state)
    {
        if (this.playHandEmitter)
        {
            UAudioManager.Instance.PlayEvent("HandDetected", this.handEmitter.gameObject);
            UAudioManager.Instance.PlayEvent("HandLoop", this.handEmitter.gameObject);
        }
    }

    private void InteractionManager_SourceLost(InteractionSourceState state)
    {
        if (this.playHandEmitter)
        {
            UAudioManager.Instance.PlayEvent("HandLost", this.handEmitter.gameObject);
            UAudioManager.Instance.StopEvent("HandLoop", this.handEmitter.gameObject);
        }
    }

    private void InteractionManager_SourcePressed(InteractionSourceState state)
    {
        SendTargetMessage("OnSelected");
        if (this.playHandEmitter)
        {
            UAudioManager.Instance.PlayEvent("FingerPressed", this.handEmitter.gameObject);
        }
    }

    private void InteractionManager_SourceReleased(InteractionSourceState state)
    {
        if (this.playHandEmitter)
        {
            UAudioManager.Instance.PlayEvent("FingerReleased", this.handEmitter.gameObject);
        }
    }

    #endregion Hand Messages
}