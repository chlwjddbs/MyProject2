using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.Localization.Components;

public class SkillSlot : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public SkillItem skillItem;
    public Image skillImage;
    public LocalizeStringEvent skillName;

    public void AddSkill(SkillItem _skillItem)
    {
        skillItem = _skillItem;
        skillImage.sprite = skillItem.itemImege;
        skillName.StringReference.TableEntryReference = skillItem.skillName;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        skillImage.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        skillImage.transform.position = eventData.position;
    }

    //��ų�Ͽ� ��ϵ� ��ų�� ��ų ��ư�� �����Ѵ�.
    public void OnEndDrag(PointerEventData eventData)
    {
        List<GameObject> hoveredObject = eventData.hovered;

        for (int i = 0; i < hoveredObject.Count; i++)
        {
            //�巡�� �� ���� ��ų ��ư�� ��
            if (hoveredObject[i].CompareTag("SkillButton"))
            {
                if (hoveredObject[i].TryGetComponent(out SkillButton skillButton))
                {
                    //��ų ��ư�� ��� ������ ���¶��
                    if (skillButton.isUse)
                    {
                        //���� ���Կ� �ִ� ��ų�� �巡�� �� ��ư�� ����
                        //������ ��ų�� SkillBook���� ����
                        SkillBook.instance.isEquip(skillItem, skillButton.buttonNum);
                    }
                    else
                    {
                        SequenceText.instance.SetSequenceText(null, "��ų�����Ұ�");
                    }
                }
            }
        }
        skillImage.raycastTarget = true;
        skillImage.transform.localPosition = Vector3.zero;

        hoveredObject.Clear();
    }
}
