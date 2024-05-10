using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.AI
{
    public class SearchTarget : MonoBehaviour
    {
        public float detectingRange = 5f;

        public LayerMask targetMask;

        private Transform nearestTarget;
        public Transform NearestTarget { get { return nearestTarget; } }

        private float distanceToTarget;
        public float DistanceToTarget { get { return distanceToTarget;} }

        [SerializeField]
        private float delayTime = 0.1f;

        private void Start()
        {
            StartCoroutine("UpdateTargetDelay", delayTime);
        }

        IEnumerator UpdateTargetDelay(float delay)
        {
            while(true)
            {
                yield return new WaitForSeconds(delay);
                UpdateTarget();
            }
        }

        void UpdateTarget()
        {
            distanceToTarget = 0f;
            float shotestDistance = Mathf.Infinity;
            Transform nearestEnemy = null;

            Collider[] enemies = Physics.OverlapSphere(transform.position, detectingRange, targetMask);

            foreach (Collider enemy in enemies)
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if(distance < shotestDistance)
                {
                    shotestDistance = distance;
                    nearestEnemy = enemy.transform;
                }
            }

            if(nearestEnemy != null && shotestDistance <= detectingRange)
            {
                distanceToTarget = shotestDistance;
                nearestTarget = nearestEnemy;
            }
            else
            {
                nearestTarget = null;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, detectingRange);
        }
    }
}