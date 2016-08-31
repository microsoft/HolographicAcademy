using HoloToolkit.Unity;
using UnityEngine;
using UnityEngine.UI;

public class AppViewSwitcher : MonoBehaviour
{
    public Text TemperatureText;
    public Text DescriptionText;
    public Text DayText;
    public GameObject Sun;
    public GameObject Clouds;
    public GameObject PartlyCloudy;

    public delegate void NavigateToFlatDelegate();
    public static NavigateToFlatDelegate NavigateToFlat;

    void Start()
    {
        GestureManager.Instance.OverrideFocusedObject = gameObject;
    }

    public void LoadWeather(string newWeather, string newTemperature, string newDay)
    {
        transform.position = Camera.main.transform.position + (2 * Camera.main.transform.forward);
        Vector3 newRotation = new Vector3(0, Camera.main.transform.eulerAngles.y, 0);
        transform.eulerAngles = newRotation;

        TemperatureText.text = newTemperature;
        DescriptionText.text = newWeather;
        DayText.text = newDay;

        switch (newWeather.ToLower())
        {
            case "sunny":
                Clouds.SetActive(false);
                PartlyCloudy.SetActive(false);
                Sun.SetActive(true);
                break;
            case "cloudy":
                Sun.SetActive(false);
                PartlyCloudy.SetActive(false);
                Clouds.SetActive(true);
                break;
            case "partly cloudy":
                Sun.SetActive(false);
                Clouds.SetActive(false);
                PartlyCloudy.SetActive(true);
                break;
        }
    }

    public void OnSelect()
    {
        if (NavigateToFlat != null)
        {
#if WINDOWS_UWP
            UnityEngine.WSA.Application.InvokeOnUIThread(() =>
            {
                NavigateToFlat();
            }, false);
#endif
        }
    }
}