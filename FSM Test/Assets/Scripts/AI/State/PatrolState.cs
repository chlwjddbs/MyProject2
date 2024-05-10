using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace MyGame.AI
{
    public class PatrolState : State
    {
        private Animator animator;
        private CharacterController characterController;
        private NavMeshAgent agent;

        private MonsterPatrol patrolMonster;

        private Transform targetWaypoint = null;
        private int wayPointIndex = 0;

        private Transform[] WayPoints => patrolMonster?.wayPoints;

        protected int isMoveHash = Animator.StringToHash("IsMove");


        public override void OnInitialize()
        {
            animator = monster.GetComponent<Animator>();
            characterController = monster.GetComponent<CharacterController>();
            agent = monster.GetComponent<NavMeshAgent>();

            patrolMonster = monster as MonsterPatrol;
        }

        public override void OnEnter()
        {
            agent.stoppingDistance = 0.5f;

            if(targetWaypoint == null)
            {
                //다음 포인트 찾는다
                FindNextWayPoint();
            }

            if(targetWaypoint)
            {
                animator?.SetBool(isMoveHash, true);
                agent?.SetDestination(targetWaypoint.position);
            }
            else
            {
                stateMachine.ChangeState(new IdleState());
            }
        }

        public override void OnUpdate(float deltaTime)
        {
            if(monster.Target)
            {
                if (monster.IsAttackable)
                {
                    stateMachine.ChangeState(new AttackState());
                }
                else
                {
                    stateMachine.ChangeState(new WalkState());
                }
            }
            else
            {
                //도착 판정
                if(!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                {
                    FindNextWayPoint();
                    stateMachine.ChangeState(new IdleState());
                }
                else
                {
                    characterController?.Move(agent.velocity * deltaTime);
                }
            }
        }

        public override void OnExit()
        {
            monster.attackDelay = 0f;
            animator?.SetBool(isMoveHash, false);
            agent.ResetPath();
        }

        void FindNextWayPoint()
        {
            targetWaypoint = null;

            if(WayPoints != null && WayPoints.Length > 0)
            {
                targetWaypoint = WayPoints[wayPointIndex];
                wayPointIndex = (wayPointIndex + 1) % WayPoints.Length;
            }
        }
    }
}
