using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuLevelButton : MonoBehaviour
{
    #region Serialized Fields

    [SerializeField] private TextMeshProUGUI _buttonText;
    [SerializeField] private Image _buttonIcon;

    #endregion

    #region Public Fields

    [HideInInspector] public int boundLevelSettingIndex;

    #endregion

    #region Public Methods

    public void SetupVisuals(Color iconColor, string buttonText)
    {
        _buttonIcon.color = iconColor;
        _buttonText.text = buttonText;
    }

    #endregion
}
