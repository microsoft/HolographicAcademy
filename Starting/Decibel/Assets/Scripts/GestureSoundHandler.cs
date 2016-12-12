using UnityEngine;

namespace Academy.HoloToolkit.Unity
{
    public class GestureSoundHandler : MonoBehaviour
    {
        public AudioClip NavigationStartedClip;
        public AudioClip NavigationUpdatedClip;

        public AudioClip[] AudioClips
        {
            get; private set;
        }

	    private void Awake()
        {
            // Make it super convenient for designers to specify sounds in the UI 
            // and developers to access specific sounds in code.
            AudioClips = new AudioClip[(int)GestureSoundManager.GestureTypes.Count];
            AudioClips[(int)GestureSoundManager.GestureTypes.NavigationStarted] = NavigationStartedClip;
            AudioClips[(int)GestureSoundManager.GestureTypes.NavigationUpdated] = NavigationUpdatedClip;
	    }
    }
}