using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization.Components;

public class ConfirmedPopup : MonoBehaviour
{
    public RectTransform myRect;
    public Button okButton;
    public Button cancelButton;
    public LocalizeStringEvent popupText;

    private Vector3 closePos = new Vector3(0,1080f,0);

    public bool destroyUI = true;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (destroyUI)
            {
                Cancel();
            }
        }   
    }

    public void Cancel()
    {
        Destroy(gameObject);
    }

    public void CloseUI()
    {
        myRect.anchoredPosition = closePos;
    }
}


