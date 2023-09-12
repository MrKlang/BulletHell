using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private MainMenuController _mainMenu;
    [SerializeField] private GameObject _gameOverMenu;

    void Start()
    {
        _mainMenu.Setup();

        ToggleMainMenu(true);
        ToggleGameOver(false);
    }
    public void ToggleMainMenu(bool active)
    {
        _mainMenu.gameObject.SetActive(active);
    }

    public void ToggleGameOver(bool active)
    {
        _gameOverMenu.SetActive(active);
    }
}
