using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;


public class ControllOption : MonoBehaviour
{
    public static ControllOption instance;

    //���� ������ Ű ����
    public Transform KeyBoard;
    //���� ������ �ɼ� ����
    public Transform OptionKeyContent;

    public Dictionary<string, BindKeyInfo> key_Dic = new Dictionary<string, BindKeyInfo>();

    //black, white key ��������Ʈ ����
    [SerializeField]private List<Sprite> bkKey_Image;
    [SerializeField]private List<Sprite> whKey_Image;
    private Dictionary<string, Sprite> bKSprite_Dic = new Dictionary<string, Sprite>();
    private Dictionary<string, Sprite> whSprite_Dic = new Dictionary<string, Sprite>();

    //bind���� ���� option�� �̹����� ����ǥ(?) �̹����� ��ü�Ѵ�.
    [SerializeField]private Sprite nobind_image;

    private BindKeyInfo selectKey;
    private KeyOptionInfo selectOption;

    public  Dictionary<KeyOption, KeyOptionInfo> bindKey_Dic = new Dictionary<KeyOption, KeyOptionInfo>();

    [SerializeField]  private bool isChange = true;
    private float changeCoolTime = 0.5f;

    public static bool isChanging = false;

    public ScrollRect sc;

    public UnityAction changeKeyCode;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(this);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void GameStart()
    {
        KeyOptionData();
        SetKeyBind();
    }

    public void KeyOptionData()
    {
        for (int i = 0; i < KeyBoard.childCount; i++)
        {
            //�ݺ��� int �� �� ������ �־���� ���� �۵���
            int keyNum = i;
            if (KeyBoard.GetChild(keyNum).TryGetComponent(out BindKeyInfo _bindkeyInfo))
            {
                //�Ű������� ����� ���ڴ� ���ٽ����� �־������
                //bt.onClick.AddListener(() => Clickbutton(bt.gameObject));
                _bindkeyInfo.GetComponent<Button>().onClick.AddListener(() => SelectBindKey(_bindkeyInfo));
                key_Dic.Add(_bindkeyInfo.name, _bindkeyInfo);

                //sprite ���¿��� Ű���ÿ� ���� sprite�� �ɷ�����.
                //sprite�� ã�� �����ϱ� ���� ��ųʸ��� Event �� �޾����� �̸����� �����Ѵ�.
                //input ������ ��ϵ� Ű�� white sprite�� �������� �ϱ� ���� black�� white �Ѵ� �����Ѵ�.
                foreach (var bk in bkKey_Image)
                {
                    if (bk.name == _bindkeyInfo.name)
                    {
                        bKSprite_Dic.Add(_bindkeyInfo.name, bk);
                    }                  
                }


                foreach (var wh in whKey_Image)
                {
                    if (wh.name == _bindkeyInfo.name)
                    {
                        whSprite_Dic.Add(_bindkeyInfo.name, wh);
                    }
                }

            }
        }
    }
    
    public void SetKeyBind()
    {
        if (DataManager.instance.newGame)
        {
            for (int i = 0; i < OptionKeyContent.childCount; i++)
            {
                int optionNum = i;

                if (OptionKeyContent.GetChild(optionNum).TryGetComponent(out KeyOptionInfo _keyOption))
                {
                    if (!bindKey_Dic.TryGetValue(_keyOption.keyOption,out KeyOptionInfo value))
                    {
                        if (_keyOption.InitialCode == KeyCode.None)
                        {
                            //�ʱⰪ���� �ʱ�ȭ �� NoneCode �̹����� ����
                            _keyOption.SetOptionInfo(_keyOption.InitialCode, nobind_image, null);
                        }
                        //���� �ɼ��� �⺻ keycode�� None�� �ƴҶ�
                        else
                        {
                            string code = _keyOption.InitialCode.ToString();
                            //���� keyOption�� ���� �����ϰ�
                            _keyOption.SetOptionInfo(_keyOption.InitialCode, bKSprite_Dic[code], key_Dic[code]);
                            key_Dic[code].BindKey(_keyOption, whSprite_Dic[code]);

                            //�ʱ� �ڵ尡 None�̸� bind �� Ű�� ���ٴ� �������� None�ϴ� �߰� ���� �ʴ´�.
                            bindKey_Dic.Add(_keyOption.keyOption, _keyOption);
                        }
                    }
                    else
                    {
                        Debug.Log($"{value.keyOption}�� �ߺ� �Ǿ����ϴ�.");
                    }
                }
            }
        }
        else
        {

        }
    }

