using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.AI
{
    public class IdleState : State
    {
        private Animator animator;
        private CharacterController characterController;

        bool isPatrol = false;
        private float minReadyTime = 1.0f;
        private float maxReadyTime = 3.0f;
        private float readyTime = 0.0f;

        protected int isMoveHash = Animator.StringToHash("IsMove");

        public override void OnInitialize()
        {
            animator = monster.GetComponent<Animator>();
            characterController = monster.GetComponent<CharacterController>();
        }

        public override void OnEnter()
        {
            animator?.SetBool(isMoveHash, false);
            characterController?.Move(Vector3.zero);

            if(monster is MonsterPatrol)
            {
                isPatrol = true;
                readyTime = Random.Range(minReadyTime, maxReadyTime);
            }
        }

        public override void OnUpdate(float deltaTime)
        {
            //타겟을 발견하면 이동한다
            if(monster.Target)
            {
                if(monster.IsAttackable)
                {
                    if(stateMachine.ElapsedTime >= monster.attackDelay)
                    {
                        stateMachine.ChangeState(new AttackState());
                    }
                }
                else
                {
                    stateMachine.ChangeState(new WalkState());
                }
            }
            else if(isPatrol)
            {
                if(stateMachine.ElapsedTime >= readyTime)
                {
                    stateMachine.ChangeState(new PatrolState());
                }
            }
        }

        public override void OnExit()
        {
            
        }
    }
}
