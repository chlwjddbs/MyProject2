using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BindKeyInfo : MonoBehaviour
{ 
    public KeyOptionInfo bindOption;

    [SerializeField] private Image mBt_Image;
    [SerializeField] private Image selectImage;

    public void SetKeyInfo()
    {
        mBt_Image = GetComponent<Image>();

        foreach (Transform child in transform)
        {
            selectImage = child.GetComponent<Image>();
        }
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

    public void SelectKey()
    {
        selectImage.enabled = true;
    }

    public void DeSelectKey()
    {
        selectImage.enabled = false;
    }
}