    public void ResetKeyOption()
    {
        for (int i = 0; i < OptionKeyContent.childCount; i++)
        {
            int optionNum = i;
            if (OptionKeyContent.GetChild(optionNum).TryGetComponent(out KeyOptionInfo _keyOption))
            {
                //�ʱⰪ�� ������ Ű�� ������ �ʱⰪ���� ����� ���� ���ٴ� ���̴�.               
                if (_keyOption.bindKey == _keyOption.InitialCode)
                {
                    Debug.Log("�̹� �⺻ �����Դϴ�.");
                    //Case1. ���� �⺻���� None�ε� bindKey�� None�̶�� ������ ������� �ʾұ� ������ �������� �ʴ´�.
                    //Case2. �⺻���� A�̰� bindKey�� A���� �⺻���� None�� �ƴ϶�� SetKeyBind()���� �⺻ ������ �Ǳ� ������ �������� �ʴ´�.
                    //Case3. �⺻���� A�̰� bindKey�� B�� �ٲ���ٰ� A�� �ٽ� ���ƿ� ���SetKeyBind()���� �⺻ ������ �ȰͰ� ���� ������ �������� �ʴ´�.
                }
                else
                {
                    //*���� if������ �ʱⰪ�� ������ ���� �ٸ��ٰ� �����Ǿ��� ������
                    //�⺻�ڵ尡 None������ ������ Ű�� ���� ���� ��찡 �ȴ�.
                    if (_keyOption.InitialCode == KeyCode.None)
                    {
                        //key�� ����� keyOpiton ������ �����ش�.
                        key_Dic[_keyOption.bindKey.ToString()].RemoveBindKey(nobind_image);

                        //bindKey�� �ٽ� None���� �ٲ��ְ�
                        _keyOption.SetOptionInfo(_keyOption.InitialCode, nobind_image);
                        //bindkey ��ųʸ����� �ش� �ɼ��� �����ش�.
                        bindKey_Dic.Remove(_keyOption.keyOption);
                      
                        //ex) SkillButton1�� InitialCode �� None������ ���� bind�� key�� A�϶�
                        //skillButton1�� bindKey�� None���� �ٲ��ְ�
                        //bindKey_Dic���� SkillButton1�� ã�Ƽ� �����ش�.
                    }
                    //�ʱⰪ�� bindKey�� �ٸ���
                    else
                    {
                        string code = _keyOption.InitialCode.ToString();

                        //���� keyOption�� �ʱⰪ���� Bind�Ѵ�.
                        _keyOption.SetOptionInfo(_keyOption.InitialCode, bKSprite_Dic[code], key_Dic[code]);
                        key_Dic[code].BindKey(_keyOption, whSprite_Dic[code]);

                        //Bind�� ������ ����Ѵ�.
                        bindKey_Dic.Add(_keyOption.keyOption, _keyOption);
                    }
                }
            }
        }
    }

    public void SelectBindKey(BindKeyInfo _selectButton)
    {
        selectKey = _selectButton;
        Focucemenu(selectKey);
    }

    public void SelectOption(KeyOptionInfo _infoKey)
    {
        selectOption = _infoKey;
        isChanging = true;
    }

    public void Focucemenu(BindKeyInfo _selectedButton)
    {
        Debug.Log(_selectedButton.bindKeyOption);
        if(_selectedButton.bindKeyOption == null)
        {
            Debug.Log("����� ��ư�� �����ϴ�.");
            return;
        }
        
        if (bindKey_Dic.ContainsKey(_selectedButton.bindKeyOption.keyOption))
        {
            Debug.Log(bindKey_Dic[_selectedButton.bindKeyOption.keyOption]);
            //inputDic[SelectedButton.name].matchedButton
            //sc.verticalNormalizedPosition = 0.5f;
            selectOption = bindKey_Dic[_selectedButton.bindKeyOption.keyOption];
        }
        else
        {
            Debug.Log("����� ��ư�� �����ϴ�.");
            //sc.verticalNormalizedPosition = 0.5f;
          
        }
        
    }

