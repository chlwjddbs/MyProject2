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

    //input ������ ������ Ű ����
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
            //�ݺ��� int �� �� ������ �־���� ���� �۵���
            int keyNum = i;
            if(KeyBoard.GetChild(keyNum).TryGetComponent(out ConnectedInputKeyInfo cnt_InputkeyInfo))
            {
                //�Ű������� ����� ���ڴ� ���ٽ����� �־������
                //bt.onClick.AddListener(() => Clickbutton(bt.gameObject));
                cnt_InputkeyInfo.GetComponent<Button>().onClick.AddListener(() => Clickbutton(cnt_InputkeyInfo));
                keysDic.Add(cnt_InputkeyInfo.name, cnt_InputkeyInfo);

                //sprite ���¿��� Ű���ÿ� ���� sprite�� �ɷ�����.
                //sprite�� ã�� �����ϱ� ���� ��ųʸ��� Event �� �޾����� �̸����� �����Ѵ�.
                //input ������ ��ϵ� Ű�� white sprite�� �������� �ϱ� ���� black�� white �Ѵ� �����Ѵ�.
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
            Debug.Log("����� ��ư�� �����ϴ�.");
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
            Debug.Log("����� ��ư�� �����ϴ�.");
            //sc.verticalNormalizedPosition = 0.5f;
          
        }
    }

    public void Test()
    {
        
    }

    public void ChangeInputKey()
    {
        //Event.current.iskey�� ���� Ű �Է��� �޾��� �� True�� �޾� if���� �ѱ���� �ִ�.
        //������ Event.current.iskey�� input.Getkey�� ���� ���������� �ѹ� �� �Է� �ޱ� ���� keyDown���� ó���Ѵ�.
        //keyDown ȿ�� �ܿ��� �������� ��ư�� ������ ������ Event�� ������ ���ϱ� ������ ������ ���δ�.
        //���� ��� 123�� ������ ���ٴ� �Է��ϸ� 1213 �� ���� �Է¿� �Է��� ��� �Ǳ⵵ �Ѵ�.
        //�׷��� ������ �ڷ�ƾ�� ���� �Է� ��Ÿ���� �����԰� ���ÿ� keyDown���� �ѹ��� �ϳ��� key�� ó�� �ϵ��� �Ѵ�.

        if (Input.anyKeyDown) 
        {
            Debug.Log(Event.current.keyCode);
            isChange = false;
            StartCoroutine(ChangeCoolTime());

            //�Է¹��� Event ������ ����
            Event ev = Event.current;
            //�Է¹��� keyCode ����
            string keyName = ev.keyCode.ToString();

            //keysDic : input ������ ������ Ű
            //�Է¹��� key�� input ������ ������ Ű�϶�
            if (keysDic.TryGetValue(keyName, out ConnectedInputKeyInfo keyInfo))
            {
                //���� ��ư�� �̹����� �Է� ���� Ű(Event.currnet)�� �̹����� ��ü�Ѵ�.
                //selectOption.GetComponent<Image>().sprite = bKSprite_Dic[keyName];

                //ex) ������ �ɼ�(skillbutton1)�� ����� keyName�� ã�´�.
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
