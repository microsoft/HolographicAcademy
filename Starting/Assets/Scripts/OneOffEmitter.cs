using HoloToolkit.Unity;
using UnityEngine;

public class OneOffEmitter : MonoBehaviour
{
    [SerializeField]
    private string audioEventName;

    private void OnSelected()
    {
        UAudioManager.Instance.PlayEvent(this.audioEventName, this.gameObject);
    }
}