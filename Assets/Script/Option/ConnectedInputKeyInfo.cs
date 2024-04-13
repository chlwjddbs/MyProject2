using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ConnectedInputKeyInfo : MonoBehaviour
{ 
    public InputKeyInfo savedInputKeyInfo;

    [SerializeField]
    private Image mBt_Image;

    private void Start()
    {
        mBt_Image = GetComponent<Image>();
    }

    public void SaveInputKeyInfo(InputKeyInfo _savedInfo,Sprite _connectImage)
    {
        mBt_Image.sprite = _connectImage;
        savedInputKeyInfo = _savedInfo;
    }

    public void UnConnect(Sprite _unconnectImage)
    {
        mBt_Image.sprite = _unconnectImage;
        savedInputKeyInfo = null;
    }
}
