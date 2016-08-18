using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;

public class GrammarManager : MonoBehaviour
{
    [Tooltip("The file name (including filetype) of the SRGS file to use for recognition. This file must be in the StreamingAssets folder.")]
    public string SRGSFileName;

    [Tooltip("The circle GameObject to change based on the user's input.")]
    public GameObject Circle;
    [Tooltip("The triangle GameObject to change based on the user's input.")]
    public GameObject Triangle;
    [Tooltip("The square GameObject to change based on the user's input.")]
    public GameObject Square;

    [Tooltip("The Text field to display the recognition text.")]
    public Text MessageToUser;

    // The GrammarRecognizer to use in this sample.
    private GrammarRecognizer grammarRecognizer;

    // This Dictionary is used to map color names to a representation usable by Unity.
    private Dictionary<string, Color> colorLookup = new Dictionary<string, Color>
    {
        {"red", Color.red }, {"blue", Color.blue }, {"black", Color.black},
        {"brown", new Color(0.65f, 0.16f, 0.16f)}, {"green", Color.green}, {"cyan", Color.cyan},
        {"purple", new Color(0.5f, 0.0f, 0.5f)}, {"yellow", Color.yellow}, {"white", Color.white},
        {"orange", new Color(1.0f, 0.65f, 0.0f)}, {"gray", Color.gray}, {"magenta", Color.magenta}
    };

    void Start()
    {
        if (string.IsNullOrEmpty(SRGSFileName) || Circle == null || Triangle == null || Square == null || MessageToUser == null)
        {
            Debug.LogError("Please specify an SRGS file name in GrammarManager.cs on " + name + ".");
            Debug.LogError("Please check your GameObject settings in GrammarManager.cs on " + name + ".");
            return;
        }

        // Instantiate the GrammarRecognizer, passing in the path to the SRGS file in the StreamingAssets folder.
        try
        {
            grammarRecognizer = new GrammarRecognizer(Path.Combine(Application.streamingAssetsPath, SRGSFileName));
            grammarRecognizer.OnPhraseRecognized += GrammarRecognizer_OnPhraseRecognized;
            grammarRecognizer.Start();
        }
        catch
        {
            // If the file specified to the GrammarRecognizer doesn't exist, let the user know.
            MessageToUser.text = "Check the SRGS file name in the Inspector on GrammarManager.cs and that the file's in the StreamingAssets folder.";
            MessageToUser.fontSize = 12;
        }
    }

    private void GrammarRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        // We'll use this to build up the recognized message from each rule.
        StringBuilder messageHeard = new StringBuilder();

        // This array contains the results of the SRGS rules the recognizer heard.
        // In this case, we'll iterate through and get the colors associated with each of the three shapes.
        SemanticMeaning[] meanings = args.semanticMeanings;

        foreach (SemanticMeaning meaning in meanings)
        {
            // From our rules, each key (the shape) can only have one corresponding value (a color).
            // If a shape wasn't heard, it won't appear in our SemanticMeaning array.
            string shape = meaning.key.Trim();
            string color = meaning.values[0].Trim();

            messageHeard.Append(color + " " + shape + " ");

            // Query the Unity Color from our recognized string, then set the alpha to 0.25f to match our theme.
            Color newColor = GetColor(color.ToLower());
            newColor.a = 0.25f;

            // Each shape corresponds to a different GameObject in our scene.
            switch (shape.ToLower())
            {
                case "circle":
                    Renderer circleRenderer = Circle.GetComponent<Renderer>();
                    circleRenderer.material.SetColor("_fillColor", newColor);
                    circleRenderer.material.SetColor("_fillShadingColor", newColor);
                    break;
                case "triangle":
                    Renderer triangleRenderer = Triangle.GetComponent<Renderer>();
                    triangleRenderer.material.SetColor("_fillColor", newColor);
                    triangleRenderer.material.SetColor("_fillShadingColor", newColor);
                    break;
                case "square":
                    Renderer squareRenderer = Square.GetComponent<Renderer>();
                    squareRenderer.material.SetColor("_fillColor", newColor);
                    squareRenderer.material.SetColor("_fillShadingColor", newColor);
                    break;
            }
        }

        // Let the user know the message that was heard. The recognizer may not return the shapes in the same order they were spoken.
        MessageToUser.text = "Heard: " + messageHeard.ToString();
    }

    void OnDestroy()
    {
        if (grammarRecognizer != null)
        {
            StopGrammarRecognizer();
            grammarRecognizer.OnPhraseRecognized -= GrammarRecognizer_OnPhraseRecognized;
            grammarRecognizer.Dispose();
        }
    }

    /// <summary>
    /// Makes sure the GrammarRecognizer isn't running, then starts it if it isn't.
    /// </summary>
    public void StartGrammarRecognizer()
    {
        if (grammarRecognizer != null && !grammarRecognizer.IsRunning)
        {
            grammarRecognizer.Start();
        }
    }

    /// <summary>
    /// Makes sure the GrammarRecognizer is running, then stops it if it is.
    /// </summary>
    public void StopGrammarRecognizer()
    {
        if (grammarRecognizer != null && grammarRecognizer.IsRunning)
        {
            grammarRecognizer.Stop();
        }
    }

    /// <summary>
    /// Creates a color object from the passed in string.
    /// </summary>
    /// <param name="colorString">The name of the color as a string.</param>
    private Color GetColor(string colorString)
    {
        Color newColor = Color.clear;

        if (colorLookup.ContainsKey(colorString.ToLower()))
        {
            newColor = colorLookup[colorString.ToLower()];
        }

        return newColor;
    }
}