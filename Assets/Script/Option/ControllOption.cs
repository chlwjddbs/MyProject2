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
            //반복문 int 는 빈 공간에 넣어줘야 정상 작동함
            int keyNum = i;
            if (KeyBoard.GetChild(keyNum).TryGetComponent(out BindKeyInfo _bindkeyInfo))
            {
                _bindkeyInfo.SetKeyInfo();
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

                    //키옵션 정보가 중복됐는지 확인하기 위해서 다시 검사해준다.                  
                    if (!bindKey_Dic.TryGetValue(_keyOption.keyOption,out KeyOptionInfo overlapVal))
                    {
                        if (_keyOption.InitialCode == KeyCode.None)
                        {
                            //None으로 초기화 후 NoneCode 이미지로 변경
                            _keyOption.Bindkey(_keyOption.InitialCode, nobind_image, null);

                            bindKey_Dic.Add(_keyOption.keyOption, _keyOption);
                        }
                        else //현재 옵션의 기본 keycode가 None이 아닐때
                        {                          
                            string code = _keyOption.InitialCode.ToString();
                            Debug.Log(key_Dic[code].gameObject.name);
                            //현재 keyOption의 값을 code로 세팅하고
                            _keyOption.Bindkey(_keyOption.InitialCode, bKSprite_Dic[code], key_Dic[code]);
                            key_Dic[code].BindOption(_keyOption, whSprite_Dic[code]);

                            bindKey_Dic.Add(_keyOption.keyOption, _keyOption);
                        }
                    }
                    else
                    {
                        //ex) 설정 실수로 인해 OptionKeyContent[0]과 OptionKeyContent[1]이 같은 keyOption을 가지게 되면
                        //[0]이 등록된 후에 [1]을 등록하려고 시도하면 이미 등록된 옵션이기 때문에 [1]이 중복되었다고 출력해준다.
                        Debug.Log($"{overlapVal.keyOption} : 중복된 옵션.");
                    }
                }
            }
            optionData.SetData(bindKey_Dic);
        }
        else //변경된 정보가 있을 경우
        {
            //옵션 정보를 모두 불러온다.
            for (int i = 0; i < OptionKeyContent.childCount; i++)
            {
                int optionNum = i;

                //옵션 정보를 가지고 있는 keyOptionInfo가 있는지 확인하고
                if (OptionKeyContent.GetChild(optionNum).TryGetComponent(out KeyOptionInfo _keyOption))
                {
                    if (_keyOption.mBt == null)
                    {
                        _keyOption.SetKeyOptionInfo();
                        _keyOption.GetComponent<Button>().onClick.AddListener(() => SelectOption(_keyOption));
                    }

                    //등록된 정보에서 중복 검사를 한다.
                    if (!bindKey_Dic.TryGetValue(_keyOption.keyOption, out KeyOptionInfo overlapVal))
                    {
                        //업데이트로 인해 새로운 옵션이 추가 되면 기존에 저장된 정보와 데이터가 맞지 않게 된다.
                        //기존 Bind 정보는 그대로 불러오고 그 이후에 추가된 정보만 새로 추가해 준다.

                        //새로 추가된 정보
                        if (optionNum >= optionData.bindKeyData.keyOtion.Count)
                        {
                            string newDataCode = _keyOption.InitialCode.ToString();

                            //새로 들어온 데이터의 초기값에 바인드 된 정보가 없으면 바로 Bind 해준다.
                            if (key_Dic[newDataCode].bindOption == null)
                            {
                                _keyOption.Bindkey(_keyOption.InitialCode, bKSprite_Dic[newDataCode], key_Dic[newDataCode]);
                                key_Dic[newDataCode].BindOption(_keyOption, whSprite_Dic[newDataCode]);

                                bindKey_Dic.Add(_keyOption.keyOption, _keyOption);

                                Debug.Log($"새로 추가 된 옵션 : {_keyOption.keyOption}");
                            }
                            else //바인드 된 정보가 있으면 기존에 사용하고 있는 옵션이 있다는 뜻으로 None으로 처리 해 준다.
                            {
                                _keyOption.Bindkey(KeyCode.None, nobind_image, null);
                                bindKey_Dic.Add(_keyOption.keyOption, _keyOption);

                                Debug.Log($"{newDataCode}에 이미 등록된 정보가 있습니다. {_keyOption.keyOption}은 None 처리 됩니다.");
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
                        Debug.Log($"에러 : {overlapVal} 데이터가 중복 되었습니다. 데이터를 확인해 주세요.");
                    }
                }
                else
                {
                    Debug.Log($"{optionNum}번 옵션에 스크립트가 없습니다.");
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
                        //기존 option에서 쓰던 key를 찾아 key에 bind된 option 제거
                        key_Dic[_keyOption.bindKey.ToString()].RemoveBindOption(bKSprite_Dic[_keyOption.bindKey.ToString()]);
                        //_keyOption.bindKeyInfo.RemoveBindKey(bKSprite_Dic[_keyOption.bindKey.ToString()]);

                        //keyOption에 bind된 key 제거
                        _keyOption.RemoveBindKey(nobind_image);

                        //option이 연결된 key가 없어졌으로 null값 적용.
                        Debug.Log(bindKey_Dic[_keyOption.keyOption].bindKeyInfo);

                        //bindkey 딕셔너리에서 해당 옵션을 지워준다. <- 프로세스 변경으로 인해 지워주지 않는다.
                        //bindKey_Dic.Remove(_keyOption.keyOption);

                        //ex) SkillButton1의 InitialCode 가 None이지만 현재 bind된 key가 A일때
                        //skillButton1의 bindKey를 None으로 바꿔주고
                        //bindKey_Dic에서 SkillButton1을 찾아서 지워준다.
                    }
                    //초기값과 bindKey가 다를때
                    else
                    {
                        string code = _keyOption.InitialCode.ToString();
                        //초기값에 해당하는 key에 할당된 option에 연결되어 있는 초기값 key 제거
                        //button1이 S에 할당되어 있고 A가 초기 값일때
                        //A에 할당되어 있는 option을 찾아
                        //해당 옵션에서 A키와의 bind 삭제
                        if (key_Dic[code].bindOption != null)
                        {
                            key_Dic[code].bindOption.RemoveBindKey(nobind_image);
                        }
                        //초기값 키에 연결된 option제거 : 해당키는 아래에서 다시 쓰일 것임으로 굳이 초기화하지 않아도 된다.
                        //key_Dic[code].RemoveBindOption(bKSprite_Dic[code]);

                        if (_keyOption.bindKeyInfo != null)
                        {
                            //현재 옵션이 사용중인 key에서 현재 옵션과의 bind를 제거한다.
                            _keyOption.bindKeyInfo.RemoveBindOption(bKSprite_Dic[_keyOption.bindKey.ToString()]);
                        }

                        //현재 keyOption의 초기값으로 Bind한다.
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
        //똑같은 Key를 한번 더 누르면 선택 해제를 해준다.
        if (selectKey == _selectKey)
        {
            DeSelectAll();
        }
        else
        {
            //기존에 선택한 키가 있고
            if (selectKey != null)
            {
                //selectKey는 선택한 키가 저장된다. 그래서 selectKey가 null이 아니라면 기존에 선택했던 key가 존재한다.
                //그래서 _selectKey를 selectKey에 넣어주기 전까지는 기존에 선택한 키가 된다.
                //기존에 선택한 키에 바인드 된 옵션이 있으면 바인드 된 옵션도 select 상태임으로 DeSelect해준다.
                selectKey.bindOption?.DeSelectOption();
                //기존에 선택한 키도 DeSelect해준다.
                selectKey.DeSelectKey();
            }

            //새로 선택한 Key를 Select 해주고
            _selectKey.SelectKey();

            //새로 선택한 키를 선택한 키로 바꿔준다.
            selectKey = _selectKey;
            FocusOption(selectKey);

            isChanging = true;
        }
    }

    public void SelectOption(KeyOptionInfo _optionInfo)
    {
        //똑같은 Option을 다시 선택하면 선택 해제를 해준다.
        if (selectOption == _optionInfo)
        {
            Debug.Log("여기 안옴?");
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
            Debug.Log("연결된 키가 없습니다.");
        }
    }

    public void FocusOption(BindKeyInfo _selectKey)
    {       
        if (_selectKey.bindOption != null)
        {
            //scrollview vale = 보여줄 컨텐츠의 위치 / (전체 content의 크기) - (contentview의 크기)
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
        else //현재 선택한 키에 바인드 된 옵션이 없다면
        {
            //기존에 선택되어 있던 옵션은 DeSelect 해준다.
            selectOption?.DeSelectOption();
            selectOption = null;
            Debug.Log("연결된 기능이 없습니다.");
        }

        /*
         * 이 문단은 if(bindKey_Dic.TryGetValue(_selectKey.bindKeyOption.keyOption, out KeyOptionInfo _keyOption)의
         * _keyOption = null 인것과 같습니다.
        Debug.Log(_selectKey.bindKeyOption);
        if(_selectKey.bindKeyOption == null)
        {
            Debug.Log("저장된 버튼이 없습니다.");
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
            Debug.Log("연결된 버튼이 없습니다.");
            //sc.verticalNormalizedPosition = 0.5f;
          
        }
        */
    }

    public void ChangeKeyBind()
    {
        //Event.current.iskey를 통해 키 입력을 받았을 시 True를 받아 if문을 넘길수도 있다.
        //하지만 Event.current.iskey는 input.Getkey와 같이 동작함으로 같은 키가 여러번 들어올 수 있기 때문에 keyDown으로 한번에 하나의 동작만 처리한다.
        //단시간 내에 여러개 버튼을 빠르게 누르면 Event가 빠르게 변하기 때문에 동작이 꼬인다.
        //예를 들어 123을 빠르게 따다닥 입력하면 1213 과 같이 출력이 된다.
        //입력에 제한 시간을 두어 한번에 하나의 key만 처리 하도록 한다.
        //&& Event.current.isKey 를 추가해 마우스 입력은 필터한다. [마우스 설정 추가시 변경 예정]
        if (Input.anyKeyDown && Event.current.isKey) 
        {
            isChange = false;
            StartCoroutine(ChangeCoolTime());

            //입력받은 key 정보를 저장
            Event ev = Event.current;
            string newKey = ev.keyCode.ToString();
            KeyCode tmp_Code = selectOption.bindKey;

            if(ev.keyCode == KeyCode.Escape)
            {
                DeSelectAll();
            }

            if (key_Dic.TryGetValue(newKey, out BindKeyInfo _bindkey))
            {             
                //selectOption이 bind된 key를 찾는다.           
                if (bindKey_Dic.TryGetValue(selectOption.keyOption, out KeyOptionInfo _keyOption))
                {
                    if (_keyOption.bindKey.ToString() == newKey)
                    {
                        Debug.Log("같은 키로 변경할 수 없습니다.");
                        return;
                    }
                    //입력받은 key에 연결된 keyoption에 있는 연결된 key정보를 없애준다.
                    //ex)skillbutton1이 S에 할당되어 있을때 A로 바꾸고 싶다면
                    //A에 연결 되어있는 keyOption(skillbutton2)을 찾아 skillbutton2에 연결된 A버튼을 제거한다.
                    //A에 skillbutton1만 새로 덮으면 데이터상 skillbutton2에도 A랑 연결되어 있기 때문에 제거 해준다.
                    if (_bindkey.bindOption != null)
                    {
                        if(optionManager.tmp_BindDic.TryGetValue(_bindkey.bindOption.keyOption, out KeyCode vale))
                        {
                            //취소 기능을 위해 만들어진 tmp 딕셔너리는 옵션이 바뀌기 전 기존의 처음값만 있으면 된다.
                            //수 없이 많이 반복해서 바꿔도 취소를 누르면 처음의 값이 필요하기 때문이다.
                        }
                        else
                        {
                            optionManager.tmp_BindDic.Add(_bindkey.bindOption.keyOption, _bindkey.bindOption.bindKey);
                        }
                        _bindkey.bindOption.RemoveBindKey(nobind_image);
                    }

                    //위와 마찬가지로 selectOption에 할당되어 있던 bindKeyInfo에서 selectOption을 제거한다.
                    if (_keyOption.bindKeyInfo != null)
                    {
                        _keyOption.bindKeyInfo.RemoveBindOption(bKSprite_Dic[_keyOption.bindKey.ToString()]);
                    }

                    //입력받은 key에 selectOtpion 정보를 새로 Bind 해준다.
                    _bindkey.BindOption(selectOption, whSprite_Dic[newKey]);

                    //selectOption의 정보를 입력받은 key로 갱신한다.
                    _keyOption.Bindkey(ev.keyCode, bKSprite_Dic[newKey], _bindkey);
                    //bindKey_Dic[selectOption.keyOption] = _keyOption; : 딕셔너리부터 값을 가져온거기 때문에 값을 변경해주면 굳이 다시 넣지 않아도 된다.

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
                    Debug.Log("연결된 기능이 없습니다. 확인해주세요.");
                }

                //Case) skillButton1과 A key가 bind , skillButton2와 S key가 bind
                //      ->skillButton1의 key를 S로 변경 시도시 -> S key에 참조된 skillButton2로 들어가 연결된 S key 제거.
                //      ->skillButton1에 참조된 A key로 들어가 연결된 skillButton1 제거
                //      ->비어이는 S key에 skillButton1 Bind
                //      ->skillButton1에 S key 정보를 입력
                //      ->A key와 SkillButton2는 연결된 정보 없음.
                //      ->S key에 SkillBUtton1 덮어쓰기 완료.              
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
                    Debug.Log($"에러 : {_keyOption.keyOption}이 저장이 안됐습니다.");
                }
            }
            else
            {
                Debug.Log($"{keyName}에 에러가 있습니다.!");
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

        //입력받은 key 정보를 저장
        Event ev = Event.current;
        string keyName = ev.keyCode.ToString();

        if (ev.keyCode == KeyCode.Escape)
        {
            selectOption = null;
            isChanging = false;
        }
       
        //key_Dic : bind 가능한 key목록
        //입력받은 keyCode가 bind 가능한 키인지 체크
        if (key_Dic.TryGetValue(keyName, out BindKeyInfo _bindkey))
        {
            //입력받은 key에 기존에 bind된 option이 있는지 확인
            if (_bindkey.bindOption != null)
            {
                //Debug.Log($"{keyName} , {_bindkey.bindKeyOption}의 keyCode :{_bindkey.bindKeyOption.bindKey}");

                //bind된 option이 있다면 바인드 된 key정보를 지워준다.                       
                bindKey_Dic[_bindkey.bindOption.keyOption].RemoveBindKey(nobind_image);
                //bindKey_Dic.Remove(_bindkey.bindKeyOption.keyOption); : BindKeyInfo의 RemovedOptionInfo안으로 통합
            }

            //selectOption이 bind된 key를 찾는다.           
            if (bindKey_Dic.TryGetValue(selectOption.keyOption, out KeyOptionInfo _keyOption))
            {
                //Debug.Log($"{keyName} , {_bindkey.bindKeyOption}의 keyCode :{_bindkey.bindKeyOption.bindKey}");
                Debug.Log($"{keyName} , {_keyOption}의 keyCode :{_keyOption.bindKey}");

                //selectOption이 변경되기 전 key와의 Bind 정보를 지워준다.
                _keyOption.bindKeyInfo.RemoveBindOption(bKSprite_Dic[_keyOption.bindKey.ToString()]);
                //입력받은 key에 selectOtpion 정보를 Bind 해준다.
                _bindkey.BindOption(selectOption, whSprite_Dic[keyName]);

                //selectOption의 정보를 입력받은 key로 바꿔준다.
                _keyOption.Bindkey(ev.keyCode, bKSprite_Dic[keyName], _bindkey);
                //bindKey_Dic[selectOption.keyOption] = _keyOption; : 딕셔너리부터 값을 가져온거기 때문에 값을 변경해주면 굳이 다시 넣지 않아도 된다.             
            }
            else
            {
                //bind 되지 않는 경우는 option의 초기값이 KeyCode.None으로 처음부터 지정이 되어 있지 않거나, bind에서 지웠을 때 이다.
                _bindkey.BindOption(selectOption, whSprite_Dic[keyName]);
                selectOption.Bindkey(ev.keyCode, bKSprite_Dic[keyName], _bindkey);
                //
                bindKey_Dic.Add(selectOption.keyOption, selectOption);
            }
            //currentButton = null;
        }
        else
        {
            Debug.Log($"{keyName} : Bind 불가능 Key");
        }
    }

    //ControllOption이 SetAcitve False 처리가 되면 동작이 멈추기 때문에 OnEnable에서 다시 실행
    //키 옵션 변경 처리 시간을 확보한다.
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
            ChangeKeyBind();
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
