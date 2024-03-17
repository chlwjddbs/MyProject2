using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(PlayerSight))]
public class PlayerSightEditor : Editor
{
    public Vector3 posOffset = new Vector3(0,1.5f,0);

    void OnSceneGUI()
    {  
        PlayerSight fow = (PlayerSight)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(fow.transform.position, Vector3.up, Vector3.forward, 361, fow.sightRaduis);
        Vector3 viewAngleA = fow.DirFromAngle(-fow.sightAngle / 2, false);
        Vector3 viewAngleB = fow.DirFromAngle(fow.sightAngle / 2, false);

        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleA * fow.sightRaduis);
        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleB * fow.sightRaduis);

        Handles.color = Color.red;
        foreach (Transform visible in fow.viewTarget)
        {
            Handles.DrawLine(fow.transform.position, visible.transform.position);
        }
    }
}
