using System.Collections.Generic;
using UnityEngine;
using Academy.HoloToolkit.Unity;

/// <summary>
/// This is a helper script to store data about various planets in our 
/// solar system. 
/// </summary>
public class SunScript : Singleton<SunScript>
{
    const float DaysInAYear = 365.25f;
    const float HoursInADay = 24.0f;

    /// <summary>
    /// A struct to hold information we need about planets.
    /// </summary>
    struct PlanetaryData
    {
        public float yearInEarthYears { get; set; }
        public float dayInEarthYears { get; set; }
        public float distanceFromSun { get; set; }
    }

    /// <summary>
    /// A dictionary to store the data for the planets.
    /// </summary>
    private Dictionary<string, PlanetaryData> PlanetData = new Dictionary<string, PlanetaryData>()
    {
        { "mercury", new PlanetaryData { yearInEarthYears = 87.96f/DaysInAYear,     dayInEarthYears = 58.7f / DaysInAYear,                 distanceFromSun = 0.39f } },
        { "venus",   new PlanetaryData { yearInEarthYears = 224.68f/DaysInAYear,    dayInEarthYears = 243.0f/DaysInAYear,                  distanceFromSun = 0.723f } },
        { "earth",   new PlanetaryData { yearInEarthYears = 1.0f,                   dayInEarthYears = 1.0f/DaysInAYear,                    distanceFromSun = 1.0f } },
        { "mars",    new PlanetaryData { yearInEarthYears = 1.88f,                  dayInEarthYears = 686.98f/DaysInAYear,                 distanceFromSun = 1.54f } },
        { "jupiter", new PlanetaryData { yearInEarthYears = 11.862f,                dayInEarthYears = 9.84f / (DaysInAYear * HoursInADay), distanceFromSun = 3.203f } },
        { "saturn",  new PlanetaryData { yearInEarthYears = 29.456f,                dayInEarthYears = 10.2f / (DaysInAYear * HoursInADay), distanceFromSun = 4.539f } },
        { "uranaus", new PlanetaryData { yearInEarthYears = 84.07f,                 dayInEarthYears = 17.9f / (DaysInAYear * HoursInADay), distanceFromSun = 5.18f } },
        { "neptune", new PlanetaryData { yearInEarthYears = 164.81f,                dayInEarthYears = 19.1f / (DaysInAYear * HoursInADay), distanceFromSun = 6.06f } },
        { "pluto",   new PlanetaryData { yearInEarthYears = 247.7f,                 dayInEarthYears = 6.39f/DaysInAYear,                   distanceFromSun = 7.53f } }
    };

    /// <summary>
    /// How long it takes for the Earth to rotate around the sun.
    /// </summary>
    [Tooltip("How long it takes for the Earth to rotate around the sun.")]
    public float EarthYear = 10;

    /// <summary>
    /// The distance the Earth should be from the sun.
    /// </summary>
    [Tooltip("The distance the Earth should be from the sun.")]
    public float OneAUInMeters = 0.15f;

    /// <summary>
    /// Gets the time it takes for a planet to rotate around the sun.
    /// </summary>
    /// <param name="Planet">The planet to look up.</param>
    /// <returns>How long a year is for the planet.</returns>
    public float GetYearTime(string Planet)
    {
        // Initialize to an invalid value;
        float retval = 10000.0f;

        string planetName = Planet.ToLower();
        if (PlanetData.ContainsKey(planetName))
        {
            retval = PlanetData[planetName].yearInEarthYears;
        }
        else
        {
            Debug.Log("Invalid planet name " + planetName);
        }

        return retval * EarthYear;
    }

    /// <summary>
    /// Gets the time a planet takes to revolve around its axis.
    /// </summary>
    /// <param name="Planet">The planet to look up.</param>
    /// <returns>The length of a day for the planet.</returns>
    public float GetDayTime(string Planet)
    {
        // Initialize to an invalid value;
        float retval = 10000.0f;

        string planetName = Planet.ToLower();
        if (PlanetData.ContainsKey(planetName))
        {
            retval = PlanetData[planetName].dayInEarthYears;
        }
        else
        {
            Debug.Log("Invalid planet name " + planetName);
        }

        return retval * EarthYear;
    }

    /// <summary>
    /// Gets the distance the planet should be from the Sun.  
    /// This is not to scale.
    /// </summary>
    /// <param name="Planet">The planet to look up.</param>
    /// <returns>The distance to put the planet from the sun.</returns>
    public float GetDistance(string Planet)
    {
        // Initialize to an invalid value;
        float retval = 10000.0f;

        string planetName = Planet.ToLower();
        if (PlanetData.ContainsKey(planetName))
        {
            retval = PlanetData[planetName].distanceFromSun;
        }
        else
        {
            Debug.Log("Invalid planet name " + planetName);
        }

        return retval * OneAUInMeters;
    }
}
