using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private List<MainMenuLevelButton> _levelSelectionButtons;
    [SerializeField] private StartButtonController _startButton;

    [SerializeField] private GameSettings _settings;

    public void Setup()
    {
        for(int i = 0; i < _settings.levelsSettings.Count; i++)
        {
            var currentSettings = _settings.levelsSettings[i];
            _levelSelectionButtons[i].SetupVisuals(currentSettings.color, currentSettings.EntityCount.ToString());
        }
    }

    public void OnLevelSelectionChanged()
    {
        for (int i = 0; i < _levelSelectionButtons.Count; i++)
        {
            EventSystem.current.currentSelectedGameObject.Equals(_levelSelectionButtons[i].gameObject);
        }

        _startButton.onStartAvailable?.Invoke();
    }
}
