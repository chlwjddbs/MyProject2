using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Area_C_Gimmick : MonoBehaviour
{
    public List<PuzzleKeyController> puzzleKeyControllers;

    public GameObject rightGate_1;
    public GameObject leftGate_1;
    public GameObject particeGate_1;

    public GameObject doorTrigger;

    public List<bool> puzzleKey;
    public List<bool> openGate;

    /*
    public GameObject puzlleKey_1;
    public GameObject puzlleKey_2;
    public GameObject puzlleKey_3;
    public GameObject puzlleKey_4;
    public GameObject puzlleKey_5;
    public GameObject puzlleKey_6;

    public ParticleController particle_1;
    public ParticleController particle_2;
    public ParticleController particle_3;
    public ParticleController particle_4;
    public ParticleController particle_5;
    public ParticleController particle_6;

    public GameObject light_1;
    public GameObject light_2;
    public GameObject light_3;
    public GameObject light_4;
    public GameObject light_5;
    public GameObject light_6;
    */

    private void Update()
    {
        TouchObject();
        OpenGate_0();
    }

    public void TouchObject()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            //hit�� ������Ʈ�� ������ hit�� �ƹ��͵� ���� ������
            //hit�� ����Ϸ����ϸ� NullReferenceException ������ ���´�.
            //�׷��Ƿ� if���� ����� hit �� ������Ʈ�� ���� ��쿡�� �������.
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.TryGetComponent(out SetCursorImage hitObj) == false)
                {
                    return;
                }

                //theDistance ���� ������Ʈ�� �÷��̾���� �Ÿ�
                //actionDis ���� ������Ʈ�� ��ȣ�ۿ��� �ϱ� ���� �Ÿ�
                //�÷��̾ ���� ������Ʈ�� ��ȣ �ۿ� �Ÿ����� �ָ� ������ �ƹ� ���� ���� �ʴ´�.
                if (hitObj.theDistance > hitObj.actionDis)
                {
                    return;
                }
                /*
                if (hit.transform.GetComponent<SetCursorImage>().theDistance > hit.transform.GetComponent<SetCursorImage>().actionDis)
                {
                    return;
                }
                */

                if (hit.transform.gameObject == puzzleKeyControllers[0].gameObject)
                {
                    puzzleKeyControllers[0].ParticleOnOff();
                    puzzleKeyControllers[1].ParticleOnOff();
                    puzzleKeyControllers[2].ParticleOnOff();

                }
                else if (hit.transform.gameObject == puzzleKeyControllers[1].gameObject)
                {
                    puzzleKeyControllers[1].ParticleOnOff();
                    puzzleKeyControllers[0].ParticleOnOff();
                    puzzleKeyControllers[3].ParticleOnOff();
                }
                else if (hit.transform.gameObject == puzzleKeyControllers[2].gameObject)
                {
                    puzzleKeyControllers[2].ParticleOnOff();
                    puzzleKeyControllers[0].ParticleOnOff();
                    puzzleKeyControllers[3].ParticleOnOff();
                }
                else if (hit.transform.gameObject == puzzleKeyControllers[3].gameObject)
                {
                    puzzleKeyControllers[3].ParticleOnOff();
                    puzzleKeyControllers[1].ParticleOnOff();
                    puzzleKeyControllers[2].ParticleOnOff();
                }

                /*
                if (hit.transform.gameObject == puzlleKey_1)
                {
                    light_1.SetActive(!light_1.activeSelf);
                    light_2.SetActive(!light_2.activeSelf);
                    light_3.SetActive(!light_3.activeSelf);
                    particle_1.ParticleOnOff();
                    particle_2.ParticleOnOff();
                    particle_3.ParticleOnOff();

                }
                else if (hit.transform.gameObject == puzlleKey_2)
                {
                    light_2.SetActive(!light_2.activeSelf);
                    light_1.SetActive(!light_1.activeSelf);
                    light_4.SetActive(!light_4.activeSelf);
                    particle_2.ParticleOnOff();
                    particle_1.ParticleOnOff();
                    particle_4.ParticleOnOff();


                }
                else if (hit.transform.gameObject == puzlleKey_3)
                {
                    light_3.SetActive(!light_3.activeSelf);
                    light_1.SetActive(!light_1.activeSelf);
                    light_4.SetActive(!light_4.activeSelf);
                    particle_3.ParticleOnOff();
                    particle_1.ParticleOnOff();
                    particle_4.ParticleOnOff();


                }
                else if (hit.transform.gameObject == puzlleKey_4)
                {
                    light_4.SetActive(!light_4.activeSelf);
                    light_2.SetActive(!light_2.activeSelf);
                    light_3.SetActive(!light_3.activeSelf);
                    particle_4.ParticleOnOff();
                    particle_2.ParticleOnOff();
                    particle_3.ParticleOnOff();
                }
                */
            }
        }
    }

    public void OpenGate_0()
    {
        if(puzzleKeyControllers[0].isOpen && puzzleKeyControllers[1].isOpen && puzzleKeyControllers[2].isOpen && puzzleKeyControllers[3].isOpen && openGate[0] == false)
        {
            openGate[0] = true;
            rightGate_1.GetComponent<BoxCollider>().isTrigger = false;
            leftGate_1.GetComponent<BoxCollider>().isTrigger = false;
            particeGate_1.GetComponent<ParticleSystem>().Play();
            particeGate_1.transform.GetChild(0).gameObject.SetActive(true);
            Destroy(doorTrigger);
        }
    }

    public void SaveData()
    {
        for (int i = 0; i < puzzleKeyControllers.Count; i++)
        {
            puzzleKey[i] = puzzleKeyControllers[i].isOpen;
        }
        DataManager.instance.userData.puzzleKey = puzzleKey;

        DataManager.instance.userData.openGate = openGate;
    }

    public void LoadData()
    {
        openGate = DataManager.instance.userData.openGate.ConvertAll(b => b);

        for (int i = 0; i < openGate.Count; i++)
        {
            if (openGate[i])
            {
                LoadGate(i);
            }
        }
        
        for (int i = 0; i < puzzleKeyControllers.Count; i++)
        {
            try
            {
                puzzleKeyControllers[i].LoadData(DataManager.instance.userData.puzzleKey[i]);
            }
            catch
            {
                Debug.Log("Area_C : ����� ������ ����");
            }
            
        }
    }

    public void LoadGate(int _num)
    {
        switch (_num)
        {
            case 0:

                rightGate_1.GetComponent<BoxCollider>().isTrigger = false;
                leftGate_1.GetComponent<BoxCollider>().isTrigger = false;
                particeGate_1.GetComponent<ParticleSystem>().Play();
                particeGate_1.transform.GetChild(0).gameObject.SetActive(true);
                Destroy(doorTrigger);

                break;

            case 1:
                break;

            default:
                break;
        }
    }

}
