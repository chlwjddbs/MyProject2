using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;


public class ControllOption : MonoBehaviour
{
    private OptionManager optionManager;
    private OptionData optionData;

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

    public ScrollRect scrollRect;
    public RectTransform AllOption;
    private float scrollVale = 0;

    public UnityAction<KeyOption,KeyCode> changeKeyCode;

    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            DeSelectAll();
        }
    }

    public void GameStart()
    {
        optionManager = OptionManager.instance;
        optionData = OptionData.instance;
        SetKeyData();
        SetKeyBindData();
    }

    public void SetKeyData()
    {
        for (int i = 0; i < KeyBoard.childCount; i++)
        {
            //�ݺ��� int �� �� ������ �־���� ���� �۵���
            int keyNum = i;
            if (KeyBoard.GetChild(keyNum).TryGetComponent(out BindKeyInfo _bindkeyInfo))
            {
                _bindkeyInfo.SetKeyInfo();
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
    
    public void SetKeyBindData()
    {
        if (!optionData.LoadData())
        {
            for (int i = 0; i < OptionKeyContent.childCount; i++)
            {
                int optionNum = i;

                if (OptionKeyContent.GetChild(optionNum).TryGetComponent(out KeyOptionInfo _keyOption))
                {
                    if (_keyOption.mBt == null)
                    {
                        _keyOption.SetKeyOptionInfo();
                        _keyOption.GetComponent<Button>().onClick.AddListener(() => SelectOption(_keyOption));
                    }

                    //Ű�ɼ� ������ �ߺ��ƴ��� Ȯ���ϱ� ���ؼ� �ٽ� �˻����ش�.                  
                    if (!bindKey_Dic.TryGetValue(_keyOption.keyOption,out KeyOptionInfo overlapVal))
                    {
                        if (_keyOption.InitialCode == KeyCode.None)
                        {
                            //None���� �ʱ�ȭ �� NoneCode �̹����� ����
                            _keyOption.Bindkey(_keyOption.InitialCode, nobind_image, null);

                            bindKey_Dic.Add(_keyOption.keyOption, _keyOption);
                        }
                        else //���� �ɼ��� �⺻ keycode�� None�� �ƴҶ�
                        {                          
                            string code = _keyOption.InitialCode.ToString();
                            Debug.Log(key_Dic[code].gameObject.name);
                            //���� keyOption�� ���� code�� �����ϰ�
                            _keyOption.Bindkey(_keyOption.InitialCode, bKSprite_Dic[code], key_Dic[code]);
                            key_Dic[code].BindOption(_keyOption, whSprite_Dic[code]);

                            bindKey_Dic.Add(_keyOption.keyOption, _keyOption);
                        }
                    }
                    else
                    {
                        //ex) ���� �Ǽ��� ���� OptionKeyContent[0]�� OptionKeyContent[1]�� ���� keyOption�� ������ �Ǹ�
                        //[0]�� ��ϵ� �Ŀ� [1]�� ����Ϸ��� �õ��ϸ� �̹� ��ϵ� �ɼ��̱� ������ [1]�� �ߺ��Ǿ��ٰ� ������ش�.
                        Debug.Log($"{overlapVal.keyOption} : �ߺ��� �ɼ�.");
                    }
                }
            }
            optionData.SetData(bindKey_Dic);
        }
        else //����� ������ ���� ���
        {
            //�ɼ� ������ ��� �ҷ��´�.
            for (int i = 0; i < OptionKeyContent.childCount; i++)
            {
                int optionNum = i;

                //�ɼ� ������ ������ �ִ� keyOptionInfo�� �ִ��� Ȯ���ϰ�
                if (OptionKeyContent.GetChild(optionNum).TryGetComponent(out KeyOptionInfo _keyOption))
                {
                    if (_keyOption.mBt == null)
                    {
                        _keyOption.SetKeyOptionInfo();
                        _keyOption.GetComponent<Button>().onClick.AddListener(() => SelectOption(_keyOption));
                    }

                    //��ϵ� �������� �ߺ� �˻縦 �Ѵ�.
                    if (!bindKey_Dic.TryGetValue(_keyOption.keyOption, out KeyOptionInfo overlapVal))
                    {
                        //������Ʈ�� ���� ���ο� �ɼ��� �߰� �Ǹ� ������ ����� ������ �����Ͱ� ���� �ʰ� �ȴ�.
                        //���� Bind ������ �״�� �ҷ����� �� ���Ŀ� �߰��� ������ ���� �߰��� �ش�.

                        //���� �߰��� ����
                        if (optionNum >= optionData.bindKeyData.keyOtion.Count)
                        {
                            string newDataCode = _keyOption.InitialCode.ToString();

                            //���� ���� �������� �ʱⰪ�� ���ε� �� ������ ������ �ٷ� Bind ���ش�.
                            if (key_Dic[newDataCode].bindOption == null)
                            {
                                _keyOption.Bindkey(_keyOption.InitialCode, bKSprite_Dic[newDataCode], key_Dic[newDataCode]);
                                key_Dic[newDataCode].BindOption(_keyOption, whSprite_Dic[newDataCode]);

                                bindKey_Dic.Add(_keyOption.keyOption, _keyOption);

                                Debug.Log($"���� �߰� �� �ɼ� : {_keyOption.keyOption}");
                            }
                            else //���ε� �� ������ ������ ������ ����ϰ� �ִ� �ɼ��� �ִٴ� ������ None���� ó�� �� �ش�.
                            {
                                _keyOption.Bindkey(KeyCode.None, nobind_image, null);
                                bindKey_Dic.Add(_keyOption.keyOption, _keyOption);

                                Debug.Log($"{newDataCode}�� �̹� ��ϵ� ������ �ֽ��ϴ�. {_keyOption.keyOption}�� None ó�� �˴ϴ�.");
                            }
                        }
                        else
                        {
                            KeyCode code = optionData.bindKeyData.bindCode[optionNum];
                            //Debug.Log(code);
                            if (code == KeyCode.None)
                            {
                                _keyOption.Bindkey(code, nobind_image, null);
                                bindKey_Dic.Add(_keyOption.keyOption, _keyOption);
                            }
                            else
                            {
                                _keyOption.Bindkey(code, bKSprite_Dic[code.ToString()], key_Dic[code.ToString()]);
                                key_Dic[code.ToString()].BindOption(_keyOption, whSprite_Dic[code.ToString()]);

                                bindKey_Dic.Add(_keyOption.keyOption, _keyOption);
                            }
                        }
                    }
                    else
                    {
                        Debug.Log($"���� : {overlapVal} �����Ͱ� �ߺ� �Ǿ����ϴ�. �����͸� Ȯ���� �ּ���.");
                    }
                }
                else
                {
                    Debug.Log($"{optionNum}�� �ɼǿ� ��ũ��Ʈ�� �����ϴ�.");
                }
            }
            optionData.SetData(bindKey_Dic);
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
        DeSelectAll();
        optionData.DeleteData();
    }

    public void SelectBindKey(BindKeyInfo _selectKey)
    {
        //�Ȱ��� Key�� �ѹ� �� ������ ���� ������ ���ش�.
        if (selectKey == _selectKey)
        {
            DeSelectAll();
        }
        else
        {
            //������ ������ Ű�� �ְ�
            if (selectKey != null)
            {
                //selectKey�� ������ Ű�� ����ȴ�. �׷��� selectKey�� null�� �ƴ϶�� ������ �����ߴ� key�� �����Ѵ�.
                //�׷��� _selectKey�� selectKey�� �־��ֱ� �������� ������ ������ Ű�� �ȴ�.
                //������ ������ Ű�� ���ε� �� �ɼ��� ������ ���ε� �� �ɼǵ� select ���������� DeSelect���ش�.
                selectKey.bindOption?.DeSelectOption();
                //������ ������ Ű�� DeSelect���ش�.
                selectKey.DeSelectKey();
            }

            //���� ������ Key�� Select ���ְ�
            _selectKey.SelectKey();

            //���� ������ Ű�� ������ Ű�� �ٲ��ش�.
            selectKey = _selectKey;
            FocusOption(selectKey);

            isChanging = true;
        }
    }

    public void SelectOption(KeyOptionInfo _optionInfo)
    {
        //�Ȱ��� Option�� �ٽ� �����ϸ� ���� ������ ���ش�.
        if (selectOption == _optionInfo)
        {
            Debug.Log("���� �ȿ�?");
            DeSelectAll();
        }
        else
        {
            if (selectOption != null)
            {
                selectOption.bindKeyInfo?.DeSelectKey();
                selectOption.DeSelectOption();
            }

            _optionInfo.SelectOption();

            selectOption = _optionInfo;
            Focuskey(selectOption);

            isChanging = true;
        }
    }

    public void DeSelectAll()
    {
        selectKey?.DeSelectKey();
        selectKey = null;       
        selectOption?.DeSelectOption();
        selectOption = null;
        isChanging = false;
    }

    public void Focuskey(KeyOptionInfo _selectOption)
    {
        if(_selectOption.bindKeyInfo != null) 
        {
            selectKey?.DeSelectKey();
            _selectOption.bindKeyInfo.SelectKey();
            selectKey = _selectOption.bindKeyInfo;
        }
        else
        {
            selectKey?.DeSelectKey();
            selectKey = null;
            Debug.Log("����� Ű�� �����ϴ�.");
        }
    }

    public void FocusOption(BindKeyInfo _selectKey)
    {       
        if (_selectKey.bindOption != null)
        {
            //scrollview vale = ������ �������� ��ġ / (��ü content�� ũ��) - (contentview�� ũ��)
            if (bindKey_Dic.TryGetValue(_selectKey.bindOption.keyOption, out KeyOptionInfo _keyOption))
            {
                Debug.Log(_keyOption.keyOption);
                //SelectOption(_keyOption);
                if (_keyOption.TryGetComponent(out RectTransform _opRect))
                {
                    float _opPos = (_opRect.anchoredPosition.y + (_opRect.rect.height / 2));
                    scrollVale = 1 + (_opPos / (AllOption.rect.height - scrollRect.viewport.rect.height));
                    Debug.Log(scrollVale);
                    scrollVale = Mathf.Clamp(scrollVale, 0, 1);
                    scrollRect.verticalScrollbar.value = (scrollVale);

                    selectOption?.DeSelectOption();
                    _keyOption.SelectOption();
                    selectOption = _keyOption;
                }
            }
        }
        else //���� ������ Ű�� ���ε� �� �ɼ��� ���ٸ�
        {
            //������ ���õǾ� �ִ� �ɼ��� DeSelect ���ش�.
            selectOption?.DeSelectOption();
            selectOption = null;
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

    public void ChangeKeyBind()
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
            string newKey = ev.keyCode.ToString();
            KeyCode tmp_Code = selectOption.bindKey;

            if(ev.keyCode == KeyCode.Escape)
            {
                DeSelectAll();
            }

            if (key_Dic.TryGetValue(newKey, out BindKeyInfo _bindkey))
            {             
                //selectOption�� bind�� key�� ã�´�.           
                if (bindKey_Dic.TryGetValue(selectOption.keyOption, out KeyOptionInfo _keyOption))
                {
                    if (_keyOption.bindKey.ToString() == newKey)
                    {
                        Debug.Log("���� Ű�� ������ �� �����ϴ�.");
                        return;
                    }
                    //�Է¹��� key�� ����� keyoption�� �ִ� ����� key������ �����ش�.
                    //ex)skillbutton1�� S�� �Ҵ�Ǿ� ������ A�� �ٲٰ� �ʹٸ�
                    //A�� ���� �Ǿ��ִ� keyOption(skillbutton2)�� ã�� skillbutton2�� ����� A��ư�� �����Ѵ�.
                    //A�� skillbutton1�� ���� ������ �����ͻ� skillbutton2���� A�� ����Ǿ� �ֱ� ������ ���� ���ش�.
                    if (_bindkey.bindOption != null)
                    {
                        if(optionManager.tmp_BindDic.TryGetValue(_bindkey.bindOption.keyOption, out KeyCode vale))
                        {
                            //��� ����� ���� ������� tmp ��ųʸ��� �ɼ��� �ٲ�� �� ������ ó������ ������ �ȴ�.
                            //�� ���� ���� �ݺ��ؼ� �ٲ㵵 ��Ҹ� ������ ó���� ���� �ʿ��ϱ� �����̴�.
                        }
                        else
                        {
                            optionManager.tmp_BindDic.Add(_bindkey.bindOption.keyOption, _bindkey.bindOption.bindKey);
                        }
                        _bindkey.bindOption.RemoveBindKey(nobind_image);
                    }

                    //���� ���������� selectOption�� �Ҵ�Ǿ� �ִ� bindKeyInfo���� selectOption�� �����Ѵ�.
                    if (_keyOption.bindKeyInfo != null)
                    {
                        _keyOption.bindKeyInfo.RemoveBindOption(bKSprite_Dic[_keyOption.bindKey.ToString()]);
                    }

                    //�Է¹��� key�� selectOtpion ������ ���� Bind ���ش�.
                    _bindkey.BindOption(selectOption, whSprite_Dic[newKey]);

                    //selectOption�� ������ �Է¹��� key�� �����Ѵ�.
                    _keyOption.Bindkey(ev.keyCode, bKSprite_Dic[newKey], _bindkey);
                    //bindKey_Dic[selectOption.keyOption] = _keyOption; : ��ųʸ����� ���� �����°ű� ������ ���� �������ָ� ���� �ٽ� ���� �ʾƵ� �ȴ�.

                    if (optionManager.tmp_BindDic.TryGetValue(selectOption.keyOption, out KeyCode _code))
                    {
                        //optionManager.tmp_BindDic[selectOption.keyOption] = tmp_Code;
                    }
                    else
                    {
                        optionManager.tmp_BindDic.Add(selectOption.keyOption, tmp_Code);
                    }

                    selectKey?.DeSelectKey();
                    _bindkey.SelectKey();
                    selectKey = _bindkey;

                    optionManager.ToggleSaveButton(true);

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

    public void BindCancel(Dictionary<KeyOption,KeyCode> tmp_bindDic)
    {
        foreach (var bindInfo in tmp_bindDic)
        {
            string keyName = bindInfo.Value.ToString();
            Debug.Log(bindInfo.Value);

            if (key_Dic.TryGetValue(keyName, out BindKeyInfo _bindkey))
            {
                if (bindKey_Dic.TryGetValue(bindInfo.Key, out KeyOptionInfo _keyOption))
                {
                    if (_bindkey.bindOption != null)
                    {
                        _bindkey.bindOption.RemoveBindKey(nobind_image);
                    }

                    if (_keyOption.bindKeyInfo != null)
                    {
                        _keyOption.bindKeyInfo.RemoveBindOption(bKSprite_Dic[_keyOption.bindKey.ToString()]);
                    }

                    _bindkey.BindOption(_keyOption, whSprite_Dic[keyName]);

                    _keyOption.Bindkey(bindInfo.Value, bKSprite_Dic[keyName], _bindkey);

                    DeSelectAll();
                    optionData.CreateSaveData();
                }
                else
                {
                    Debug.Log($"���� : {_keyOption.keyOption}�� ������ �ȵƽ��ϴ�.");
                }
            }
            else
            {
                Debug.Log($"{keyName}�� ������ �ֽ��ϴ�.!");
            }
        }
    }

    public void SaveOption()
    {
        DeSelectAll();
        optionData.CreateSaveData();
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
            ChangeKeyBind();
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
        BindCancel(optionManager.tmp_BindDic);
        DeSelectAll();
    }
    
}

public enum KeyOption
{
    SkillButton1,
    SkillButton2,
    SkillButton3,
    SkillButton4,
    SkillButton5,

    Inventory,
    Status,
    SkillBook,

    QuickSlot1,
    QuickSlot2,
    QuickSlot3,
    QuickSlot4,
    QuickSlot5,

    ViewDescription,

    KeyCount,
}
