using UnityEngine;


// This was made because unity seems to have a bug in this version that resets the positions other than (0,0) upon relaunch and build.
public class UIPanelRepositioningHelper : MonoBehaviour
{
    #region Serialized Fields

    [SerializeField] private float _panelPositionX;
    [SerializeField] private float _panelPositionY;

    #endregion

    #region Private Methods

    private void Start()
    {
        transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(_panelPositionX, _panelPositionY);
    }

    #endregion
}
