using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class KeyOptionInfo : MonoBehaviour
{
    public KeyOption keyOption;
    public KeyCode bindKey;
    public Sprite curImage;

    public BindKeyInfo bindKeyInfo;

    public KeyCode InitialCode;

    [SerializeField]
    private Image mBt_Image;

    private void Awake()
    {
        mBt_Image = GetComponent<Image>();
    }

    public void SetOptionInfo(KeyCode _bindKey, Sprite _curImage, BindKeyInfo _bindKeyInfo = null)
    {
        bindKey = _bindKey;
        curImage = _curImage;
        mBt_Image.sprite = curImage;

        bindKeyInfo = _bindKeyInfo;
    }
}
