using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class BindKeyInfo : MonoBehaviour
{ 
    public KeyOptionInfo bindOption;

    [SerializeField]
    private Image mBt_Image;

    private void Awake()
    {
        mBt_Image = GetComponent<Image>();
    }

    public void BindOption(KeyOptionInfo _bindOption, Sprite _image)
    {
        bindOption = _bindOption;
        mBt_Image.sprite = _image;
    }

    public void RemoveBindOption(Sprite _initial_image)
    {
        mBt_Image.sprite = _initial_image;
        bindOption = null;
    }
}
