// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity.InputModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Academy
{
    /// <summary>
    /// Shows the list of available voice commands on the KeywordManager.cs script.
    /// </summary>
    public class DisplayActiveKeywords : MonoBehaviour
    {
        Text textComponent;
        string originalText = string.Empty;
        StringBuilder sb = new StringBuilder();

        SpeechInputHandler[] speechInputHandlers;
        Dictionary<string, UnityEvent> responsesLookup;

        void Start()
        {
            textComponent = gameObject.GetComponent<Text>();
            originalText = textComponent.text;

            // Find the KeywordManager scripts.
            speechInputHandlers = FindObjectsOfType<SpeechInputHandler>();
            if (speechInputHandlers == null)
            {
                Debug.LogError("Could not find SpeechInputHandler.cs anywhere.");
                return;
            }

            // Reset the text panel.
            sb.Length = 0;
            sb.AppendLine(originalText);

            // Ensure we display active commands on all keyword managers.
            foreach (SpeechInputHandler speechInputHandler in speechInputHandlers)
            {
                AddActiveKeywords(speechInputHandler);
            }

            textComponent.text = sb.ToString();
        }

        private void AddActiveKeywords(SpeechInputHandler speechInputHandler)
        {
            // Convert the struct array into a dictionary, with the keywords as the keys and the methods as the values.
            responsesLookup = speechInputHandler.Keywords.ToDictionary(
                keywordAndResponse => keywordAndResponse.Keyword,
                keywordAndResponse => keywordAndResponse.Response);

            // Find which keywords have wired up responses in the editor and display only those.
            foreach (string keyword in responsesLookup.Keys)
            {
                if (!String.IsNullOrEmpty(responsesLookup[keyword].GetPersistentMethodName(0)))
                {
                    sb.AppendLine(keyword);
                }
            }
        }
    }
}