using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class SkillButton : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    //���� ��ư�� ������ ��ų
    public SkillItem skillItem;
    public SkillManager skill;

    //���� ��ư�� ��ȣ
    public int buttonNum;

    public Image skillImage;

    //���� �� ��ų�� �� ��ġ
    public Transform skillPos;
    
    //�ش� ��ư�� Ʈ���� ��ư
    public string keyCode;

    //��ų ��ġ ���� �� �ʿ��� ����
    public GameObject Dragtemp;
    private Transform _startParent;

    //��ų ��Ÿ�� ��Ÿ���� �������� ������ �� �̹���
    public Image coolTimeFilled;
    //�ܷ� ��Ÿ�� ǥ�� Text
    public TextMeshProUGUI coolTimeAlarm;

    //���� ��Ÿ�� ��ġ
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
    

    //��ư ��� ���� ����
    public bool isUse = true;

    public PlayerController playerController;

    private bool usetext = true;

    // Update is called once per frame
    void Update()
    {
        //��ư�� ��ų�� �����Ǿ��� ���� ��밡��
        if (skill != null)
        {
            CoolTimeAlarm();

            //keyCode�� ���� �� ��ų �ߵ�
            if (Input.GetKeyDown(keyCode))
            {
                //�÷��̾ �׼��� ������ �ʰų� ��Ÿ���� á�� ���
                if (UseButton())
                {
                    //���� ������ ���� ���
                    if (playerController.playerStatus.CheckMana(skill.cunsumeMana))
                    {
                        //��ų ���
                        skill.UseSkill(playerController);
                        AudioManager.instance.PlayerSkillSound(buttonNum);
                    }
                }
                else
                {
                    Debug.Log("��ų�� ����� �� ���� �����Դϴ�.");
                }
            }
        }
    }

    public void SetData()
    {
        skillImage.enabled = false;
        coolTimeAlarm.text = null;
        coolTimeFilled.enabled = false;

        if (DataManager.instance.newGame)
        {
            SkillBook.instance.skillButtonInfo.Add(buttonNum, new SkillBook.SetEquipSkill(keyCode, skillItem, remainingTime));
        }
        else
        {
            LoadData();
        }
    }

    public void LoadData()
    {
        if(SkillBook.instance.skillButtonInfo.TryGetValue(buttonNum,out SkillBook.SetEquipSkill value))
        {
            keyCode = value.keyCode;
            LoadSkill(value.equipSkill, value.coolTime);
        }
    }

    public void SaveData()
    {
        SkillBook.SetEquipSkill saveData = new SkillBook.SetEquipSkill(keyCode, skillItem, remainingTime);
        SkillBook.instance.skillButtonInfo[buttonNum] = saveData;
    }

    public void EquipSkill(SkillItem _skillItem)
    {
        if (_skillItem != null)
        {
            AudioManager.instance.AddSkillSound(_skillItem.skillSound, buttonNum);
            //��ų ������ SkillBook ��ũ��Ʈ�� ���� ��ų ��Ͽ� ���� ��ư��ġ�� ������ ��ų�� ����
            //ex) 2��° ��ų ��ư�� heal��ų�� �����Ǹ� ���� ��ų ��� ����Ʈ 2������ heal��ų �߰�
            SkillBook.instance.equipSkill[buttonNum] = _skillItem;
            skillItem = _skillItem;
            GameObject _skill = Instantiate(skillItem.skill, skillPos);
            skill = _skill.GetComponent<SkillManager>();
            skillImage.sprite = skillItem.itemImege;
            skillImage.enabled = true;
            CheckButtonState();
        }
    }

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
        //���� ���� �� ������ ��ų ��Ͽ��� ����
        SkillBook.instance.equipSkill[buttonNum] = null;
        AudioManager.instance.RemoveSkillSound(buttonNum);
        skillItem = null;
        if (skill != null)
        {
            Destroy(skill.gameObject);
        }
        //null�� ������ ������ missing���� ��. ������ ���� �ʾ���
        skill = null;
        skillImage.sprite = null;
        skillImage.enabled = false;
        coolTimeFilled.enabled = false;
        coolTimeAlarm.text = null;
        isUse = true;
        remainingTime = 0;

    }

    public bool UseButton()
    {
        if (!PlayerController.isAction && isUse)
        {
            return true;
        }
        else if (PlayerController.isAction)
        {
            if (usetext)
            {
                usetext = false;
                SequenceText.instance.SetSequenceText(null, "��ų���Ұ�");
                Invoke("isUseText", .5f);
            }
            return false;
        }
        else if (!isUse)
        {
            if (usetext)
            {
                usetext = false;
                SequenceText.instance.SetSequenceText(null, "��ų�غ���");
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
            //��Ÿ�� ���� �ִ� ��ų�� ���� ���� �Ұ�
            if (isUse)
            {
                UnequipSkill();
            }
            else
            {
                SequenceText.instance.SetSequenceText(null, "��ų�����Ұ�");
            }
        }
        else
        {
            for (int i = 0; i < hoveredObject.Count; i++)
            {
                if (hoveredObject[i].CompareTag("SkillButton"))
                {
                    //���� ��ư�� ��ų�� drop��ġ�� �������� ��ȯ�Ѵ�.
                    swapSkill(hoveredObject[i].GetComponent<SkillButton>());
                }
                /*
                //drop ��ġ�� ��ų ��ư�̰�
                if (hoveredObject[i].CompareTag("SkillButton"))
                {
                    if (hoveredObject[i].name == this.name)
                    {
                        continue;
                    }
                    else
                    {
                        //���� ��ư�� ��ų�� drop��ġ�� �������� ��ȯ�Ѵ�.
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

    //���� ��ų��ư�� ��ų�� drop��ư�� ��ų�� ��ġ�� �ٲ۴�. (���� ������ ��ų�� �Ѱ�����ϱ� ����)
    public void swapSkill(SkillButton _dropSlot)
    {
        //�� ���Կ� ���� ��ų�� �����ϰ�
        SkillItem dragSkill = skillItem;
        float dragremainingTime = skill.remainingTime;

        //���� ��ų ��ư�� ����.
        UnequipSkill();

        //����� ���� ��ų ��ư�� drop��ư�� ��ų�� �����Ѵ�.
        skillItem = _dropSlot.skillItem;
        EquipSkill(skillItem);

        //drop��ų�� ���� �Ѵٸ� drop��ų�� �ܷ� ��Ÿ�� ��ġ�� �����´�.
        //���� drop��ų�� ���ٸ� �ܷ� ��Ÿ�� ��ġ�� ���� ������ �������� �ʴ´�.
        if (_dropSlot.skillItem != null)
        {
            skill.SwapSkill(_dropSlot.skill.remainingTime);
        }

        //drop������ ����
        _dropSlot.UnequipSkill();
        //drop���Կ� ���� ��ų�� �����Ѵ�.
        _dropSlot.EquipSkill(dragSkill);
        //Debug.Log(dragremainingTime);

        //���� ��ư�� ��ų�� _dropSlot���� �Ѱ��ذ��̱� ������ dropSlot������ �� �ڵ��� if (_dropSlot.skillItem != null) �� ���� ��� �ڵ尡 ���ʿ���.
        _dropSlot.skill.SwapSkill(dragremainingTime);

        _dropSlot.skillImage.raycastTarget = true;
    }

    public void swapSkill1(SkillButton _dropSlot)
    {
        //�� ���Կ� ���� ��ų�� �����ϰ�
        SkillItem dragSkill = skillItem;
        float dragremainingTime = skill.GetComponent<SkillManager>().remainingTime;

        //���� ��ų ��ư�� ����.
        UnequipSkill();

        //����� ���� ��ų ��ư�� drop��ư�� ��ų�� �����Ѵ�.
        skillItem = _dropSlot.skillItem;
        EquipSkill(skillItem);

        //drop��ų�� ���� �Ѵٸ� drop��ų�� �ܷ� ��Ÿ�� ��ġ�� �����´�.
        //���� drop��ų�� ���ٸ� �ܷ� ��Ÿ�� ��ġ�� ���� ������ �������� �ʴ´�.
        if (_dropSlot.skillItem != null)
        {
            skill.GetComponent<SkillManager>().SwapSkill(_dropSlot.skill.GetComponent<SkillManager>().remainingTime);
        }

        //drop������ ����
        _dropSlot.UnequipSkill();
        //drop���Կ� ���� ��ų�� �����Ѵ�.
        _dropSlot.EquipSkill(dragSkill);
        //Debug.Log(dragremainingTime);

        //���� ��ư�� ��ų�� _dropSlot���� �Ѱ��ذ��̱� ������ dropSlot������ �� �ڵ��� if (_dropSlot.skillItem != null) �� ���� ��� �ڵ尡 ���ʿ���.
        _dropSlot.skill.GetComponent<SkillManager>().SwapSkill(dragremainingTime);

        _dropSlot.skillImage.raycastTarget = true;
    }

    public void CoolTimeAlarm()
    {
        //���� ��ų�� �ܷ� ��Ÿ���� �����´�.
        //remainingTime = skill.remainingTime;

        //��Ÿ���� ���� ȸ���Ǿ��� �� �ѹ� �� �����Ѵ�.
        if (remainingTime <= 0 && isUse == false)
        {
            //text�� ������ �ʰ� �Ѵ�.
            coolTimeAlarm.text = null;
            coolTimeFilled.enabled = false;

            //��ų �������� ��� ǥ���Ͽ� ���ü��� ���δ�.
            skillImage.color = new Color(1, 1, 1);

            //��ų������ ��밡���ϰ� ���ش�.
            isUse = true;
        }

        //��ų ��� �� ��Ÿ�� ���� ���� �� �ѹ� �� �����Ѵ�.
        else if (remainingTime > 0 && isUse == true)
        {
            //��ų ��Ÿ���� ȸ���ǰ� ������ �����ֱ� ���� coolTiemFilled �̹����� Ȱ��ȭ �Ѵ�.
            coolTimeFilled.enabled = true;

            //��ų �������� ��Ӱ� ����� ��� �Ұ����� ǥ�����ش�.
            skillImage.color = new Color(.4f, .4f, .4f);

            //��ų��ư ��� �Ұ�.
            isUse = false;

        }

        if (remainingTime <= 0)
        {
            //�� ����(remainingTime <= 0)�� ���� �� else ������ �� ���ǵ� true�� ���� ��Ÿ���� ���� ȸ�� �Ǿ������� 0������ ���ڸ� ǥ���ϴ� ���� �߻�
        }
        else if (remainingTime < 1 && remainingTime > 0)
        {
            //��Ÿ���� 1�� �̸��� �Ҽ����� �� ��Ȯ�ϰ� Ȯ�� �� �� �ְ� �Ҽ��� 1�ڸ����� ��Ÿ���� ǥ��.
            coolTimeAlarm.text = remainingTime.ToString("F1");
            coolTimeFilled.fillAmount = (remainingTime / skill.coolTime);
        }
        else
        {
            //��Ÿ���� 1�� �̻��� �� �Ҽ����� Ȯ���� �ʿ䰡 �����Ƿ� �ڿ����� ǥ���ϵ��� �Ѵ�.
            //ToString("F")�� ��� �ݿø����� ǥ�����ֱ� ������ �������� ��� ǥ��
            //��Ÿ���� ���� ���ں��� 0(��Ÿ�� ȸ��)���� �����ϰ� �ȴ�.
            //�̶� 1�� �̻��� ��Ÿ���� �ڿ��������� ǥ�Ⱑ �Ǵµ� 4.5~5�ʸ� ��� 5�ʷ� ǥ���ϱ� ������ 4.x �ʷ� �������� 4�ʷ� ���̰� �ϱ� ���� Mathf.Floor�� ����ǥ�⸦ ���ش�.
            coolTimeAlarm.text = (Mathf.Floor(remainingTime)).ToString("F0");
            coolTimeFilled.fillAmount = (remainingTime / skill.coolTime);
        }
    }

    //���� ��ų ��ư�� ���¸� üũ���ش�.
    public void CheckButtonState()
    {
        /*
        //���� ��ų ��ư�� ������ ��ų�� �ִٸ� �ܷ� ��Ÿ���� �޾ƿ´�.
        if (skillItem != null)
        {
            remainingTime = skill.remainingTime;
        }
        */
        //�ܷ� ��Ÿ���� ���� ���� ������ ��ų ��ư�� ��밡���ϰ� Ȱ��ȭ �Ѵ�.
        if (remainingTime <= 0)
        {
            coolTimeAlarm.text = null;
            coolTimeFilled.enabled = false;
            skillImage.color = new Color(1, 1, 1);
            isUse = true;
        }

        //���� ��Ÿ�ӿ� ���� ��ų ��ư�� ��Ȱ��ȭ �԰� ���ÿ� cootime�˶��� �����Ѵ�.
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
