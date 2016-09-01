using System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

namespace WeatherApp
{
    public sealed partial class FlatPage : Page
    {
        private App app = Application.Current as App;

        public FlatPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// This method passes data from the flat XAML page to the Unity page. It also navigates to the Unity exclusive main view.
        /// </summary>
        private async void WeatherBtn_Click(object sender, RoutedEventArgs e)
        {
            if (app.MainPageRef != null)
            {
                Button button = sender as Button;

                if (button != null)
                {
                    TextBlock tb = button.Content as TextBlock;
                    
                    if (tb != null)
                    {
                        app.MainPageRef.LoadWeather((tb.Inlines[6] as Run).Text, (tb.Inlines[4] as Run).Text, (tb.Inlines[0] as Run).Text);
                    }
                }
            }

            if (app.MainViewId != 0)
            {
                await ApplicationViewSwitcher.SwitchAsync(app.MainViewId);
            }
        }
    }
}