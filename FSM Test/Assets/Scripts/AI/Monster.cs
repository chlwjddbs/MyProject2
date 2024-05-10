using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace MyGame.AI
{
    public class Monster : MonoBehaviour
    {
        protected StateMachine stateMachine;
        protected NavMeshAgent agent;
        protected Animator animator;
        private SearchTarget searchTarget;

        public Transform Target { get { return searchTarget.NearestTarget; } }
        public LayerMask TargetMask { get { return searchTarget.targetMask; } }

        public virtual float AttackRange => 2.0f;
        public virtual bool IsAttackable
        {
            get
            {
                if(Target)
                {
                    float distance = Vector3.Distance(transform.position, Target.position);
                    Debug.Log("distance: " + distance);
                    return (distance <= AttackRange);
                }
                else
                {
                    return false;
                }
            }
        }

        [HideInInspector]
        public float attackDelay = 0f;

        protected virtual void Start()
        {
            //상태머신 생성 및 상태 등록
            stateMachine = new StateMachine(this, new IdleState());
            stateMachine.RegisterState(new WalkState());
            stateMachine.RegisterState(new AttackState());
            //....

            agent = GetComponent<NavMeshAgent>();
            agent.updatePosition = false;
            agent.updateRotation = true;
            
            animator = GetComponent<Animator>();
            searchTarget = GetComponent<SearchTarget>();
        }

        protected virtual void Update()
        {
            stateMachine.Update(Time.deltaTime);
        }

        private void OnAnimatorMove()
        {
            Vector3 position = transform.position;
            position.y = agent.nextPosition.y;

            animator.rootPosition = position;
            agent.nextPosition = position;
        }

        public State ChangeState(State newState)
        {
            return stateMachine.ChangeState(newState);
        }

        //...

    }
}
