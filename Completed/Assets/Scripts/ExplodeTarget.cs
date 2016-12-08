using UnityEngine;
using HoloToolkit.Unity;
using HoloToolkit.Sharing;

public class ExplodeTarget : Singleton<ExplodeTarget>
{
    [Tooltip("Object to disable after the target explodes.")]
    public GameObject Target;

    [Tooltip("Object to enable after the target explodes.")]
    public GameObject Underworld;

    void Start()
    {
        // Attach ExplodingBlob to our target, so it will explode when hit by projectiles.
        this.transform.Find("EnergyHub/BlobOutside").gameObject.AddComponent<ExplodingBlob>();

        // If a user joins late, we need to reset the target.
        SharingSessionTracker.Instance.SessionJoined += Instance_SessionJoined;

        // Handles the ExplodeTarget message from the network.
        CustomMessages.Instance.MessageHandlers[CustomMessages.TestMessageID.ExplodeTarget] = this.OnExplodeTarget;
    }

    /// <summary>
    /// When a new user joins the session after the underworld is enabled,
    /// reset the target so that everyone is in the same game state.
    /// </summary>
    /// <param name="sender">sender</param>
    /// <param name="e">args</param>
    private void Instance_SessionJoined(object sender, SharingSessionTracker.SessionJoinedEventArgs e)
    {
        if (Underworld.activeSelf)
        {
            HologramPlacement.Instance.ResetStage();
        }
    }

    /// <summary>
    /// Disables target and spatial mapping after an explosion occurs, enables the underworld.
    /// </summary>
    public void OnExplode()
    {
        // Hide the target and show the underworld.
        Target.SetActive(false);
        Underworld.SetActive(true);
        Underworld.transform.localPosition = Target.transform.localPosition;

        // Disable spatial mapping so drones can fly out of the underworld and players can shoot projectiles inside.
        SpatialMappingManager.Instance.gameObject.SetActive(false);
    }

    /// <summary>
    /// When a remote system has triggered an explosion, we'll be notified here.
    /// </summary>
    void OnExplodeTarget(NetworkInMessage msg)
    {
        OnExplode();
    }
}