using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ReleasePool : MonoBehaviour
{
    private IObjectPool<GameObject> poolObj;

    public void SetPool(IObjectPool<GameObject> _poolObj , float _timer)
    {
        poolObj = _poolObj;
        StartCoroutine(ReleaseObj(_timer)); 
    }

    IEnumerator ReleaseObj(float _timer)
    {
        yield return new WaitForSeconds(_timer);
        poolObj.Release(gameObject);
    }
}
