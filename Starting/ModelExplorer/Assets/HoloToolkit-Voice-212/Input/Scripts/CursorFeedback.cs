using UnityEngine;

namespace Academy.HoloToolkit.Unity
{
    /// <summary>
    /// CursorFeedback class takes GameObjects to give cursor feedback
    /// to users based on different states.
    /// </summary>
    public class CursorFeedback : MonoBehaviour
    {
        [Tooltip("Drag a prefab object to display when a hand is detected.")]
        public GameObject HandDetectedAsset;
        private GameObject handDetectedGameObject;

        [Tooltip("Drag a prefab object to display when a scroll enabled Interactible is detected.")]
        public GameObject ScrollDetectedAsset;
        private GameObject scrollDetectedGameObject;

        [Tooltip("Drag a prefab object to display when a pathing enabled Interactible is detected.")]
        public GameObject PathingDetectedAsset;
        private GameObject pathingDetectedGameObject;

        [Tooltip("Drag a prefab object to display when a voice command is detected.")]
        public GameObject VoiceCommandDetectedAsset;
        private GameObject voiceCommandDetectedGameObject;

        [Tooltip("Drag a prefab object to parent the feedback assets.")]
        public GameObject FeedbackParent;

        private Interactible FocusedInteractible
        {
            get
            {
                if (InteractibleManager.Instance.FocusedGameObject != null)
                {
                    return InteractibleManager.Instance.FocusedGameObject.GetComponent<Interactible>();
                }

                return null;
            }
        }

        void Awake()
        {
            if (HandDetectedAsset != null)
            {
                handDetectedGameObject = InstantiatePrefab(HandDetectedAsset);
            }

            if (ScrollDetectedAsset != null)
            {
                scrollDetectedGameObject = InstantiatePrefab(ScrollDetectedAsset);
            }

            if (PathingDetectedAsset != null)
            {
                pathingDetectedGameObject = InstantiatePrefab(PathingDetectedAsset);
            }

            if (VoiceCommandDetectedAsset != null)
            {
                voiceCommandDetectedGameObject = InstantiatePrefab(VoiceCommandDetectedAsset);
            }
        }

        private GameObject InstantiatePrefab(GameObject inputPrefab)
        {
            GameObject instantiatedPrefab = null;

            if (inputPrefab != null && FeedbackParent != null)
            {
                instantiatedPrefab = GameObject.Instantiate(inputPrefab);
                // Assign parent to be the FeedbackParent
                // so that feedback assets move and rotate with this parent.
                instantiatedPrefab.transform.parent = FeedbackParent.transform;

                // Set starting state of the prefab's GameObject to be inactive.
                instantiatedPrefab.gameObject.SetActive(false);
            }

            return instantiatedPrefab;
        }

        void Update()
        {
            UpdateHandDetectedState();

            UpdatePathDetectedState();

            UpdateScrollDetectedState();

            UpdateVoiceCommandDetectedState();
        }

        private void UpdateHandDetectedState()
        {
            if (handDetectedGameObject == null || CursorManager.Instance == null)
            {
                return;
            }

            handDetectedGameObject.SetActive(HandsManager.Instance.HandDetected);
        }

        private void UpdatePathDetectedState()
        {
            if (pathingDetectedGameObject == null)
            {
                return;
            }

            if (CursorManager.Instance == null || FocusedInteractible == null ||
                GestureManager.Instance.ActiveRecognizer != GestureManager.Instance.ManipulationRecognizer)
            {
                pathingDetectedGameObject.SetActive(false);
                return;
            }

            pathingDetectedGameObject.SetActive(true);
        }

        private void UpdateScrollDetectedState()
        {
            if (scrollDetectedGameObject == null)
            {
                return;
            }

            if (CursorManager.Instance == null || FocusedInteractible == null ||
                GestureManager.Instance.ActiveRecognizer != GestureManager.Instance.NavigationRecognizer)
            {
                scrollDetectedGameObject.SetActive(false);
                return;
            }

            scrollDetectedGameObject.SetActive(true);
        }

        private void UpdateVoiceCommandDetectedState()
        {
            if (voiceCommandDetectedGameObject == null)
            {
                return;
            }

            if (CursorManager.Instance == null || InteractibleManager.Instance.FocusedGameObject == null)
            {
                voiceCommandDetectedGameObject.SetActive(false);
                return;
            }

            bool voiceCommandAvailable = InteractibleManager.Instance.FocusedGameObject.tag.Equals("VoiceCommand") && !AstronautWatch.Instance.CommunicatorOpen;

            // Show cursor feedback that voice command is available.
            voiceCommandDetectedGameObject.SetActive(voiceCommandAvailable);
        }
    }
}