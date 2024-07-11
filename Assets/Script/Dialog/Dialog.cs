using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialog
{
    public int logNum;          //xml에서 접근할 Dialog 번호
    public string talkerImage;        //dialog ui에 표시할 이미지 
    public string talkerName;   //dialog에서 말하고 있는 대상
    public string sentence;     //dialog 내용

    public int nextlogNum;      //다음에 보여줄 dialog 번호.
                                //ex) 0번 dialog를 진행중
                                //case1) nextlogNum이 -1 : 0번 dialog가 끝난후 대화 종료 
                                //case2) nextlogNum이 2 : 0번 dialog가 끝난후 2번 dialog 출력
}
