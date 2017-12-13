using UnityEngine;

/// <summary>
/// InteractibleAction performs custom actions when you tap on the holograms.
/// </summary>
public abstract class InteractibleAction : MonoBehaviour
{
    public abstract void PerformAction();
}