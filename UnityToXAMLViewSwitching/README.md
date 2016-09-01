# Exclusive Unity view to XAML view switching sample
This sample shows how to add a new flat view to your Unity app.

This view will display as a flat XAML panel in your world, alongside other app panels you may have placed. You can swap between this view and the exclusive holographic view, optionally passing data between them.

## Steps for use
Your project must be built with the **XAML** UWP Build Type. All files in these steps can be found in the AdditionalFiles folder. If you're adding these to your own project, make sure to update the namespace at the top of each file.

Before building in Unity (the first step is already done in the WeatherApp sample):

1. Add **AppViewSwitcher.cs** to your Unity project in the **Scripts** folder and add the script to an object in your scene.

2. Build in Unity with the **XAML** UWP Build Type.

After building in Unity (you'll need to do this):

1. Add the partial classes **MainPageCustom.xaml.cs** and **AppCustom.xaml.cs**, as well as the new page **FlatPage.xaml** and **FlatPage.xaml.cs**, to your project. As mentioned above, these files are located in the AdditionalFiles folder. You can do this by right-clicking the generating C# project in Visual Studio, clicking **Add** in the menu, then clicking **Existing Item**. Navigate to the **UnityToXAMLViewSwitching\AdditionalFiles** folder to add these four files.

2. In **MainPage.xaml.cs**, in the constructor, before the line `bool isWindowsHolographic = false;`, add `SetUpFlatView();`.

3. In **App.xaml.cs**, in the `InitializeUnity` method, after the line `rootFrame.Navigate(typeof(MainPage));`, add   
```C#
MainPageRef = rootFrame.Content as MainPage;
MainViewId = ApplicationView.GetForCurrentView().Id;
```

## Notable Code

* **MainPageCustom.cs**
    * The `SetUpFlatView` method shows how to set up the new XAML view and navigate to the desired content. It also sets the delegate for the **AppViewSwitcher** Unity script to call (see below).
    * The `SwitchToFlat` method shows how to switch between the two views, in this case from Unity to XAML.
    * The `LoadWeather` method shows how to call a method in a Unity script from XAML code. This is one of the steps for passing data between the two views.
* **AppViewSwitcher.cs**
    * The `OnSelect` method shows how to call a XAML page method from a Unity script. This delegate is set in **MainPageCustom.cs**'s `SetUpFlatView` method.
* **FlatPage.xaml.cs**
    * The `WeatherBtn_Click` method shows how to switch between the two views, in this case from XAML to Unity. It also passes data into the Unity view by calling `LoadWeather`.