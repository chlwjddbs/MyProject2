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

        //bind�� key�� ���� �ȴٸ� ����� ������ �޾ƾ��ϴ� ��ũ��Ʈ�� ������ �ǳ��ش�.
        //ControllOption���� UnityAction�� ���� ��û�� �� ��ũ��Ʈ�� �ѷ��ش�.
        //ex)SkillButton�� keyCodeText�� ������ ���� bindkey�� ����� ������ ������ �޾ƾ� �Ѵ�.
        //�׷��� Action�Լ��� ���� ������ �����ϴ� �Լ��� ����Ͽ� ������ ó���ϵ��� �Ѵ�.
        ControllOption.instance.changeKeyCode?.Invoke(keyOption, bindKey);
    }

    public void RemoveBindKey(Sprite _nobindImage)
    {
        bindKey = KeyCode.None;
        curImage = _nobindImage;
        mBt_Image.sprite = curImage;

        bindKeyInfo = null;

        //���μ��� �������� ���� ��ųʸ����� �������� �ʾƵ� �ȴ�.
        //ControllOption.instance.bindKey_Dic.Remove(keyOption);
        ControllOption.instance.changeKeyCode?.Invoke(keyOption,bindKey);
    }
}
