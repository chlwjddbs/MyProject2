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
            //�ݺ��� int �� �� ������ �־���� ���� �۵���
            int keyNum = i;
            if(KeyBoard.GetChild(keyNum).TryGetComponent(out Button bt))
            {
                //�Ű������� ����� ���ڴ� ���ٽ����� �־������
                bt.onClick.AddListener(() => Clickbutton(bt.gameObject));
                keysDic.Add(bt.name,bt.gameObject);

                //sprite ���¿��� Ű���ÿ� ���� sprite�� �ɷ�����.
                //sprite�� ã�� �����ϱ� ���� ��ųʸ��� Event �� �޾����� �̸����� �����Ѵ�.
                //input ������ ��ϵ� Ű�� white sprite�� �������� �ϱ� ���� black�� white �Ѵ� �����Ѵ�.
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
            Debug.Log("����� ��ư�� �����ϴ�.");
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
            isChange = false;
            StartCoroutine(ChangeCoolTime());

            //���� �̺�Ʈ�� isKey(Ű �Է��̸�)
            string keyName = Event.current.keyCode.ToString();

            if (keysDic.TryGetValue(keyName, out GameObject keyObj))
            {
                //���� ��ư�� �̹����� �Է� ���� Ű(Event.currnet)�� �̹����� ��ü�Ѵ�.
                currentButton.GetComponent<Image>().sprite = bKSprite_Dic[keyName];

                //input ����Ű�� ������ Ű���
                if (inputDic.ContainsKey(keyName))
                {
                    //ev�� �ش��ϴ� key�� �����Ѵ�.
                    //ex) wŰ�� �Է� ������ w�� ���� �Ǿ��ٰ� SetInputOption�� ���� ������ش�.
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
        //������ ��ư�� ������ Ű ������ �۵��ϸ� �ȵǹǷ� ���� �����ش�.
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
