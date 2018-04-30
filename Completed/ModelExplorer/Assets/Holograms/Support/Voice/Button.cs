// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.Events;
using HoloToolkit.Unity.InputModule;

namespace Academy
{
    /// <summary>
    /// Button keeps track of various methods and textures for each media button on the communicator.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class Button : MonoBehaviour, IFocusable, IInputClickHandler
    {
        [Tooltip("The GameObject to be displayed to 'highlight' the button on gaze.")]
        [SerializeField]
        private GameObject highlight;

        [Tooltip("Set the initial state of the button.")]
        [SerializeField]
        private State startingState;

        [Tooltip("The method to be called on click.")]
        [SerializeField]
        private UnityEvent method;

        private Renderer buttonRenderer;
        private State currentState = State.Inactive;
        private AudioSource clickAudio;

        public enum State { Inactive, Active, Gazed, Selected };

        void Awake()
        {
            buttonRenderer = GetComponent<Renderer>();
            clickAudio = GetComponent<AudioSource>();

            ChangeButtonState(startingState);
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
                highlight.SetActive(false);
            }
        }

        void IFocusable.OnFocusEnter()
        {
            if (IsOn())
            {
                ChangeButtonState(State.Gazed);
                highlight.SetActive(true);
            }
        }

        void IFocusable.OnFocusExit()
        {
            if (IsOn())
            {
                ChangeButtonState(State.Active);
                highlight.SetActive(false);
            }
        }

        void IInputClickHandler.OnInputClicked(InputClickedEventData eventData)
        {
            if (IsOn())
            {
                ChangeButtonState(State.Selected);
                clickAudio.Play();
                ChangeButtonState(State.Active);
                method.Invoke();
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
}