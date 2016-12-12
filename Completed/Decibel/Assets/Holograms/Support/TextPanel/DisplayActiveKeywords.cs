using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Academy.HoloToolkit.Unity;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Shows the list of available voice commands on the KeywordManager.cs script.
/// </summary>
public class DisplayActiveKeywords : MonoBehaviour
{
    Text textComponent;
    string originalText = string.Empty;
    StringBuilder sb = new StringBuilder();

    KeywordManager[] keywordManagers;
    Dictionary<string, UnityEvent> responsesLookup;

    void Start()
    {
        textComponent = this.gameObject.GetComponent<Text>();
        originalText = textComponent.text;

        // Find the KeywordManager scripts.
        keywordManagers = FindObjectsOfType<KeywordManager>();
        if (keywordManagers == null)
        {
            Debug.LogError("Could not find KeywordManager.cs anywhere.");
            return;
        }

        // Reset the text panel.
        sb.Length = 0;
        sb.AppendLine(originalText);

        // Ensure we display active commands on all keyword managers.
        foreach (KeywordManager keywordManager in keywordManagers)
        {
            AddActiveKeywords(keywordManager);
        }

        textComponent.text = sb.ToString();
    }

    private void AddActiveKeywords(KeywordManager keywordManager)
    {
        // Convert the struct array into a dictionary, with the keywords as the keys and the methods as the values.
        responsesLookup = keywordManager.KeywordsAndResponses.ToDictionary(
            keywordAndResponse => keywordAndResponse.Keyword,
            keywordAndResponse => keywordAndResponse.Response);

        // Find which keywords have wired up responses in the editor and display only those.
        foreach (string keyword in responsesLookup.Keys)
        {
            if (responsesLookup[keyword].GetPersistentEventCount() != 0)
            {
                sb.AppendLine(keyword);
            }
        }        
    }
}
