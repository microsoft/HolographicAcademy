using System;
using UnityPlayer;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace WeatherApp
{
    public sealed partial class MainPage : Page
    {
        private App app = Application.Current as App;

        /// <summary>
        /// This method creates a new view for our flat XAML page. It also saves off the view ID so we can navigate to the new view later.
        /// Additionally, this sets a delegate in the Unity script AppViewSwitcher, which will allow it to call a method in this file from the Unity view.
        /// </summary>
        public async void SetUpFlatView()
        {
            if (app.FlatViewId == 0)
            {
                CoreApplicationView FlatPageView = CoreApplication.CreateNewView();
                await FlatPageView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    app.FlatViewId = ApplicationView.GetForCurrentView().Id;
                    Frame frame = new Frame();
                    frame.Navigate(typeof(FlatPage), null);
                    Window.Current.Content = frame;
                    Window.Current.Activate();
                });
            }

            AppViewSwitcher.NavigateToFlat = SwitchToFlat;
        }

        /// <summary>
        /// This switches from the Unity exclusive view to our XAML flat panel, if it exists. If not, we try to set the new view up again.
        /// </summary>
        public async void SwitchToFlat()
        {
            if (app.FlatViewId == 0)
            {
                SetUpFlatView();
            }

            if (app.FlatViewId != 0)
            {
                await ApplicationViewSwitcher.SwitchAsync(app.FlatViewId);
            }
        }


        /// <summary>
        /// This method shows how to call a Unity script method from a GameObject in the scene.
        /// This is called when we pass data between our views, from the XAML view to the Unity view.
        /// </summary>
        public void LoadWeather(string newWeather, string newTemperature, string newDay)
        {
            if (AppCallbacks.Instance.IsInitialized())
            {
                AppCallbacks.Instance.InvokeOnAppThread(() =>
                {
                    UnityEngine.GameObject go = UnityEngine.GameObject.Find("SceneManager");
                    if (go != null)
                    {
                        go.GetComponent<AppViewSwitcher>().LoadWeather(newWeather, newTemperature, newDay);
                    }
                    else
                    {
                        UnityEngine.Debug.Log("SceneManager not found, have you exported the correct scene?");
                    }
                }, false);
            }
        }
    }
}