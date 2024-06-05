using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.InputSystem;

public class SkillButton : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    //현재 버튼에 장착된 스킬
    public SkillItem skillItem;
    public SkillManager skill;

    //현재 버튼의 번호
    public int buttonNum;

    public Image skillImage;

    //장착 된 스킬이 들어갈 위치
    public Transform skillPos;
    
    //해당 버튼의 옵션
    public KeyOption keyOption;

    [SerializeField] private TextMeshProUGUI keyCodeText;

    //스킬 위치 변경 시 필요한 공간
    public GameObject Dragtemp;
    private Transform _startParent;

    //스킬 쿨타임 쿨타임이 차오르는 연출을 할 이미지
    public Image coolTimeFilled;
    //잔량 쿨타임 표시 Text
    public TextMeshProUGUI coolTimeAlarm;

    //남은 쿨타임 수치
    private float remainingTime
    {
        get 
        {
            if (skill != null /*& skill.TryGetComponent(out SkillManager _equipSkill)*/)
            {
                return skill.remainingTime;
            }
            else
            {
                return 0;
            }
        }
        set 
        {
            if (skill != null)
            {
                skill.remainingTime = value;
            }
        }
    }

    //버튼 사용 가능 여부
    public bool isUse = true;

    public Player player;

    //알림창을 사용한 가능한 상태인지 체크
    private bool usetext = true;

    private ControllOption controlOption;

    // Update is called once per frame
    void Update()
    {
        //버튼에 스킬이 장착되었을 떄만 사용가능
        if (skill != null)
        {
            CoolTimeAlarm();
            //keyOption에 할당된 keycode를 눌렀을 시 스킬 사용.
            if (Input.GetKeyDown(controlOption.bindKey_Dic[keyOption].bindKey))
            {
                //플레이어가 액션을 취하지 않거나 쿨타임이 찼을 경우
                if (UseButton())
                {
                    skill.UseSkill();
                    /*
                    //남은 마나가 있을 경우
                    if (playerController.playerStatus.CheckMana(skill.cunsumeMana))
                    {
                        //스킬 사용
                        skill.UseSkill(playerController);
                        AudioManager.instance.PlayerSkillSound(buttonNum);
                    }
                    */
                }
                else
                {
                    Debug.Log("스킬을 사용할 수 없는 상태입니다.");
                }
            }
        }
    }

    //KeyCode가 변경 되었을때 변경된 KeyCode를 표시해준다.
    public void ChangeKeyCode(KeyOption _option, KeyCode _keyCode)
    {
        if (keyOption == _option)
        {
            if(_keyCode == KeyCode.None)
            {
                //Debug.Log($"{gameObject.name}키는 삭제됐습니다.");
                keyCodeText.text = null;
            }
            else
            {
                //Debug.Log($"{gameObject.name}키는 {_keyCode}으로 변경되었습니다.");
                keyCodeText.text = _keyCode.ToString();
            }
        }
        /*
        if (controlOption.bindKey_Dic.TryGetValue(keyOption, out KeyOptionInfo vale))
        {
            if(keyCodeText.text == vale.bindKey.ToString())
            {
                Debug.Log($"{gameObject.name}키는 변경 안해도 됩니다.");
                return;
            }
            if (vale.bindKey.ToString() == "None")
            {
                Debug.Log($"{gameObject.name}키는 비어 있습니다.");
                keyCodeText.text = null;
            }
            else
            {
                Debug.Log($"{gameObject.name}키는 {vale.bindKey.ToString()}으로 변경되었습니다.");
                keyCodeText.text = vale.bindKey.ToString();
            }
        }
        */
    }

    //DataSetting
    public void SetData()
    {
        controlOption = OptionManager.instance.controllOption;
        controlOption.changeKeyCode += ChangeKeyCode;
     
        //KeyCode Text 표시
        if (controlOption.bindKey_Dic.TryGetValue(keyOption, out KeyOptionInfo vale))
        {
            ChangeKeyCode(vale.keyOption,vale.bindKey);
        }

        //처음 데이터가 세팅되면 스킬 이미지, 남은 쿨타임을 표시해주는 text, 남을 쿨타임을 보여주는 Filled를 모두 초기화 해준다.
        skillImage.enabled = false;
        coolTimeAlarm.text = null;
        coolTimeFilled.enabled = false;

        //newGame이면 SkillButton의 스킬을 관리하는 SkillBook Class에 현재 스킬 Button 정보를 넘겨주어 저장한다.
        //게임 플레이 중 변경되는 Button들의 정보를 수월하게 관리하기 위해 미리 딕셔너리 정보를 생성한다. 
        if (GameData.instance.newGame)
        {
            SkillBook.instance.skillButtonInfo.Add(buttonNum, new SkillBook.SetEquipSkill(skillItem, remainingTime));
        }
    }

    public void LoadData()
    {
        //스킬버튼을 관리하는 딕셔너리에서 현재 스킬 버튼의 넘버를 키값으로 가지는 벨류를 찾아 값을 할당한다.
        if(SkillBook.instance.skillButtonInfo.TryGetValue(buttonNum,out SkillBook.SetEquipSkill value))
        {
            LoadSkill(value.equipSkill, value.coolTime);
        }
    }

    public void SaveData()
    {
        //SetData 과정에서 스킬버튼을 관리하는 딕셔너리를 미리 생성해 두었기 때문에 현재 정보를 만들어 딕셔너리에 벨류만 교체해주면 된다.
        SkillBook.SetEquipSkill saveData = new SkillBook.SetEquipSkill(skillItem, remainingTime);
        SkillBook.instance.skillButtonInfo[buttonNum] = saveData;
    }

    public void EquipSkill(SkillItem _skillItem)
    {
        if (_skillItem != null)
        {
            AudioManager.instance.AddSkillSound(_skillItem.skillSound, buttonNum);
            //스킬 장착시 SkillBook 스크립트의 장착 스킬 목록에 현재 버튼위치에 장착된 스킬을 저장
            //ex) 2번째 스킬 버튼에 heal스킬이 장착되면 장착 스킬 목록 리스트 2번쨰에 heal스킬 추가
            SkillBook.instance.equipSkill[buttonNum] = _skillItem;
            skillItem = _skillItem;
            GameObject _skill = Instantiate(skillItem.skill, skillPos);
            skill = _skill.GetComponent<SkillManager>();
            skill.SetSkill(player);
            skillImage.sprite = skillItem.itemImege;
            skillImage.enabled = true;
            CheckButtonState();
        }
    }

    //게임을 로드하면 사용했던 스킬의 쿨타임을 적용해주어야 하기 때문에 EquipSkill에서 쿨타임을 적용하는 형태로 적용한다. 
    public void LoadSkill(SkillItem _skillItem , float _coolTime)
    {
        if(_skillItem == null)
        {
            return;
        }
        AudioManager.instance.AddSkillSound(_skillItem.skillSound, buttonNum);
        SkillBook.instance.equipSkill[buttonNum] = _skillItem;
        skillItem = _skillItem;
        GameObject _skill = Instantiate(skillItem.skill, skillPos);
        skill = _skill.GetComponent<SkillManager>();
        skillImage.sprite = skillItem.itemImege;
        skillImage.enabled = true;
        remainingTime = _coolTime;
        CheckButtonState();
    }

    public void UnequipSkill()
    {
        //장착 해제 시 장착한 스킬 목록에서 제거
        SkillBook.instance.equipSkill[buttonNum] = null;
        AudioManager.instance.RemoveSkillSound(buttonNum);
        skillItem = null;
        if (skill != null)
        {
            Destroy(skill.gameObject);
        }
        //null을 해주지 않으면 missing으로 됌. 에러는 뜨지 않았음
        skill = null;
        skillImage.sprite = null;
        skillImage.enabled = false;
        coolTimeFilled.enabled = false;
        coolTimeAlarm.text = null;
        isUse = true;
        remainingTime = 0;
    }

    //현재 버튼이 사용가능한 상태인지 체크.
    public bool UseButton()
    {
        //플레이어가 다른 액션을 취하지 않아 스킬을 사용가능하고 스킬이 쿨타임이 회복된 등 사용 가능한 상태일때 True 반환
        if (player.ActionRestriction() && isUse)
        {
            return true;
        }
        else if (player.isAction)
        {
            if (usetext)
            {
                usetext = false;
                SequenceText.instance.SetSequenceText(null, "스킬사용불가");
                Invoke("isUseText", .5f);
            }
            return false;
        }
        else if (!isUse)
        {
            if (usetext)
            {
                usetext = false;
                SequenceText.instance.SetSequenceText(null, "스킬준비중");
                Invoke("isUseText", .5f);
            }
            return false;
        }
        else
        {
            return false;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(skillItem == null)
        {
            return;
        }

        _startParent = transform.GetChild(0);
        skillImage.raycastTarget = false;
        skillImage.transform.SetParent(Dragtemp.transform);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (skillItem == null)
        {
            return;
        }
        skillImage.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (skillItem == null)
        {
            return;
        }
        List<GameObject> hoveredObject = eventData.hovered;
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            //쿨타임 돌고 있는 스킬은 장착 해제 불가
            if (isUse)
            {
                UnequipSkill();
            }
            else
            {
                SequenceText.instance.SetSequenceText(null, "스킬해제불가");
            }
        }
        else
        {
            for (int i = 0; i < hoveredObject.Count; i++)
            {
                if (hoveredObject[i].CompareTag("SkillButton"))
                {
                    //현재 버튼의 스킬과 drop위치의 아이템을 교환한다.
                    swapSkill(hoveredObject[i].GetComponent<SkillButton>());
                }
                /*
                //drop 위치가 스킬 버튼이고
                if (hoveredObject[i].CompareTag("SkillButton"))
                {
                    if (hoveredObject[i].name == this.name)
                    {
                        continue;
                    }
                    else
                    {
                        //현재 버튼의 스킬과 drop위치의 아이템을 교환한다.
                        swapSkill(hoveredObject[i].GetComponent<SkillButton>());
                        break;
                    }
                }
                */
            }
        }

        skillImage.raycastTarget = true;
        skillImage.transform.SetParent(_startParent);
        skillImage.transform.localPosition = Vector3.zero;
        hoveredObject.Clear();
    }

    //현재 스킬버튼의 스킬과 drop버튼의 스킬의 위치를 바꾼다. (현재 슬롯의 스킬을 넘겨줘야하기 떄문)
    public void swapSkill(SkillButton _dropSlot)
    {
        //빈 슬롯에 현재 스킬을 저장하고
        SkillItem dragSkill = skillItem;
        float dragremainingTime = skill.remainingTime;

        //현재 스킬 버튼을 비운다.
        UnequipSkill();

        //비워진 현재 스킬 버튼에 drop버튼의 스킬을 장착한다.
        skillItem = _dropSlot.skillItem;
        EquipSkill(skillItem);

        //drop스킬이 존재 한다면 drop스킬의 잔량 쿨타임 수치도 가져온다.
        //만약 drop스킬이 없다면 잔량 쿨타임 수치가 없기 때문에 가져오지 않는다.
        if (_dropSlot.skillItem != null)
        {
            skill.SwapSkill(_dropSlot.skill.remainingTime);
        }

        //drop슬롯을 비우고
        _dropSlot.UnequipSkill();
        //drop슬롯에 현재 스킬을 장착한다.
        _dropSlot.EquipSkill(dragSkill);
        //Debug.Log(dragremainingTime);

        //현재 버튼의 스킬을 _dropSlot으로 넘겨준것이기 때문에 dropSlot에서는 위 코드의 if (_dropSlot.skillItem != null) 와 같은 방어 코드가 불필요함.
        _dropSlot.skill.SwapSkill(dragremainingTime);

        _dropSlot.skillImage.raycastTarget = true;
    }

    public void swapSkill1(SkillButton _dropSlot)
    {
        //빈 슬롯에 현재 스킬을 저장하고
        SkillItem dragSkill = skillItem;
        float dragremainingTime = skill.GetComponent<SkillManager>().remainingTime;

        //현재 스킬 버튼을 비운다.
        UnequipSkill();

        //비워진 현재 스킬 버튼에 drop버튼의 스킬을 장착한다.
        skillItem = _dropSlot.skillItem;
        EquipSkill(skillItem);

        //drop스킬이 존재 한다면 drop스킬의 잔량 쿨타임 수치도 가져온다.
        //만약 drop스킬이 없다면 잔량 쿨타임 수치가 없기 때문에 가져오지 않는다.
        if (_dropSlot.skillItem != null)
        {
            skill.GetComponent<SkillManager>().SwapSkill(_dropSlot.skill.GetComponent<SkillManager>().remainingTime);
        }

        //drop슬롯을 비우고
        _dropSlot.UnequipSkill();
        //drop슬롯에 현재 스킬을 장착한다.
        _dropSlot.EquipSkill(dragSkill);
        //Debug.Log(dragremainingTime);

        //현재 버튼의 스킬을 _dropSlot으로 넘겨준것이기 때문에 dropSlot에서는 위 코드의 if (_dropSlot.skillItem != null) 와 같은 방어 코드가 불필요함.
        _dropSlot.skill.GetComponent<SkillManager>().SwapSkill(dragremainingTime);

        _dropSlot.skillImage.raycastTarget = true;
    }

    public void CoolTimeAlarm()
    {
        //현재 스킬의 잔량 쿨타임을 가져온다.
        //remainingTime = skill.remainingTime;

        //쿨타임이 전부 회복되었을 시 한번 만 실행한다.
        if (remainingTime <= 0 && isUse == false)
        {
            //text를 보이지 않게 한다.
            coolTimeAlarm.text = null;
            coolTimeFilled.enabled = false;

            //스킬 아이콘을 밝게 표시하여 가시성을 높인다.
            skillImage.color = new Color(1, 1, 1);

            //스킬버턴을 사용가능하게 해준다.
            isUse = true;
        }

        //스킬 사용 후 쿨타임 모드로 들어갔을 시 한번 만 실행한다.
        else if (remainingTime > 0 && isUse == true)
        {
            //스킬 쿨타임이 회복되고 있음을 보여주기 위한 coolTiemFilled 이미지를 활성화 한다.
            coolTimeFilled.enabled = true;

            //스킬 아이콘을 어둡게 만들어 사용 불가함을 표시해준다.
            skillImage.color = new Color(.4f, .4f, .4f);

            //스킬버튼 사용 불가.
            isUse = false;

        }

        if (remainingTime <= 0)
        {
            //위 조건(remainingTime <= 0)이 없을 시 else 문에서 위 조건도 true로 들어와 쿨타임이 전부 회복 되었음에도 0이하의 숫자를 표기하는 오류 발생
        }
        else if (remainingTime < 1 && remainingTime > 0)
        {
            //쿨타임이 1초 미만의 소수점일 시 정확하게 확인 할 수 있게 소수점 1자리까지 쿨타임을 표시.
            coolTimeAlarm.text = remainingTime.ToString("F1");
            coolTimeFilled.fillAmount = (remainingTime / skill.coolTime);
        }
        else
        {
            //쿨타임이 1초 이상일 때 소수점은 확인할 필요가 없으므로 자연수만 표시하도록 한다.
            //ToString("F")의 경우 반올림으로 표시해주기 때문에 버림으로 묶어서 표시
            //쿨타임은 높은 숫자부터 0(쿨타임 회복)까지 감소하게 된다.
            //이때 1초 이상의 쿨타임은 자연수까지만 표기가 되는데 4.5~5초를 모두 5초로 표기하기 때문에 4.x 초로 떨어지면 4초로 보이게 하기 위해 Mathf.Floor로 버림표기를 해준다.
            coolTimeAlarm.text = (Mathf.Floor(remainingTime)).ToString("F0");
            coolTimeFilled.fillAmount = (remainingTime / skill.coolTime);
        }
    }

    //현재 스킬 버튼의 상태를 체크해준다.
    public void CheckButtonState()
    {
        /*
        //현재 스킬 버튼에 장착된 스킬이 있다면 잔량 쿨타임을 받아온다.
        if (skillItem != null)
        {
            remainingTime = skill.remainingTime;
        }
        */
        //잔량 쿨타임이 남아 있지 않으면 스킬 버튼을 사용가능하게 활성화 한다.
        if (remainingTime <= 0)
        {
            coolTimeAlarm.text = null;
            coolTimeFilled.enabled = false;
            skillImage.color = new Color(1, 1, 1);
            isUse = true;
        }

        //남은 쿨타임에 맞춰 스킬 버튼을 비활성화 함과 동시에 cootime알람을 설정한다.
        else if (remainingTime < 1 && remainingTime > 0)
        {
            coolTimeAlarm.text = remainingTime.ToString("F1");
            coolTimeFilled.fillAmount = (remainingTime / skill.coolTime);
            coolTimeFilled.enabled = true;
            skillImage.color = new Color(.4f, .4f, .4f);
            isUse = false;
        }
        else
        {
            coolTimeAlarm.text = (Mathf.Floor(remainingTime)).ToString("F0");
            coolTimeFilled.fillAmount = (remainingTime / skill.coolTime);
            coolTimeFilled.enabled = true;
            skillImage.color = new Color(.4f, .4f, .4f);
            isUse = false;
        }
    }

    private void isUseText()
    {
        usetext = true;
    }
}
