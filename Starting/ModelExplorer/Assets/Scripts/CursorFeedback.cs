using HoloToolkit.Unity.InputModule;
using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// CursorFeedback class takes GameObjects to give cursor feedback
    /// to users based on different states.
    /// </summary>
    public class CursorFeedback : MonoBehaviour
    {
        [Tooltip("Drag the GameObject to display when a scroll enabled Interactible is detected.")]
        [SerializeField]
        private GameObject scrollDetectedGameObject;

        [Tooltip("Drag the GameObject to display when a pathing enabled Interactible is detected.")]
        [SerializeField]
        private GameObject pathingDetectedGameObject;

        private InputModule.Cursor cursor;

        private bool IsNavigationFocused
        {
            get
            {
                GameObject targeted = cursor.GetTargetedObject();
                if (targeted != null)
                {
                    GestureAction gestureAction = targeted.GetComponent<GestureAction>();
                    if (gestureAction != null)
                    {
                        return gestureAction.IsNavigationEnabled;
                    }
                    else
                    {
                        gestureAction = targeted.transform.root.GetComponent<GestureAction>();
                        if (gestureAction != null)
                        {
                            return gestureAction.IsNavigationEnabled;
                        }
                    }
                }

                return false;
            }
        }

        private bool IsManipulationFocused
        {
            get
            {
                GameObject targeted = cursor.GetTargetedObject();
                if (targeted != null)
                {
                    GestureAction gestureAction = targeted.GetComponent<GestureAction>();
                    if (gestureAction != null)
                    {
                        return !gestureAction.IsNavigationEnabled;
                    }
                    else
                    {
                        gestureAction = targeted.transform.root.GetComponent<GestureAction>();
                        if (gestureAction != null)
                        {
                            return !gestureAction.IsNavigationEnabled;
                        }
                    }
                }

                return false;
            }
        }

        private void Awake()
        {
            cursor = GetComponent<InputModule.Cursor>();
        }

        private void Update()
        {
            UpdatePathDetectedState();

            UpdateScrollDetectedState();
        }

        private void UpdatePathDetectedState()
        {
            if (pathingDetectedGameObject == null)
            {
                return;
            }

            if (!IsManipulationFocused)
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

            if (!IsNavigationFocused)
            {
                scrollDetectedGameObject.SetActive(false);
                return;
            }

            scrollDetectedGameObject.SetActive(true);
        }
    }
}