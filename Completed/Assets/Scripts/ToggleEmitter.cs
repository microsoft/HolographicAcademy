using UnityEngine;
using HoloToolkit.Unity;

public class ToggleEmitter : MonoBehaviour
{
    private int timesSelected = 0;

    private void OnSelected()
    {
        this.timesSelected++;
        if (this.timesSelected == 1)
        {
            //Call "PlayEvent" on the UAudioManager using the "SimpleSound" event on this GameObject
            UAudioManager.Instance.PlayEvent("SimpleSound", this.gameObject);
        }
        else if (this.timesSelected == 2)
        {
            //Call "StopEvent" on the UAudioManager using the "SimpleSound" event on this GameObject
            UAudioManager.Instance.StopEvent("SimpleSound", this.gameObject);
            //Call "PlayEvent" on the UAudioManager using the "SpatialSound" event on this GameObject
            UAudioManager.Instance.PlayEvent("SpatialSound", this.gameObject);
        }
        else
        {
            this.timesSelected = 0;
            //Call "StopEvent" on the UAudioManager using the "SpatialSound" event on this GameObject
            UAudioManager.Instance.StopEvent("SpatialSound", this.gameObject);
        }
    }
}