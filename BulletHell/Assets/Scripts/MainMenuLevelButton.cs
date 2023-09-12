using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuLevelButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _buttonText;
    [SerializeField] private Image _buttonIcon;

    public void SetupVisuals(Color iconColor, string buttonText)
    {
        _buttonIcon.color = iconColor;
        _buttonText.text = buttonText;
    }
}
