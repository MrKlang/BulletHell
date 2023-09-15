using UnityEngine;

public class UIController : MonoBehaviour
{
    #region Serialized Fields

    [SerializeField] private MainMenuController _mainMenu;
    [SerializeField] private GameObject _gameOverMenu;

    #endregion

    #region Public Methods

    public void ToggleMainMenu(bool active)
    {
        _mainMenu.gameObject.SetActive(active);
    }

    public void ToggleGameOver(bool active)
    {
        _gameOverMenu.SetActive(active);
    }

    #endregion

    #region Private Methods

    private void Start()
    {
        _mainMenu.Setup();

        ToggleMainMenu(true);
        ToggleGameOver(false);
    }

    #endregion
}
