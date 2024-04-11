using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class ControlOption : MonoBehaviour
{
    [Serializable]
    public class MatchKeyOption
    {
        public List<Transform> matchedOption;
        public bool isMatch;

        public void SetOption()
        {

        }
    }

    public Transform KeyBoard;
    public Transform KeyOption;

    public GameObject currentButton;

   
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
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Clickbutton(GameObject selectButton)
    {
        currentButton = selectButton;
        Focucemenu(currentButton);
    }

    public void Focucemenu(GameObject SelectedButton)
    {

    }
}
