using UnityEngine;

/// <summary>
/// Rotates an asteroid field;
/// </summary>
public class RotateAsteroids : MonoBehaviour
{
    /// <summary>
    /// Controls how fast the asteroid belt will rotate.
    /// </summary>
    public float period;
	
	// Update is called once per frame
	void Update ()
    {
        float periodRat = (Time.time) / period;
        this.transform.Rotate(0, 0, -periodRat*Time.deltaTime*0.1f);
	}
}
