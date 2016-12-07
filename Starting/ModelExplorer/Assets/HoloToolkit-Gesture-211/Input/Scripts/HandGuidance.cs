using UnityEngine;
using UnityEngine.VR.WSA.Input;

namespace Academy.HoloToolkit.Unity
{
    /// <summary>
    /// Show a hand guidance indicator when the user's hand is close to leaving the camera's view.
    /// </summary>
    public class HandGuidance : Singleton<HandGuidance>
    {
        [Tooltip("GameObject to display when your hand is about to lose tracking.")]
        public GameObject HandGuidanceIndicator;
        private GameObject handGuidanceIndicatorGameObject = null;

        [Tooltip("Parent transform to place the indicators around. Leave this field empty to place the indicators relative to the user's gaze.")]
        public GameObject IndicatorParent;

        // Hand guidance score to start showing a hand indicator.
        // As the guidance score approaches 0, the hand is closer to being out of view.
        [Range(0.0f, 1.0f)]
        [Tooltip("When to start showing the Hand Guidance Indicator. 1 is out of view, 0 is centered in view.")]
        public float HandGuidanceThreshold = 0.5f;

        private Quaternion defaultHandGuidanceRotation;

        private uint? currentlyTrackedHand = null;

        void Start()
        {
            // Register for hand and finger events to know where your hand
            // is being tracked and what state it is in.
            InteractionManager.SourceLost += InteractionManager_SourceLost;
            InteractionManager.SourceUpdated += InteractionManager_SourceUpdated;
            InteractionManager.SourceReleased += InteractionManager_SourceReleased;

            if (HandGuidanceIndicator != null)
            {
                defaultHandGuidanceRotation = HandGuidanceIndicator.transform.rotation;
            }

            handGuidanceIndicatorGameObject = Instantiate(HandGuidanceIndicator);
            handGuidanceIndicatorGameObject.SetActive(false);

            if (IndicatorParent != null)
            {
                // Set handGuidanceIndicator's parent to be the Cursor.
                handGuidanceIndicatorGameObject.transform.parent = IndicatorParent.transform;
            }
        }

        /// <summary>
        /// If this hand is being tracked, display an indicator at the hand's current position.
        /// This indicator will show what direction the user should move their hand to increase their guidance score.
        /// </summary>
        /// <param name="hand">The hand being tracked.</param>
        private void ShowHandGuidanceIndicator(InteractionSourceState hand)
        {
            if (!currentlyTrackedHand.HasValue)
            {
                return;
            }

            if (handGuidanceIndicatorGameObject != null)
            {
                Vector3 position;
                Quaternion rotation;
                GetIndicatorPositionAndRotation(hand, out position, out rotation);

                handGuidanceIndicatorGameObject.transform.position = position;
                handGuidanceIndicatorGameObject.transform.rotation = rotation * defaultHandGuidanceRotation;
                handGuidanceIndicatorGameObject.SetActive(true);
            }
        }

        private void GetIndicatorPositionAndRotation(InteractionSourceState hand, out Vector3 position, out Quaternion rotation)
        {
            float maxDistanceFromCenter = 0.3f;
            float distanceFromCenter = (float)(hand.properties.sourceLossRisk * maxDistanceFromCenter);

            // Subtract direction from origin so that the indicator is between the hand and the origin.
            position = IndicatorParent.transform.position - hand.properties.sourceLossMitigationDirection * distanceFromCenter;
            rotation = Quaternion.LookRotation(Camera.main.transform.forward, hand.properties.sourceLossMitigationDirection);
        }

        private void HideHandGuidanceIndicator(InteractionSourceState hand)
        {
            if (!currentlyTrackedHand.HasValue)
            {
                return;
            }

            if (handGuidanceIndicatorGameObject != null)
            {
                handGuidanceIndicatorGameObject.SetActive(false);
            }
        }

        // Call these events from HandsManager. This fires when the hand is moved.
        private void InteractionManager_SourceUpdated(InteractionSourceState hand)
        {
            // Only display hand indicators when we have targeted an interactible and the hand is in a pressed state.
            if (!hand.pressed ||
                HandsManager.Instance.FocusedGameObject == null ||
                (HandsManager.Instance.FocusedGameObject != null &&
                HandsManager.Instance.FocusedGameObject.GetComponent<Interactible>() == null))
            {
                return;
            }

            // Only track a new hand if are not currently tracking a hand.
            if (!currentlyTrackedHand.HasValue)
            {
                currentlyTrackedHand = hand.source.id;
            }
            else if (currentlyTrackedHand.Value != hand.source.id)
            {
                // This hand is not the currently tracked hand, do not drawn a guidance indicator for this hand.
                return;
            }

            // Start showing an indicator to move your hand toward the center of the view.
            if (hand.properties.sourceLossRisk > HandGuidanceThreshold)
            {
                ShowHandGuidanceIndicator(hand);
            }
            else
            {
                HideHandGuidanceIndicator(hand);
            }
        }

        // This fires when the finger is released.
        private void InteractionManager_SourceReleased(InteractionSourceState hand)
        {
            RemoveTrackedHand(hand);
        }

        // This fires when the hand is lost.
        private void InteractionManager_SourceLost(InteractionSourceState hand)
        {
            if (!currentlyTrackedHand.HasValue || currentlyTrackedHand.Value != hand.source.id)
            {
                return;
            }

            RemoveTrackedHand(hand);
        }

        private void RemoveTrackedHand(InteractionSourceState hand)
        {
            if (currentlyTrackedHand.HasValue && currentlyTrackedHand.Value == hand.source.id)
            {
                handGuidanceIndicatorGameObject.SetActive(false);
                currentlyTrackedHand = null;
            }
        }

        void OnDestroy()
        {
            InteractionManager.SourceLost -= InteractionManager_SourceLost;
            InteractionManager.SourceUpdated -= InteractionManager_SourceUpdated;
            InteractionManager.SourceReleased -= InteractionManager_SourceReleased;
        }
    }
}