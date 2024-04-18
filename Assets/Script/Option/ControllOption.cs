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
    public GameObject test;

    public UnityAction<KeyOption,KeyCode> changeKeyCode;

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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            ResetKeyBind();
        }
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
                            _keyOption.Bindkey(_keyOption.InitialCode, nobind_image, null);
                            bindKey_Dic.Add(_keyOption.keyOption, _keyOption);
                        }
                        //���� �ɼ��� �⺻ keycode�� None�� �ƴҶ�
                        else
                        {
                            string code = _keyOption.InitialCode.ToString();
                            //���� keyOption�� ���� �����ϰ�
                            _keyOption.Bindkey(_keyOption.InitialCode, bKSprite_Dic[code], key_Dic[code]);
                            key_Dic[code].BindOption(_keyOption, whSprite_Dic[code]);

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

    public void ResetKeyBind()
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
                        //���� option���� ���� key�� ã�� key�� bind�� option ����
                        key_Dic[_keyOption.bindKey.ToString()].RemoveBindOption(bKSprite_Dic[_keyOption.bindKey.ToString()]);
                        //_keyOption.bindKeyInfo.RemoveBindKey(bKSprite_Dic[_keyOption.bindKey.ToString()]);

                        //keyOption�� bind�� key ����
                        _keyOption.RemoveBindKey(nobind_image);

                        //option�� ����� key�� ���������� null�� ����.
                        Debug.Log(bindKey_Dic[_keyOption.keyOption].bindKeyInfo);

                        //bindkey ��ųʸ����� �ش� �ɼ��� �����ش�. <- ���μ��� �������� ���� �������� �ʴ´�.
                        //bindKey_Dic.Remove(_keyOption.keyOption);

                        //ex) SkillButton1�� InitialCode �� None������ ���� bind�� key�� A�϶�
                        //skillButton1�� bindKey�� None���� �ٲ��ְ�
                        //bindKey_Dic���� SkillButton1�� ã�Ƽ� �����ش�.
                    }
                    //�ʱⰪ�� bindKey�� �ٸ���
                    else
                    {
                        string code = _keyOption.InitialCode.ToString();
                        //�ʱⰪ�� �ش��ϴ� key�� �Ҵ�� option�� ����Ǿ� �ִ� �ʱⰪ key ����
                        //button1�� S�� �Ҵ�Ǿ� �ְ� A�� �ʱ� ���϶�
                        //A�� �Ҵ�Ǿ� �ִ� option�� ã��
                        //�ش� �ɼǿ��� AŰ���� bind ����
                        if (key_Dic[code].bindOption != null)
                        {
                            key_Dic[code].bindOption.RemoveBindKey(nobind_image);
                        }
                        //�ʱⰪ Ű�� ����� option���� : �ش�Ű�� �Ʒ����� �ٽ� ���� �������� ���� �ʱ�ȭ���� �ʾƵ� �ȴ�.
                        //key_Dic[code].RemoveBindOption(bKSprite_Dic[code]);

                        if (_keyOption.bindKeyInfo != null)
                        {
                            //���� �ɼ��� ������� key���� ���� �ɼǰ��� bind�� �����Ѵ�.
                            _keyOption.bindKeyInfo.RemoveBindOption(bKSprite_Dic[_keyOption.bindKey.ToString()]);
                        }

                        //���� keyOption�� �ʱⰪ���� Bind�Ѵ�.
                        _keyOption.Bindkey(_keyOption.InitialCode, bKSprite_Dic[code], key_Dic[code]);
                        key_Dic[code].BindOption(_keyOption, whSprite_Dic[code]);
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

    public void Focucemenu(BindKeyInfo _selectKey)
    {
        //scrollview vale = ������ �������� ��ġ / (��ü content�� ũ��) - (contentview�� ũ��)
        if (bindKey_Dic.TryGetValue(_selectKey.bindOption.keyOption, out KeyOptionInfo _keyOption))
        {
            Debug.Log(_keyOption.keyOption);
            selectOption = _keyOption;
        }
        else
        {
            Debug.Log("����� ����� �����ϴ�.");
        }

        /*
         * �� ������ if(bindKey_Dic.TryGetValue(_selectKey.bindKeyOption.keyOption, out KeyOptionInfo _keyOption)��
         * _keyOption = null �ΰͰ� �����ϴ�.
        Debug.Log(_selectKey.bindKeyOption);
        if(_selectKey.bindKeyOption == null)
        {
            Debug.Log("����� ��ư�� �����ϴ�.");
            return;
        }

        if (bindKey_Dic.ContainsKey(_selectKey.bindKeyOption.keyOption))
        {
            Debug.Log(bindKey_Dic[_selectKey.bindKeyOption.keyOption]);
            //inputDic[SelectedButton.name].matchedButton
            //sc.verticalNormalizedPosition = 0.5f;
            selectOption = bindKey_Dic[_selectKey.bindKeyOption.keyOption];
        }
        else
        {
            Debug.Log("����� ��ư�� �����ϴ�.");
            //sc.verticalNormalizedPosition = 0.5f;
          
        }
        */
    }

    public void ChangeInputKey()
    {
        //Event.current.iskey�� ���� Ű �Է��� �޾��� �� True�� �޾� if���� �ѱ���� �ִ�.
        //������ Event.current.iskey�� input.Getkey�� ���� ���������� ���� Ű�� ������ ���� �� �ֱ� ������ keyDown���� �ѹ��� �ϳ��� ���۸� ó���Ѵ�.
        //�ܽð� ���� ������ ��ư�� ������ ������ Event�� ������ ���ϱ� ������ ������ ���δ�.
        //���� ��� 123�� ������ ���ٴ� �Է��ϸ� 1213 �� ���� ����� �ȴ�.
        //�Է¿� ���� �ð��� �ξ� �ѹ��� �ϳ��� key�� ó�� �ϵ��� �Ѵ�.
        //&& Event.current.isKey �� �߰��� ���콺 �Է��� �����Ѵ�. [���콺 ���� �߰��� ���� ����]
        if (Input.anyKeyDown && Event.current.isKey) 
        {
            isChange = false;
            StartCoroutine(ChangeCoolTime());

            //�Է¹��� key ������ ����
            Event ev = Event.current;
            string keyName = ev.keyCode.ToString();

            if(ev.keyCode == KeyCode.Escape)
            {
                selectOption = null;
                isChanging = false;
            }

            if (key_Dic.TryGetValue(keyName, out BindKeyInfo _bindkey))
            {             
                //selectOption�� bind�� key�� ã�´�.           
                if (bindKey_Dic.TryGetValue(selectOption.keyOption, out KeyOptionInfo _keyOption))
                {
                    if (keyName == _keyOption.bindKey.ToString())
                    {
                        Debug.Log("���� Ű�� ������ �� �����ϴ�.");
                        return;
                    }
                    //�Է¹��� key�� ����� keyoption�� �ִ� ����� key������ �����ش�.
                    //ex)skillbutton1�� S�� �Ҵ�Ǿ� ������ A�� �ٲٰ� �ʹٸ� A�� skillbutton2�� �Ҵ�Ǿ� �ִٸ� skillbutton2�� ����� A��ư�� �����Ѵ�.
                    //A�� skillbutton1�� ���� ������ �����ͻ� skillbutton2���� A�� ����Ǿ� �ֱ� ������ �������ش�.
                    if (_bindkey.bindOption != null)
                    {
                        _bindkey.bindOption.RemoveBindKey(nobind_image);
                    }

                    //���� ���������� selectOption�� �Ҵ�Ǿ� �ִ� bindKeyInfo���� selectOption�� �����Ѵ�.
                    if (_keyOption.bindKeyInfo != null)
                    {
                        _keyOption.bindKeyInfo.RemoveBindOption(bKSprite_Dic[_keyOption.bindKey.ToString()]);
                    }

                    //�Է¹��� key�� selectOtpion ������ ���� Bind ���ش�.
                    _bindkey.BindOption(selectOption, whSprite_Dic[keyName]);

                    //selectOption�� ������ �Է¹��� key�� �����Ѵ�.
                    _keyOption.Bindkey(ev.keyCode, bKSprite_Dic[keyName], _bindkey);
                    //bindKey_Dic[selectOption.keyOption] = _keyOption; : ��ųʸ����� ���� �����°ű� ������ ���� �������ָ� ���� �ٽ� ���� �ʾƵ� �ȴ�.                  
                }
                else
                {
                    Debug.Log("����� ����� �����ϴ�. Ȯ�����ּ���.");
                }

                //Case) skillButton1�� A key�� bind , skillButton2�� S key�� bind
                //      ->skillButton1�� key�� S�� ���� �õ��� -> S key�� ������ skillButton2�� �� ����� S key ����.
                //      ->skillButton1�� ������ A key�� �� ����� skillButton1 ����
                //      ->����̴� S key�� skillButton1 Bind
                //      ->skillButton1�� S key ������ �Է�
                //      ->A key�� SkillButton2�� ����� ���� ����.
                //      ->S key�� SkillBUtton1 ����� �Ϸ�.
            }
        }
    }

    public void OldChangeInputKey()
    {
        isChange = false;
        StartCoroutine(ChangeCoolTime());

        //�Է¹��� key ������ ����
        Event ev = Event.current;
        string keyName = ev.keyCode.ToString();

        if (ev.keyCode == KeyCode.Escape)
        {
            selectOption = null;
            isChanging = false;
        }
       
        //key_Dic : bind ������ key���
        //�Է¹��� keyCode�� bind ������ Ű���� üũ
        if (key_Dic.TryGetValue(keyName, out BindKeyInfo _bindkey))
        {
            //�Է¹��� key�� ������ bind�� option�� �ִ��� Ȯ��
            if (_bindkey.bindOption != null)
            {
                //Debug.Log($"{keyName} , {_bindkey.bindKeyOption}�� keyCode :{_bindkey.bindKeyOption.bindKey}");

                //bind�� option�� �ִٸ� ���ε� �� key������ �����ش�.                       
                bindKey_Dic[_bindkey.bindOption.keyOption].RemoveBindKey(nobind_image);
                //bindKey_Dic.Remove(_bindkey.bindKeyOption.keyOption); : BindKeyInfo�� RemovedOptionInfo������ ����
            }

            //selectOption�� bind�� key�� ã�´�.           
            if (bindKey_Dic.TryGetValue(selectOption.keyOption, out KeyOptionInfo _keyOption))
            {
                //Debug.Log($"{keyName} , {_bindkey.bindKeyOption}�� keyCode :{_bindkey.bindKeyOption.bindKey}");
                Debug.Log($"{keyName} , {_keyOption}�� keyCode :{_keyOption.bindKey}");

                //selectOption�� ����Ǳ� �� key���� Bind ������ �����ش�.
                _keyOption.bindKeyInfo.RemoveBindOption(bKSprite_Dic[_keyOption.bindKey.ToString()]);
                //�Է¹��� key�� selectOtpion ������ Bind ���ش�.
                _bindkey.BindOption(selectOption, whSprite_Dic[keyName]);

                //selectOption�� ������ �Է¹��� key�� �ٲ��ش�.
                _keyOption.Bindkey(ev.keyCode, bKSprite_Dic[keyName], _bindkey);
                //bindKey_Dic[selectOption.keyOption] = _keyOption; : ��ųʸ����� ���� �����°ű� ������ ���� �������ָ� ���� �ٽ� ���� �ʾƵ� �ȴ�.             
            }
            else
            {
                //bind ���� �ʴ� ���� option�� �ʱⰪ�� KeyCode.None���� ó������ ������ �Ǿ� ���� �ʰų�, bind���� ������ �� �̴�.
                _bindkey.BindOption(selectOption, whSprite_Dic[keyName]);
                selectOption.Bindkey(ev.keyCode, bKSprite_Dic[keyName], _bindkey);
                //
                bindKey_Dic.Add(selectOption.keyOption, selectOption);
            }
            //currentButton = null;
        }
        else
        {
            Debug.Log($"{keyName} : Bind �Ұ��� Key");
        }
    }

    //ControllOption�� SetAcitve False ó���� �Ǹ� ������ ���߱� ������ OnEnable���� �ٽ� ����
    //Ű �ɼ� ���� ó�� �ð��� Ȯ���Ѵ�.
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

    public void TestScrollbar()
    {
        Debug.Log(test.GetComponent<RectTransform>().rect.height);
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
