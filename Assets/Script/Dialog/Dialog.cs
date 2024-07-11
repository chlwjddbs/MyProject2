using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialog
{
    public int logNum;          //xml���� ������ Dialog ��ȣ
    public string talkerImage;        //dialog ui�� ǥ���� �̹��� 
    public string talkerName;   //dialog���� ���ϰ� �ִ� ���
    public string sentence;     //dialog ����

    public int nextlogNum;      //������ ������ dialog ��ȣ.
                                //ex) 0�� dialog�� ������
                                //case1) nextlogNum�� -1 : 0�� dialog�� ������ ��ȭ ���� 
                                //case2) nextlogNum�� 2 : 0�� dialog�� ������ 2�� dialog ���
}
