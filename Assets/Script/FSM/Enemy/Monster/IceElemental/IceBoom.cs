using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBoom : MonoBehaviour
{
    private float countdown = 1.5f;
    public float attackDamage = 10f;
    [SerializeField]private Collider coll;
    private int iceBlastGaugePoint = 50;
    private Queue<GameObject> bfsQueue = new Queue<GameObject>();
    private List<GameObject> visitSequence = new List<GameObject>();

    private IceElemental iceElemental;

   
    public bool Search(GameObject researchTest)
    {
        bfsQueue.Enqueue(researchTest);
        visitSequence.Add(researchTest);

        while (bfsQueue.Count > 0)
        {
            GameObject nowObj = bfsQueue.Dequeue();
            if(nowObj.TryGetComponent<IceElemental>(out IceElemental value))
            {
                iceElemental = value;
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

    public void SetData(IceElemental _iceElemental)
    {
        iceElemental = _iceElemental;
        coll.enabled = true;
    }

    IEnumerator EndBoom()
    {
        yield return new WaitForSeconds(1.5f);
        coll.enabled = false;
        yield return new WaitForSeconds(.5f);
        iceElemental.iceBoomPool.Release(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Slowly");
            iceElemental.PlayESound("iceBoom");
            coll.enabled = false;
            other.GetComponent<IAttackable>().TakeDamage(attackDamage,null);
            iceElemental.AddBlastGauge(iceBlastGaugePoint);
        }
    }

    private void OnEnable()
    {
        coll.enabled = true;
        StartCoroutine(EndBoom());
    }
}
