using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Button keeps track of various methods and textures for each media button on the communicator.
/// </summary>

[RequireComponent(typeof(AudioSource))]
public class Button : MonoBehaviour
{
    [Tooltip("The GameObject to be displayed to 'highlight' the button on gaze.")]
    public GameObject Highlight;
    [Tooltip("Set the initial state of the button.")]
    public State StartingState;
    [Tooltip("The method to be called on click.")]
    public UnityEvent Method;

    private Renderer buttonRenderer;
    private State currentState = State.Inactive;
    private AudioSource clickAudio;

    public enum State { Inactive, Active, Gazed, Selected };

    void Awake()
    {
        buttonRenderer = GetComponent<Renderer>();
        clickAudio = GetComponent<AudioSource>();

        ChangeButtonState(StartingState);
    }

    public bool IsOn()
    {
        return currentState != State.Inactive;
    }

    public void SetActive(bool setOn)
    {
        if (setOn)
        {
            ChangeButtonState(State.Active);
        }
        else
        {
            ChangeButtonState(State.Inactive);
            Highlight.SetActive(false);
        }
    }

    void GazeEntered()
    {
        if (IsOn())
        {
            ChangeButtonState(State.Gazed);
            Highlight.SetActive(true);
        }
    }

    void GazeExited()
    {
        if (IsOn())
        {
            ChangeButtonState(State.Active);
            Highlight.SetActive(false);
        }
    }

    void OnSelect()
    {
        if (IsOn())
        {
            ChangeButtonState(State.Selected);
            clickAudio.Play();
            ChangeButtonState(State.Active);
            Method.Invoke();
        }
    }

    private void ChangeButtonState(State newState)
    {
        State oldState = currentState;
        currentState = newState;

        if (newState > oldState)
        {
            for (int j = (int)newState; j > (int)oldState; j--)
            {
                buttonRenderer.material.SetFloat("_BlendTex0" + j, 1.0f);
            }
        }
        else
        {
            for (int j = (int)oldState; j > (int)newState; j--)
            {
                buttonRenderer.material.SetFloat("_BlendTex0" + j, 0.0f);
            }
        }
    }
}
