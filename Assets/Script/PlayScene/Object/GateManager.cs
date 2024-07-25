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

    //UI에 표시 시켜될 gateSlot이 있는 부모 오브젝트
    public Transform GateListUI;

    //이동하려는 게이트가 게이트를 연 장소인지 확인
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
        //이동 할 게이트 넘버를 통해 게이트 딕셔너리를 _value 를 가져온다.
        if (gateDic.TryGetValue(_gateNum, out GateInfo _value))
        {
            //_value가 활성화 된 상태이고
            if (_value.isActive)
            {
                //현재 게이트와 이동할 게이트가 같다면 다른 게이트를 선택해 달라는 메시지 출력
                if (startingGate == _gateNum)
                {
                    SequenceText.instance.SetSequenceText(null, "OtherGate");
                }
                //현재 게이트와 이동할 게이트가 다르면 이동할 게이트의 위치로 이동
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

    //GateManager에서 관리되고 있는 게이트들이 사용 가능한 상태가 되었을때 플레이어에게 UI를 제공한다.
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
