using UnityEditor;

namespace Academy.HoloToolkit.Sharing
{
    public static class SharingMenu
    {
        [MenuItem("HoloToolkit-Sharing-240/Launch Sharing Service", false)]
        public static void LaunchSessionServer()
        {
            Utilities.ExternalProcess.FindAndLaunch(@"HoloToolkit-Sharing-240\Sharing\SharingService\SharingService.exe", @"-local");
        }        
    }
}