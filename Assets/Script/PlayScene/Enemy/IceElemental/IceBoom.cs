using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBoom : MonoBehaviour
{
    private float countdown = 1.5f;
    public float attackDamage = 10f;
    private GameObject iceElemental;
    [SerializeField]private Collider coll;
    private int iceBlastGaugePoint = 10;
    private Queue<GameObject> bfsQueue = new Queue<GameObject>();
    private List<GameObject> visitSequence = new List<GameObject>();

    private IceElemental2 iceElemental2;

    /*
    // Start is called before the first frame update
    void Start()
    {
        iceElemental = GameObject.Find("IceElemental");
    }
    */

    // Update is called once per frame
    void Update()
    {
        countdown -= Time.deltaTime;
        if(countdown <= 0)
        {
            coll.enabled = false;
        }
    }

    public bool Search(GameObject researchTest)
    {
        bfsQueue.Enqueue(researchTest);
        visitSequence.Add(researchTest);

        while (bfsQueue.Count > 0)
        {
            GameObject nowObj = bfsQueue.Dequeue();
            if(nowObj.TryGetComponent<IceElemental2>(out IceElemental2 value))
            {
                iceElemental2 = value;
                return true;
            }
            
            for (int i = 0; i < nowObj.transform.childCount; i++)
            {
                bfsQueue.Enqueue(nowObj.transform.GetChild(i).gameObject);
                //Debug.Log(nowObj.transform.GetChild(i).name);
                visitSequence.Add(nowObj.transform.GetChild(i).gameObject);
            }
        }

        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Slowly");
            AudioManager.instance.PlayExternalSound("iceBoom");
            coll.enabled = false;
            other.GetComponent<PlayerStatus>().TakeDamage(attackDamage);

            int searchInt = GetComponentsInParent<Transform>().Length - 1;
            //GetComponentInParent<IceElemental2>().AddBlastGauge(iceBlastGaugePoint);
            if (Search(GetComponentsInParent<Transform>()[searchInt].gameObject))
            {
                iceElemental2.AddBlastGauge(iceBlastGaugePoint);
                bfsQueue.Clear();
                visitSequence.Clear();
            }
            else
            {
                Debug.Log("IceElemental2를 찾을 수 없습니다.");
            }
            //iceElemental.GetComponent<IceElementalAction>().AddBlastGauge(iceBlastGaugePoint);
        }
    }
}
