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
            //반복문 int 는 빈 공간에 넣어줘야 정상 작동함
            int keyNum = i;
            if(KeyBoard.GetChild(keyNum).TryGetComponent(out Button bt))
            {
                //매개변수로 사용할 인자는 람다식으로 넣어줘야함
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
