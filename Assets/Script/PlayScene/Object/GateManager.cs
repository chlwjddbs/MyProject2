using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class GateManager : MonoBehaviour
{
    [Serializable]
    public class GateInfo
    {
        public string gateName;
        public int gateNum;
        public Vector3 coordinate;
        public bool isActive;
        public GateInfo(string _gateName, int _gateNum, Vector3 _coordinate, bool _isActive = false)
        {
            gateName = _gateName;
            gateNum = _gateNum;
            coordinate = _coordinate;
            isActive = _isActive;
        }
    }

    public static GateManager instence;

    public List<TeleportGate> gateList;

    public Dictionary<int, GateInfo> gateDic;

    public TeleportGateUI teleportGateUI;
    public UnityAction getUI;

    public bool isOpen;

    public GameObject player;

    public GameObject gateSlotPrefab;

    //UI�� ǥ�� ���ѵ� gateSlot�� �ִ� �θ� ������Ʈ
    public Transform GateListUI;

    //�̵��Ϸ��� ����Ʈ�� ����Ʈ�� �� ������� Ȯ��
    public int startingGate;

    private bool isAllgate  = false;

    private void Awake()
    {
        if(instence != null) 
        {
            Destroy(gameObject);
            return;
        }

        instence = this;
        DontDestroyOnLoad(gameObject);

    }

    public void SetData()
    {
        gateDic = new Dictionary<int, GateInfo>();
        getUI?.Invoke();
        teleportGateUI.CloseUI();

        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).TryGetComponent(out TeleportGate _gate))
            {
                _gate.gateNumber = i;
                _gate.SetData(this);
                gateList.Add(_gate);
            }
        }
    }

    public void LoadData()
    {
        foreach (var gateInfo in GameData.instance.userData.gateInfo)
        {
            Debug.Log(gateInfo.gateName);
            ActiveGateSlot(gateInfo.gateNum, gateInfo.gateName, gateInfo.coordinate, gateInfo.isActive);
            gateList[gateInfo.gateNum].LoadData();
        }
    }

    public void SaveData()
    {
        GameData.instance.userData.gateNum = new List<int>(gateDic.Keys);
        GameData.instance.userData.gateInfo = new List<GateInfo>(gateDic.Values);

        foreach (var _gate in GameData.instance.userData.gateNum)
        {
            Debug.Log(_gate);
        }
        foreach (var _gate in GameData.instance.userData.gateInfo)
        {
            Debug.Log(_gate.gateName + " " + _gate.coordinate);
        }
    }

    public void Move(int _gateNum)
    {
        //�̵� �� ����Ʈ �ѹ��� ���� ����Ʈ ��ųʸ��� _value �� �����´�.
        if (gateDic.TryGetValue(_gateNum, out GateInfo _value))
        {
            //_value�� Ȱ��ȭ �� �����̰�
            if (_value.isActive)
            {
                //���� ����Ʈ�� �̵��� ����Ʈ�� ���ٸ� �ٸ� ����Ʈ�� ������ �޶�� �޽��� ���
                if (startingGate == _gateNum)
                {
                    SequenceText.instance.SetSequenceText(null, "OtherGate");
                }
                //���� ����Ʈ�� �̵��� ����Ʈ�� �ٸ��� �̵��� ����Ʈ�� ��ġ�� �̵�
                else
                {
                    player.GetComponent<NavMeshAgent>().enabled = false;
                    player.transform.position = gateDic[_gateNum].coordinate;
                    teleportGateUI.CloseUI();
                    player.GetComponent<NavMeshAgent>().enabled = true;
                }
            }
        }
    }

   

    /*
    public void CreateGateSlot(int _gateNum, string _gateName, Vector3 _gateCoordinate)
    {
        gateDic.Add(_gateName, _gateCoordinate);
        GameObject _gateSlot = Instantiate(gateSlot, slotList);
        _gateSlot.GetComponent<GateSlot>().SetGate(_gateNum, _gateName, _gateCoordinate, this.gameObject);
    }
    */

    //GateManager���� �����ǰ� �ִ� ����Ʈ���� ��� ������ ���°� �Ǿ����� �÷��̾�� UI�� �����Ѵ�.
    public void ActiveGateSlot(int _gateNum, string _gateName, Vector3 _gateCoordinate, bool _isAcive = false)
    {
        gateDic.Add(_gateNum, new GateInfo(_gateName, _gateNum ,_gateCoordinate, _isAcive));
        Debug.Log(GateListUI.GetChild(_gateNum));
        GateListUI.GetChild(_gateNum).gameObject.SetActive(true);
        GateListUI.GetChild(_gateNum).gameObject.GetComponent<GateSlot>().ActiveGate(_gateNum, _gateName, _gateCoordinate, this);
    }

    public void CheckStartingPoint(int _startGateNum)
    {
        startingGate = _startGateNum;
    }
}
