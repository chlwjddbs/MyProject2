using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class KeyOptionInfo : MonoBehaviour
{
    public KeyOption keyOption;

    public KeyCode bindKey;

    public Sprite curImage;
    [SerializeField]
    private Image mBt_Image;

    public BindKeyInfo bindKeyInfo;

    public KeyCode InitialCode;

    

    private void Awake()
    {
        mBt_Image = GetComponent<Image>();
    }

    public void Bindkey(KeyCode _bindKey, Sprite _curImage, BindKeyInfo _bindKeyInfo = null)
    {
        bindKey = _bindKey;
        curImage = _curImage;
        mBt_Image.sprite = curImage;

        bindKeyInfo = _bindKeyInfo;

        //bind된 key가 변경 된다면 변경된 정보를 받아야하는 스크립트에 정보를 건내준다.
        //ControllOption에서 UnityAction에 정보 요청을 한 스크립트에 뿌려준다.
        //ex)SkillButton은 keyCodeText의 변경을 위해 bindkey가 변경될 때마다 정보를 받아야 한다.
        //그래서 Action함수에 변경 정보를 수정하는 함수를 등록하여 정보를 처리하도록 한다.
        ControllOption.instance.changeKeyCode?.Invoke(keyOption, bindKey);
    }

    public void RemoveBindKey(Sprite _nobindImage)
    {
        bindKey = KeyCode.None;
        curImage = _nobindImage;
        mBt_Image.sprite = curImage;

        bindKeyInfo = null;

        //프로세스 변경으로 인해 딕셔너리에서 제거하지 않아도 된다.
        //ControllOption.instance.bindKey_Dic.Remove(keyOption);
        ControllOption.instance.changeKeyCode?.Invoke(keyOption,bindKey);
    }
}
