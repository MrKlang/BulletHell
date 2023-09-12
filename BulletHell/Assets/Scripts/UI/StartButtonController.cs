using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public delegate void OnStartAvailable();

public class StartButtonController : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private GameObject _buttonIcon;

    public OnStartAvailable onStartAvailable;
    
    public void Start()
    {
        _button.interactable = false;
        _buttonIcon.SetActive(false);
        onStartAvailable = Activate;
    }

    private void Activate()
    {
        if (!_button.interactable)
        {
            _button.interactable = true;
            _buttonIcon.SetActive(true);
        }
    }
}
