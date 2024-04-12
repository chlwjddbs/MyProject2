using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class ControlOption : MonoBehaviour
{
    [Serializable]
    public class InputOption
    {
        public string optionName;
        
        public bool isMatch;

        public Button matchedButton;

        public void SetInputOption(string btName, Button connectButton)
        {
            optionName = btName;
            matchedButton = connectButton;
            isMatch = true;
        }

        public void UnsetInputOption()
        {
            matchedButton = null;
            isMatch = false;
        }
    }

    public Transform KeyBoard;
    public Transform KeyOption;

    public Dictionary<string,GameObject> keysDic = new Dictionary<string, GameObject>();

    public GameObject currentButton;

    public Sprite image;
    public Sprite selected;

    public List<Sprite> blackKey;
    public List<Sprite> whiteKey;

    public Dictionary<string, InputOption> inputDic = new Dictionary<string, InputOption>();

    private Dictionary<string, Sprite> bKSprite_Dic = new Dictionary<string, Sprite>();
    private Dictionary<string, Sprite> whSprite_Dic = new Dictionary<string, Sprite>();

    private bool isChange = true;
    private float changeCoolTime = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < KeyBoard.childCount; i++)
        {
            //반복문 int 는 빈 공간에 넣어줘야 정상 작동함
            int keyNum = i;
            if(KeyBoard.GetChild(keyNum).TryGetComponent(out Button bt))
            {
                //매개변수로 사용할 인자는 람다식으로 넣어줘야함
                bt.onClick.AddListener(() => Clickbutton(bt.gameObject));
                keysDic.Add(bt.name,bt.gameObject);

                //sprite 에셋에서 키세팅에 들어가는 sprite만 걸러낸다.
                //sprite를 찾기 쉽게하기 위해 딕셔너리에 Event 로 받아지는 이름으로 저장한다.
                //input 단축이 등록된 키는 white sprite로 보여지게 하기 위해 black과 white 둘다 저장한다.
                foreach (var bk in blackKey)
                {
                    if(bk.name == bt.name)
                    {
                        bKSprite_Dic.Add(bt.name, bk);
                    }
                }

                foreach (var wh in whiteKey)
                {
                    if (wh.name == bt.name)
                    {
                        whSprite_Dic.Add(bt.name, wh);
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

    public void Clickbutton(GameObject selectButton)
    {
        Debug.Log(selectButton.name);
        currentButton = selectButton;
        Focucemenu(currentButton);
    }

    public void Focucemenu(GameObject SelectedButton)
    {
        if (inputDic.ContainsKey(SelectedButton.name))
        {
            Debug.Log(inputDic[SelectedButton.name].optionName);
            //inputDic[SelectedButton.name].matchedButton
        }
        else
        {
            Debug.Log("연결된 버튼이 없습니다.");
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
            isChange = false;
            StartCoroutine(ChangeCoolTime());

            //현재 이벤트가 isKey(키 입력이면)
            string keyName = Event.current.keyCode.ToString();

            if (keysDic.TryGetValue(keyName, out GameObject keyObj))
            {
                //현재 버튼의 이미지를 입력 받은 키(Event.currnet)의 이미지로 교체한다.
                currentButton.GetComponent<Image>().sprite = bKSprite_Dic[keyName];

                //input 단축키와 연동된 키라면
                if (inputDic.ContainsKey(keyName))
                {
                    //ev에 해당하는 key를 연결한다.
                    //ex) w키를 입력 받으면 w와 연결 되었다고 SetInputOption을 통해 등록해준다.
                    InputOption inputOp = new InputOption();
                    inputOp.SetInputOption(keyName, keyObj.GetComponent<Button>());
                    inputDic[keyName] = inputOp;
                    inputDic[keyName].matchedButton.GetComponent<Image>().sprite = whSprite_Dic[keyName];

                }
                else
                {
                    InputOption inputOp = new InputOption();
                    inputOp.SetInputOption(keyName, keyObj.GetComponent<Button>());
                    inputDic.Add(keyName, inputOp);
                }
                Debug.Log(Event.current.keyCode.ToString());
                //currentButton = null;               
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
        if (currentButton == null)
        {
            return;
        }

        if (isChange)
        {
            ChangeInputKey();
        }
    }
}
