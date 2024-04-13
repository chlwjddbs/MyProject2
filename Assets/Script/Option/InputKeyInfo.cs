using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class InputKeyInfo : MonoBehaviour
{
    public string optionName;
    public KeyCode connectedCode;
    public Sprite currentImage;

    public ConnectedInputKeyInfo connectInfo;

    [SerializeField]
    private Image mBt_Image;

    private void Start()
    {
        mBt_Image = GetComponent<Image>();
    }

    public void ChangeKeycode(KeyCode _code, Sprite _image, ConnectedInputKeyInfo _connectInfo)
    {
        connectedCode = _code;
        currentImage = _image;
        mBt_Image.sprite = currentImage;

        connectInfo = _connectInfo;

        
    }
}
