using System;
using UnityEngine;
using UnityEngine.UI;

public class KeyOptionInfo : MonoBehaviour
{
    public KeyOption keyOption;

    public KeyCode bindKey;

    public Sprite curImage;

    [SerializeField] private Image mBt_Image;
    [SerializeField] private Button mBt;

    public BindKeyInfo bindKeyInfo;

    public KeyCode InitialCode;

    [SerializeField] private Image focusImage;
  
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
        OptionManager.instance.controllOption.changeKeyCode?.Invoke(keyOption, bindKey);

        //옵션이 변경되면 변경된 정보를 넘겨주어 저장한다.
        OptionData.instance.ChangeData(keyOption,this);
    }
      
    public void RemoveBindKey(Sprite _nobindImage)
    {
        bindKey = KeyCode.None;
        curImage = _nobindImage;
        mBt_Image.sprite = curImage;

        bindKeyInfo = null;

        //프로세스 변경으로 인해 딕셔너리에서 제거하지 않아도 된다.
        //ControllOption.instance.bindKey_Dic.Remove(keyOption);
        OptionManager.instance.controllOption.changeKeyCode?.Invoke(keyOption,bindKey);
        OptionData.instance.ChangeData(keyOption, this);
    }

    public void SelectOption()
    {
        focusImage.enabled = true;
    }

    public void DeSelectOption()
    {
        focusImage.enabled = false;
    }
}
