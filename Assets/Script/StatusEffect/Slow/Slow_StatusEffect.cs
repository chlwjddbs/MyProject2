using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slow_StatusEffect : StatusEffect
{
    [Tooltip("´ë»ó¿¡°Ô Àû¿ëµÉ µÐÈ­·ü(%)")]
    [Range(0.0f,10.0f)]
    public float slowRate;

    public float count;

    public void Update()
    {
        if (count <= duration)
        {
            count+= Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        target.RemoveStatusEffect(effctName);
    }
}
