using UnityEngine;
using UnityEngine.UI;

public class VoiceTooltip : MonoBehaviour
{
    [Tooltip("The message stating which voice command is available on this tooltip.")]
    public Text UserMessage;

    private Color startingColor;
    private Color commandHeardColor = new Color(0.33f, 0.14f, 0.93f, 1.0f);

    void Start()
    {
        startingColor = UserMessage.color;
    }

    public void VoiceCommandHeard()
    {
        UserMessage.color = commandHeardColor;
    }

    public void ResetTooltip()
    {
        UserMessage.color = startingColor;
    }
}
