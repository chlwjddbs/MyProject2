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

        //bind�� key�� ���� �ȴٸ� ����� ������ �޾ƾ��ϴ� ��ũ��Ʈ�� ������ �ǳ��ش�.
        //ControllOption���� UnityAction�� ���� ��û�� �� ��ũ��Ʈ�� �ѷ��ش�.
        //ex)SkillButton�� keyCodeText�� ������ ���� bindkey�� ����� ������ ������ �޾ƾ� �Ѵ�.
        //�׷��� Action�Լ��� ���� ������ �����ϴ� �Լ��� ����Ͽ� ������ ó���ϵ��� �Ѵ�.
        OptionManager.instance.controllOption.changeKeyCode?.Invoke(keyOption, bindKey);

        //�ɼ��� ����Ǹ� ����� ������ �Ѱ��־� �����Ѵ�.
        OptionData.instance.ChangeData(keyOption,this);
    }
      
    public void RemoveBindKey(Sprite _nobindImage)
    {
        bindKey = KeyCode.None;
        curImage = _nobindImage;
        mBt_Image.sprite = curImage;

        bindKeyInfo = null;

        //���μ��� �������� ���� ��ųʸ����� �������� �ʾƵ� �ȴ�.
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
