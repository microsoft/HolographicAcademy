using Academy.HoloToolkit.Unity;
using UnityEngine;

/// <summary>
/// InteractibleAction performs custom actions when you gaze at the holograms.
/// </summary>
public class InteractibleAction : MonoBehaviour
{
    [Tooltip("Drag the Tagalong prefab asset you want to display.")]
    public GameObject ObjectToTagAlong;

    void PerformTagAlong()
    {
        if (ObjectToTagAlong == null)
        {
            Debug.LogError("Please add a TagAlong object on " + name + ".");
            return;
        }

        // Recommend having only one tagalong.
        if (GameObject.Find("/Tagalong(Clone)") != null || GameObject.Find("/SRGSToolbox(Clone)") != null || GameObject.Find("/Communicator(Clone)") != null)
        {
            return;
        }

        GameObject instantiatedObjectToTagAlong = Instantiate(ObjectToTagAlong);

        instantiatedObjectToTagAlong.SetActive(true);
        instantiatedObjectToTagAlong.AddComponent<Billboard>();
        instantiatedObjectToTagAlong.AddComponent<SimpleTagalong>();
   }
}