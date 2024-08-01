using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBlast : MonoBehaviour
{
    public GameObject iceBlast;
    private float AttackDamage = 50000;
    private Vector3 blastStartRange = new Vector3(10f, 10f, 10f);
    private Vector3 blastRange = new Vector3(1f,1f,1f);
    private GameObject iceElemental;
    public ParticleSystem magicCircle;
    public ParticleSystem light_1;
    public ParticleSystem light_2;

    public Collider coll;

    public AudioSource audioSource;

    /*
    // Start is called before the first frame update
    void Start()
    {  
        iceElemental = GameObject.Find("IceElemental");
        iceElemental.GetComponent<IceElemental>().blast = BlastGimmick;
    }

    private void Update()
    {
        if (iceElemental.GetComponent<IceElemental>().isDeath)
        {
            StopParticle();
            StopSound();
        }
    }
    */

    public void SetBlast()
    {
        iceBlast.SetActive(false);
        gameObject.SetActive(true);
        transform.localScale = blastStartRange;
        magicCircle.Play();
        light_1.Play();
        light_2.Play();
        audioSource.Play();
    }

    public void BlastGimmick()
    {
        if (transform.localScale.x > 5)
        {
            transform.localScale -= blastRange;
        }
    }

    public void StopParticle()
    {
        magicCircle.Stop();
        light_1.Stop();
        light_2.Stop();
    }

    public void StopSound()
    {
        audioSource.Stop();
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player"))
        {
            if(other.TryGetComponent<IAttackable>(out IAttackable value))
            {
                value.TakeDamage(AttackDamage,null);
                Debug.Log("blast");
            }
            
        }
    }
}
