using UnityEngine;
using UnityEngine.UI;

#region Delegates

public delegate void OnStartAvailable();

#endregion

public class StartButtonController : MonoBehaviour
{
    #region Serialized Fields

    [SerializeField] private Button _button;
    [SerializeField] private GameObject _buttonIcon;

    #endregion

    #region Public Fields

    public OnStartAvailable onStartAvailable;

    #endregion

    #region Public Methods

    public void Start()
    {
        _button.interactable = false;
        _buttonIcon.SetActive(false);
        onStartAvailable = Activate;
    }

    #endregion

    #region Private Methods

    private void Activate()
    {
        if (!_button.interactable)
        {
            _button.interactable = true;
            _buttonIcon.SetActive(true);
        }
    }

    #endregion
}
