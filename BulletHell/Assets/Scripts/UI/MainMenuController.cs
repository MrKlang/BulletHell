using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private List<MainMenuLevelButton> _levelSelectionButtons;
    [SerializeField] private StartButtonController _startButton;

    public void Setup()
    {
        for(int i = 0; i < GameplayController.instance.Settings.LevelsSettings.Count; i++)
        {
            var currentSettings = GameplayController.instance.Settings.LevelsSettings[i];
            _levelSelectionButtons[i].boundLevelSettingIndex = i;
            _levelSelectionButtons[i].SetupVisuals(currentSettings.Color, currentSettings.EntityCount.ToString());
        }
    }

    public void OnLevelSelectionChanged()
    {
        for (int i = 0; i < _levelSelectionButtons.Count; i++)
        {
            if (EventSystem.current.currentSelectedGameObject.Equals(_levelSelectionButtons[i].gameObject))
            {
                GameplayController.instance.SetCurrentLevelSettings(_levelSelectionButtons[i].boundLevelSettingIndex);
            }
        }

        _startButton.onStartAvailable?.Invoke();
    }
}
