using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class BindKeyInfo : MonoBehaviour
{ 
    public KeyOptionInfo bindKeyOption;

    [SerializeField]
    private Image mBt_Image;

    private void Awake()
    {
        mBt_Image = GetComponent<Image>();
    }

    public void BindKey(KeyOptionInfo _bindOption, Sprite _image)
    {
        mBt_Image.sprite = _image;
        bindKeyOption = _bindOption;
    }

    public void RemoveBindKey(Sprite _initial_image)
    {
        mBt_Image.sprite = _initial_image;
        bindKeyOption = null;
    }
}
