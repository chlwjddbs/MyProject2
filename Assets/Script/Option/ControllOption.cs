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

    //설정 가능한 키 모음
    public Transform KeyBoard;
    //설정 가능한 옵션 모음
    public Transform OptionKeyContent;

    public Dictionary<string, BindKeyInfo> key_Dic = new Dictionary<string, BindKeyInfo>();

    //black, white key 스프라이트 모음
    [SerializeField]private List<Sprite> bkKey_Image;
    [SerializeField]private List<Sprite> whKey_Image;
    private Dictionary<string, Sprite> bKSprite_Dic = new Dictionary<string, Sprite>();
    private Dictionary<string, Sprite> whSprite_Dic = new Dictionary<string, Sprite>();

    //bind되지 않은 option의 이미지는 물음표(?) 이미지로 대체한다.
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
            //반복문 int 는 빈 공간에 넣어줘야 정상 작동함
            int keyNum = i;
            if (KeyBoard.GetChild(keyNum).TryGetComponent(out BindKeyInfo _bindkeyInfo))
            {
                //매개변수로 사용할 인자는 람다식으로 넣어줘야함
                //bt.onClick.AddListener(() => Clickbutton(bt.gameObject));
                _bindkeyInfo.GetComponent<Button>().onClick.AddListener(() => SelectBindKey(_bindkeyInfo));
                key_Dic.Add(_bindkeyInfo.name, _bindkeyInfo);

                //sprite 에셋에서 키세팅에 들어가는 sprite만 걸러낸다.
                //sprite를 찾기 쉽게하기 위해 딕셔너리에 Event 로 받아지는 이름으로 저장한다.
                //input 단축이 등록된 키는 white sprite로 보여지게 하기 위해 black과 white 둘다 저장한다.
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
                            //초기값으로 초기화 후 NoneCode 이미지로 변경
                            _keyOption.SetOptionInfo(_keyOption.InitialCode, nobind_image, null);
                        }
                        //현재 옵션의 기본 keycode가 None이 아닐때
                        else
                        {
                            string code = _keyOption.InitialCode.ToString();
                            //현재 keyOption의 값을 세팅하고
                            _keyOption.SetOptionInfo(_keyOption.InitialCode, bKSprite_Dic[code], key_Dic[code]);
                            key_Dic[code].BindKey(_keyOption, whSprite_Dic[code]);

                            //초기 코드가 None이면 bind 될 키가 없다는 뜻임으로 None일는 추가 하지 않는다.
                            bindKey_Dic.Add(_keyOption.keyOption, _keyOption);
                        }
                    }
                    else
                    {
                        Debug.Log($"{value.keyOption}이 중복 되었습니다.");
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
                //초기값과 설정된 키가 같으면 초기값에서 변경된 것이 없다는 뜻이다.               
                if (_keyOption.bindKey == _keyOption.InitialCode)
                {
                    Debug.Log("이미 기본 세팅입니다.");
                    //Case1. 만약 기본값이 None인데 bindKey도 None이라면 어차피 연결되지 않았기 때문에 세팅하지 않는다.
                    //Case2. 기본값이 A이고 bindKey도 A여도 기본값이 None이 아니라면 SetKeyBind()에서 기본 세팅이 되기 때문에 세팅하지 않는다.
                    //Case3. 기본값이 A이고 bindKey가 B로 바뀌었다가 A로 다시 돌아온 경우SetKeyBind()에서 기본 세팅이 된것과 같기 떄문에 세팅하지 않는다.
                }
                else
                {
                    //*위의 if문에서 초기값과 설정된 값이 다르다고 판정되었기 때문에
                    //기본코드가 None이지만 유저가 키를 세팅 했을 경우가 된다.
                    if (_keyOption.InitialCode == KeyCode.None)
                    {
                        //key에 저장된 keyOpiton 정보를 지워준다.
                        key_Dic[_keyOption.bindKey.ToString()].RemoveBindKey(nobind_image);

                        //bindKey를 다시 None으로 바꿔주고
                        _keyOption.SetOptionInfo(_keyOption.InitialCode, nobind_image);
                        //bindkey 딕셔너리에서 해당 옵션을 지워준다.
                        bindKey_Dic.Remove(_keyOption.keyOption);
                      
                        //ex) SkillButton1의 InitialCode 가 None이지만 현재 bind된 key가 A일때
                        //skillButton1의 bindKey를 None으로 바꿔주고
                        //bindKey_Dic에서 SkillButton1을 찾아서 지워준다.
                    }
                    //초기값과 bindKey가 다를때
                    else
                    {
                        string code = _keyOption.InitialCode.ToString();

                        //현재 keyOption의 초기값으로 Bind한다.
                        _keyOption.SetOptionInfo(_keyOption.InitialCode, bKSprite_Dic[code], key_Dic[code]);
                        key_Dic[code].BindKey(_keyOption, whSprite_Dic[code]);

                        //Bind된 정보를 등록한다.
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
            Debug.Log("저장된 버튼이 없습니다.");
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
            Debug.Log("연결된 버튼이 없습니다.");
            //sc.verticalNormalizedPosition = 0.5f;
          
        }
        
    }

    public void ChangeInputKey()
    {
        //Event.current.iskey를 통해 키 입력을 받았을 시 True를 받아 if문을 넘길수도 있다.
        //하지만 Event.current.iskey는 input.Getkey와 같이 동작함으로 한번 만 입력 받기 위에 keyDown으로 처리한다.
        //keyDown 효과 외에도 여러가지 버튼을 빠르게 누르면 Event가 빠르게 변하기 때문에 동작이 꼬인다.
        //예를 들어 123을 빠르게 따다닥 입력하면 1213 과 같이 입력외 입력이 출력 되기도 한다.
        //그렇기 때문에 코루틴을 통해 입력 쿨타임을 제한함과 동시에 keyDown으로 한번에 하나의 key만 처리 하도록 한다.
        //&& Event.current.isKey 를 추가해 마우스 입력은 필터한다.

        if (Input.anyKeyDown && Event.current.isKey) 
        {
            isChange = false;
            StartCoroutine(ChangeCoolTime());

            //입력받은 Event 정보를 저장
            Event ev = Event.current;
            //입력받은 keyCode 저장
            string keyName = ev.keyCode.ToString();

            //keysDic : input 설정이 가능한 키
            //입력받은 key가 keycode 설정이 가능한 키일때
            if (key_Dic.TryGetValue(keyName, out BindKeyInfo keyInfo))
            {
                if (keyInfo.bindKeyOption != null)
                {
                    Debug.Log(keyInfo.bindKeyOption.bindKey.ToString());
                    bindKey_Dic[keyInfo.bindKeyOption.keyOption].SetOptionInfo(KeyCode.None,nobind_image);
                }
                //현재 버튼의 이미지를 입력 받은 키(Event.currnet)의 이미지로 교체한다.
                //selectOption.GetComponent<Image>().sprite = bKSprite_Dic[keyName];

                //ex) 선택한 옵션(skillbutton1)이 bind된 key를 찾는다.              
                if (bindKey_Dic.TryGetValue(selectOption.keyOption, out KeyOptionInfo _keyOption) )
                {
                    _keyOption.bindKeyInfo.RemoveBindKey(bKSprite_Dic[_keyOption.bindKey.ToString()]);
                    //key_Dic[_keyOption.bindKey.ToString()].RemoveBindKey(bKSprite_Dic[_keyOption.bindKeyInfo.name]);
                    keyInfo.BindKey(selectOption, whSprite_Dic[keyName]);
                    selectOption.SetOptionInfo(ev.keyCode, bKSprite_Dic[keyName], keyInfo);
                }
                //bind 되지 않는 경우는 option의 초기값이 KeyCode.None으로 처음부터 지정이 되어 있지 않거나, bind에서 지웠을 때 이다.
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
        //선택한 버튼이 없으면 키 변경이 작동하면 안되므로 리턴 시켜준다.
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
        //옵션창이 닫히면 오브젝트가 active false가 되어 코루틴 실행이 중지된다.
        //옵션창을 다시 켰을때 중지된 코루틴을 실행하여 키 옵션 설정을 가능하게 해준다.
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
