using UnityEngine;

namespace Academy.HoloToolkit.Unity
{
    /// <summary>
    /// DirectionIndicator creates an indicator around the cursor showing
    /// what direction to turn to find this GameObject.
    /// </summary>
    public class DirectionIndicator : MonoBehaviour
    {
        [Tooltip("The Cursor object an on-cursor direction indicator will attach to.")]
        public GameObject Cursor;

        [Tooltip("Model to display on-cursor direction to the object this script is attached to.")]
        public GameObject DirectionIndicatorObject;

        [Tooltip("Color to shade the direction indicator.")]
        public Color DirectionIndicatorColor = Color.blue;

        [Tooltip("Allowable percentage inside the holographic frame to continue to show a directional indicator.")]
        [Range(-0.3f, 0.3f)]
        public float TitleSafeFactor = 0.1f;

        // The default rotation of the cursor direction indicator.
        private Quaternion directionIndicatorDefaultRotation = Quaternion.identity;

        // Cache the MeshRenderer for the on-cursor indicator since it will be enabled and disabled frequently.
        private MeshRenderer directionIndicatorRenderer;

        // Check if the cursor direction indicator is visible.
        private bool isDirectionIndicatorVisible;

        public void Awake()
        {
            if (DirectionIndicatorObject == null)
            {
                return;
            }

            // Instantiate the direction indicator.
            DirectionIndicatorObject = InstantiateDirectionIndicator(DirectionIndicatorObject);
            directionIndicatorDefaultRotation = DirectionIndicatorObject.transform.rotation;

            directionIndicatorRenderer = DirectionIndicatorObject.GetComponent<MeshRenderer>();
        }

        public void OnDestroy()
        {
            GameObject.Destroy(DirectionIndicatorObject);
        }

        private GameObject InstantiateDirectionIndicator(GameObject directionIndicator)
        {
            GameObject indicator = Instantiate(directionIndicator);

            MeshRenderer indicatorRenderer = indicator.GetComponent<MeshRenderer>();

            if (indicatorRenderer == null)
            {
                // The Direction Indicator must have a MeshRenderer so it can give visual feedback to the user which way to look.
                // Add one if there wasn't one.
                indicatorRenderer = indicator.AddComponent<MeshRenderer>();
            }

            // Start with the indicator disabled.
            indicatorRenderer.enabled = false;

            // Remove any colliders and rigidbodies so the indicators do not interfere with Unity's physics system.
            foreach (Collider collider in indicator.GetComponents<Collider>())
            {
                Destroy(collider);
            }

            foreach (Rigidbody rigidBody in indicator.GetComponents<Rigidbody>())
            {
                Destroy(rigidBody);
            }

            Material indicatorMaterial = indicatorRenderer.material;
            indicatorMaterial.color = DirectionIndicatorColor;
            indicatorMaterial.SetColor("_TintColor", DirectionIndicatorColor);

            // Make this indicator a child of the targeted GameObject.
            indicator.transform.SetParent(gameObject.transform);

            return indicator;
        }

        public void Update()
        {
            if (DirectionIndicatorObject == null)
            {
                return;
            }

            // Direction from the Main Camera to this script's parent gameObject.
            Vector3 camToObjectDirection = gameObject.transform.position - Camera.main.transform.position;
            camToObjectDirection.Normalize();

            // The cursor indicator should only be visible if the target is not visible.
            isDirectionIndicatorVisible = !IsTargetVisible();
            directionIndicatorRenderer.enabled = isDirectionIndicatorVisible;

            if (isDirectionIndicatorVisible)
            {
                Vector3 position;
                Quaternion rotation;
                GetDirectionIndicatorPositionAndRotation(
                    camToObjectDirection,
                    out position,
                    out rotation);

                DirectionIndicatorObject.transform.position = position;
                DirectionIndicatorObject.transform.rotation = rotation;
            }
        }

        private bool IsTargetVisible()
        {
            // This will return true if the target's mesh is within the Main Camera's view frustums.
            Vector3 targetViewportPosition = Camera.main.WorldToViewportPoint(gameObject.transform.position);
            return (targetViewportPosition.x > TitleSafeFactor && targetViewportPosition.x < 1 - TitleSafeFactor &&
                targetViewportPosition.y > TitleSafeFactor && targetViewportPosition.y < 1 - TitleSafeFactor &&
                targetViewportPosition.z > 0);
        }

        private void GetDirectionIndicatorPositionAndRotation(
            Vector3 camToObjectDirection,
            out Vector3 position,
            out Quaternion rotation)
        {
            // Find position:
            // Use this value to decrease the distance from the cursor center an object is rendered to keep it in view.
            float metersFromCursor = 0.3f;

            // Save the cursor transform position in a variable.
            Vector3 origin = Cursor.transform.position;

            // Project the camera to target direction onto the screen plane.
            Vector3 cursorIndicatorDirection = Vector3.ProjectOnPlane(camToObjectDirection, -1 * Camera.main.transform.forward);
            cursorIndicatorDirection.Normalize();

            // If the direction is 0, set the direction to the right.
            // This will only happen if the camera is facing directly away from the target.
            if (cursorIndicatorDirection == Vector3.zero)
            {
                cursorIndicatorDirection = Camera.main.transform.right;
            }

            // The final position is translated from the center of the screen along this direction vector.
            position = origin + cursorIndicatorDirection * metersFromCursor;

            // Find the rotation from the facing direction to the target object.
            rotation = Quaternion.LookRotation(
                Camera.main.transform.forward,
                cursorIndicatorDirection) * directionIndicatorDefaultRotation;
        }
    }
}