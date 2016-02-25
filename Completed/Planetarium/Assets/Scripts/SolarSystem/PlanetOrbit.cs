using UnityEngine;
using System.Collections;

/// <summary>
/// Controls a planet's movement around the sun (years) 
/// and rotation around its axis (days) and distance to 
/// maintain from the sun.
/// </summary>
public class PlanetOrbit : MonoBehaviour
{
    /// <summary>
    /// Stores length of a year.
    /// </summary>
    float orbitPeriod;

    /// <summary>
    /// Stores length of a day.
    /// </summary>
    float rotatePeriod;

    /// <summary>
    /// Stores distance from the sun.
    /// </summary>
    float distance;

    /// <summary>
    /// Keeps a reference to the sun.
    /// </summary>
    GameObject sun;

	// Use this for initialization
	void Start ()
    {
        // Collect our variables.
        sun = SunScript.Instance.transform.gameObject;
        orbitPeriod = SunScript.Instance.GetYearTime(this.name);
        distance = SunScript.Instance.GetDistance(this.name); 
        rotatePeriod = SunScript.Instance.GetDayTime(this.name);
	}
	
	// Update is called once per frame
	void Update ()
    {
        // Discover where the planet should be placed. 
        // periodRat informs how far the planet is through its year.
        // Magic number added to time is to make it so the planets aren't
        // aligned when the script starts.
        float periodRat = (Time.time+123450) / orbitPeriod;
        float xPos = Mathf.Cos(periodRat);
        float yPos = Mathf.Sin(periodRat);
        this.transform.localPosition = sun.transform.localPosition + distance * new Vector3(xPos, 0, yPos);

        // Rotate the planet based on how far along through its day it is.
        this.transform.Rotate(0, 0, Time.deltaTime /-rotatePeriod);
    }
}
