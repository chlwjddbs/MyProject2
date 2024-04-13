using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class ControlOption : MonoBehaviour
{
    [Serializable]
    public class InputOption
    {
        public string optionName;

        public KeyCode inputKey;

        public GameObject matchedButton;
        
        public bool isMatch;

        public void SetInputOption(KeyCode key, GameObject connectButton)
        {
            optionName = connectButton.name;
            inputKey = key;
            matchedButton = connectButton;
            isMatch = true;
        }

        public void UnsetInputOption()
        {
            matchedButton = null;
            isMatch = false;
        }
    }

    public enum UserKey
    {
        SkillButton1,
        SkillButton2,
        SkillButton3,
        SkillButton4,
        SkillButton5,
        KeyCount,
    }

    public Transform KeyBoard;
    public Transform KeyOption;

    //input 설정이 가능한 키 모음
    public Dictionary<string, ConnectedInputKeyInfo> keysDic = new Dictionary<string, ConnectedInputKeyInfo>();

    public ConnectedInputKeyInfo selectButton;
    public InputKeyInfo selectOption;

    public List<Sprite> blackKey;
    public List<Sprite> whiteKey;

    public Dictionary<string, InputKeyInfo> inputDic = new Dictionary<string, InputKeyInfo>();

    private Dictionary<string, Sprite> bKSprite_Dic = new Dictionary<string, Sprite>();
    private Dictionary<string, Sprite> whSprite_Dic = new Dictionary<string, Sprite>();

    [SerializeField]
    private bool isChange = true;
    private float changeCoolTime = 0.5f;

    public static bool isChaning = false;

    public ScrollRect sc;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < KeyBoard.childCount; i++)
        {
            //반복문 int 는 빈 공간에 넣어줘야 정상 작동함
            int keyNum = i;
            if(KeyBoard.GetChild(keyNum).TryGetComponent(out ConnectedInputKeyInfo cnt_InputkeyInfo))
            {
                //매개변수로 사용할 인자는 람다식으로 넣어줘야함
                //bt.onClick.AddListener(() => Clickbutton(bt.gameObject));
                cnt_InputkeyInfo.GetComponent<Button>().onClick.AddListener(() => Clickbutton(cnt_InputkeyInfo));
                keysDic.Add(cnt_InputkeyInfo.name, cnt_InputkeyInfo);

                //sprite 에셋에서 키세팅에 들어가는 sprite만 걸러낸다.
                //sprite를 찾기 쉽게하기 위해 딕셔너리에 Event 로 받아지는 이름으로 저장한다.
                //input 단축이 등록된 키는 white sprite로 보여지게 하기 위해 black과 white 둘다 저장한다.
                foreach (var bk in blackKey)
                {
                    if(bk.name == cnt_InputkeyInfo.name)
                    {
                        bKSprite_Dic.Add(cnt_InputkeyInfo.name, bk);
                    }
                }

                foreach (var wh in whiteKey)
                {
                    if (wh.name == cnt_InputkeyInfo.name)
                    {
                        whSprite_Dic.Add(cnt_InputkeyInfo.name, wh);
                    }
                }

            }
        }

        //Debug.Log($"{bKSprite_Dic.Count} + {whSprite_Dic.Count}");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Clickbutton(ConnectedInputKeyInfo _selectButton)
    {
        selectButton = _selectButton;
        Focucemenu(selectButton);
    }

    public void SelectInputOption(InputKeyInfo _infoKey)
    {
        selectOption = _infoKey;
        isChaning = true;
    }

    public void Focucemenu(ConnectedInputKeyInfo _selectedButton)
    {
        Debug.Log(_selectedButton.savedInputKeyInfo);
        if(_selectedButton.savedInputKeyInfo == null)
        {
            Debug.Log("저장된 버튼이 없습니다.");
            return;
        }
        if (inputDic.ContainsKey(_selectedButton.savedInputKeyInfo.optionName))
        {
            Debug.Log(inputDic[_selectedButton.savedInputKeyInfo.optionName]);
            //inputDic[SelectedButton.name].matchedButton
            //sc.verticalNormalizedPosition = 0.5f;
            selectOption = inputDic[_selectedButton.savedInputKeyInfo.optionName];
        }
        else
        {
            Debug.Log("연결된 버튼이 없습니다.");
            //sc.verticalNormalizedPosition = 0.5f;
          
        }
    }

    public void Test()
    {
        
    }

    public void ChangeInputKey()
    {
        //Event.current.iskey를 통해 키 입력을 받았을 시 True를 받아 if문을 넘길수도 있다.
        //하지만 Event.current.iskey는 input.Getkey와 같이 동작함으로 한번 만 입력 받기 위에 keyDown으로 처리한다.
        //keyDown 효과 외에도 여러가지 버튼을 빠르게 누르면 Event가 빠르게 변하기 때문에 동작이 꼬인다.
        //예를 들어 123을 빠르게 따다닥 입력하면 1213 과 같이 입력외 입력이 출력 되기도 한다.
        //그렇기 때문에 코루틴을 통해 입력 쿨타임을 제한함과 동시에 keyDown으로 한번에 하나의 key만 처리 하도록 한다.

        if (Input.anyKeyDown) 
        {
            Debug.Log(Event.current.keyCode);
            isChange = false;
            StartCoroutine(ChangeCoolTime());

            //입력받은 Event 정보를 저장
            Event ev = Event.current;
            //입력받은 keyCode 저장
            string keyName = ev.keyCode.ToString();

            //keysDic : input 설정이 가능한 키
            //입력받은 key가 input 설정이 가능한 키일때
            if (keysDic.TryGetValue(keyName, out ConnectedInputKeyInfo keyInfo))
            {
                //현재 버튼의 이미지를 입력 받은 키(Event.currnet)의 이미지로 교체한다.
                //selectOption.GetComponent<Image>().sprite = bKSprite_Dic[keyName];

                //ex) 선택한 옵션(skillbutton1)이 저장된 keyName을 찾는다.
                if (inputDic.ContainsKey(selectOption.optionName))
                {
                    keysDic[selectOption.connectInfo.name].UnConnect(bKSprite_Dic[selectOption.connectInfo.name]);
                    keyInfo.SaveInputKeyInfo(selectOption, whSprite_Dic[keyName]);
                    selectOption.ChangeKeycode(ev.keyCode, bKSprite_Dic[keyName], keyInfo);
                }
                else
                {
                    keyInfo.SaveInputKeyInfo(selectOption, whSprite_Dic[keyName]);
                    selectOption.ChangeKeycode(ev.keyCode, bKSprite_Dic[keyName], keyInfo);
                    inputDic.Add(selectOption.optionName, selectOption);
                }
                Debug.Log(Event.current.keyCode.ToString());
                //currentButton = null;               
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
        if(!isChange)
        {
            StartCoroutine(ChangeCoolTime());
        }
    }

    private void OnDisable()
    {
        selectOption = null;
        isChaning = false;
    }
}
