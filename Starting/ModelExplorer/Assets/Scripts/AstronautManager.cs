using Academy.HoloToolkit.Unity;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class AstronautManager : Singleton<AstronautManager>
{
    float expandAnimationCompletionTime;
    // Store a bool for whether our astronaut model is expanded or not.
    bool isModelExpanding = false;

    // KeywordRecognizer object.
    KeywordRecognizer keywordRecognizer;

    // Defines which function to call when a keyword is recognized.
    delegate void KeywordAction(PhraseRecognizedEventArgs args);
    Dictionary<string, KeywordAction> keywordCollection;

    void Start()
    {
        keywordCollection = new Dictionary<string, KeywordAction>();

        // Add keyword to start manipulation.
        keywordCollection.Add("Move Astronaut", MoveAstronautCommand);
        keywordCollection.Add("Expand Model", ExpandModelCommand);
        keywordCollection.Add("Reset Model", ResetModelCommand);

        // Initialize KeywordRecognizer with the previously added keywords.
        keywordRecognizer = new KeywordRecognizer(keywordCollection.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
        keywordRecognizer.Start();
    }

    void OnDestroy()
    {
        keywordRecognizer.Dispose();
    }

    private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        KeywordAction keywordAction;

        if (keywordCollection.TryGetValue(args.text, out keywordAction))
        {
            keywordAction.Invoke(args);
        }
    }

    private void MoveAstronautCommand(PhraseRecognizedEventArgs args)
    {
        GestureManager.Instance.Transition(GestureManager.Instance.ManipulationRecognizer);
    }

    private void ResetModelCommand(PhraseRecognizedEventArgs args)
    {
        // Reset local variables.
        isModelExpanding = false;

        // Disable the expanded model.
        ExpandModel.Instance.ExpandedModel.SetActive(false);

        // Enable the idle model.
        ExpandModel.Instance.gameObject.SetActive(true);

        // Enable the animators for the next time the model is expanded.
        Animator[] expandedAnimators = ExpandModel.Instance.ExpandedModel.GetComponentsInChildren<Animator>();
        foreach (Animator animator in expandedAnimators)
        {
            animator.enabled = true;
        }

        ExpandModel.Instance.Reset();
    }

    private void ExpandModelCommand(PhraseRecognizedEventArgs args)
    {
        // Swap out the current model for the expanded model.
        GameObject currentModel = ExpandModel.Instance.gameObject;

        ExpandModel.Instance.ExpandedModel.transform.position = currentModel.transform.position;
        ExpandModel.Instance.ExpandedModel.transform.rotation = currentModel.transform.rotation;
        ExpandModel.Instance.ExpandedModel.transform.localScale = currentModel.transform.localScale;

        currentModel.SetActive(false);
        ExpandModel.Instance.ExpandedModel.SetActive(true);

        // Play animation.  Ensure the Loop Time check box is disabled in the inspector for this animation to play it once.
        Animator[] expandedAnimators = ExpandModel.Instance.ExpandedModel.GetComponentsInChildren<Animator>();
        // Set local variables for disabling the animation.
        if (expandedAnimators.Length > 0)
        {
            expandAnimationCompletionTime = Time.realtimeSinceStartup + expandedAnimators[0].runtimeAnimatorController.animationClips[0].length * 0.9f;
        }

        // Set the expand model flag.
        isModelExpanding = true;

        ExpandModel.Instance.Expand();
    }

    public void Update()
    {
        if (isModelExpanding && Time.realtimeSinceStartup >= expandAnimationCompletionTime)
        {
            isModelExpanding = false;

            Animator[] expandedAnimators = ExpandModel.Instance.ExpandedModel.GetComponentsInChildren<Animator>();

            foreach (Animator animator in expandedAnimators)
            {
                animator.enabled = false;
            }
        }
    }
}