    public void ChangeInputKey()
    {
        //Event.current.iskey�� ���� Ű �Է��� �޾��� �� True�� �޾� if���� �ѱ���� �ִ�.
        //������ Event.current.iskey�� input.Getkey�� ���� ���������� �ѹ� �� �Է� �ޱ� ���� keyDown���� ó���Ѵ�.
        //keyDown ȿ�� �ܿ��� �������� ��ư�� ������ ������ Event�� ������ ���ϱ� ������ ������ ���δ�.
        //���� ��� 123�� ������ ���ٴ� �Է��ϸ� 1213 �� ���� �Է¿� �Է��� ��� �Ǳ⵵ �Ѵ�.
        //�׷��� ������ �ڷ�ƾ�� ���� �Է� ��Ÿ���� �����԰� ���ÿ� keyDown���� �ѹ��� �ϳ��� key�� ó�� �ϵ��� �Ѵ�.
        //&& Event.current.isKey �� �߰��� ���콺 �Է��� �����Ѵ�.

        if (Input.anyKeyDown && Event.current.isKey) 
        {
            isChange = false;
            StartCoroutine(ChangeCoolTime());

            //�Է¹��� Event ������ ����
            Event ev = Event.current;
            //�Է¹��� keyCode ����
            string keyName = ev.keyCode.ToString();

            //keysDic : input ������ ������ Ű
            //�Է¹��� key�� keycode ������ ������ Ű�϶�
            if (key_Dic.TryGetValue(keyName, out BindKeyInfo keyInfo))
            {
                if (keyInfo.bindKeyOption != null)
                {
                    Debug.Log(keyInfo.bindKeyOption.bindKey.ToString());
                    bindKey_Dic[keyInfo.bindKeyOption.keyOption].SetOptionInfo(KeyCode.None,nobind_image);
                }
                //���� ��ư�� �̹����� �Է� ���� Ű(Event.currnet)�� �̹����� ��ü�Ѵ�.
                //selectOption.GetComponent<Image>().sprite = bKSprite_Dic[keyName];

                //ex) ������ �ɼ�(skillbutton1)�� bind�� key�� ã�´�.              
                if (bindKey_Dic.TryGetValue(selectOption.keyOption, out KeyOptionInfo _keyOption) )
                {
                    _keyOption.bindKeyInfo.RemoveBindKey(bKSprite_Dic[_keyOption.bindKey.ToString()]);
                    //key_Dic[_keyOption.bindKey.ToString()].RemoveBindKey(bKSprite_Dic[_keyOption.bindKeyInfo.name]);
                    keyInfo.BindKey(selectOption, whSprite_Dic[keyName]);
                    selectOption.SetOptionInfo(ev.keyCode, bKSprite_Dic[keyName], keyInfo);
                }
                //bind ���� �ʴ� ���� option�� �ʱⰪ�� KeyCode.None���� ó������ ������ �Ǿ� ���� �ʰų�, bind���� ������ �� �̴�.
                else
                {
                    keyInfo.BindKey(selectOption, whSprite_Dic[keyName]);
                    selectOption.SetOptionInfo(ev.keyCode, bKSprite_Dic[keyName], keyInfo);
                    bindKey_Dic.Add(selectOption.keyOption, selectOption);
                }            
                //currentButton = null;
                changeKeyCode?.Invoke();
            }
            else
            {
                Debug.Log(keyName);
            }
        }
    }

    IEnumerator ChangeCoolTime()
    {
        yield return new WaitForSecondsRealtime(changeCoolTime);
        isChange = true;
    }

    private void OnGUI()
    {
        //������ ��ư�� ������ Ű ������ �۵��ϸ� �ȵǹǷ� ���� �����ش�.
        if (selectOption == null)
        {
            return;
        }

        if (isChange)
        {
            ChangeInputKey();
        }
    }

    private void OnEnable()
    {
        //�ɼ�â�� ������ ������Ʈ�� active false�� �Ǿ� �ڷ�ƾ ������ �����ȴ�.
        //�ɼ�â�� �ٽ� ������ ������ �ڷ�ƾ�� �����Ͽ� Ű �ɼ� ������ �����ϰ� ���ش�.
        if(!isChange)
        {
            StartCoroutine(ChangeCoolTime());
        }
    }

    private void OnDisable()
    {
        selectOption = null;
        isChanging = false;
    }
}

public enum KeyOption
{
    SkillButton1,
    SkillButton2,
    SkillButton3,
    SkillButton4,
    SkillButton5,
    KeyCount,
}
