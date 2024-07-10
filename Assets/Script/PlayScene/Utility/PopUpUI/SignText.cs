using UnityEngine;
using UnityEngine.Localization.Components;

public class SignText : Interaction
{
    public Transform sequencetextPos;
    public LocalizeStringEvent sequencetextPrefab;
    public string s_text;
    public string entryReference;
    public float stextLifeTime = 5f;

    private float countdown;
    private float coolTime = 4;

    private DrawOutline drawOutline;

    private void Start()
    {
        drawOutline = GetComponent<DrawOutline>();
    }

    public override void LateUpdate()
    {
        theDistance = Player.checkObjectDis;
        if(countdown > 0)
        {
            countdown -= Time.deltaTime;
        }
    }

    public void OnMouseOver()
    {
        if (theDistance < actionDis)
        {
            DoAction();
        }

        drawOutline.DrawOutLine();
    }

    public override void DoAction()
    {
        player.isObject = true;
        if (Input.GetMouseButtonDown(0))
        {
            SetSequenceText();
        }
    }

    public override void DontAction()
    {
        base.DontAction();
        drawOutline.DrawOrign();
    }

    private void SetSequenceText()
    {
        if (countdown <= 0)
        {
            if (sequencetextPos.childCount >= 3)
            {
                Destroy(sequencetextPos.GetChild(0).gameObject);
            }

            LocalizeStringEvent stext = Instantiate(sequencetextPrefab, sequencetextPos);

            stext.StringReference.TableReference = "SignMessage";
            stext.StringReference.TableEntryReference = entryReference;

            Destroy(stext.gameObject, stextLifeTime);
            countdown = coolTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {      
        if (other.CompareTag("Player"))
        {
            SetSequenceText();
        }
    }
}
