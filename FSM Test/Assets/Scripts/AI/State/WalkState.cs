using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace MyGame.AI
{
    public class WalkState : State
    {
        private Animator animator;
        private CharacterController characterController;
        private NavMeshAgent agent;

        protected int isMoveHash = Animator.StringToHash("IsMove");

        public override void OnInitialize()
        {
            animator = monster.GetComponent<Animator>();
            characterController = monster.GetComponent<CharacterController>();
            agent = monster.GetComponent<NavMeshAgent>();
        }

        public override void OnEnter()
        {
            animator?.SetBool(isMoveHash, true);

            if (agent != null)
            {
                agent.stoppingDistance = 1.5f;
                agent.SetDestination(monster.Target.position);
            }
        }

        public override void OnUpdate(float deltaTime)
        {
            if(monster.Target)
            {
                agent.SetDestination(monster.Target.position);
                characterController.Move(agent.velocity * deltaTime);

                //µµÂø ÆÇÁ¤
                if(agent.remainingDistance <= agent.stoppingDistance)
                {
                    if(!agent.pathPending)
                    {
                        agent.ResetPath();
                        stateMachine.ChangeState(new IdleState());
                    }
                }
            }
            else
            {   
                stateMachine.ChangeState(new IdleState());
            }
        }

        public override void OnExit()
        {
            monster.attackDelay = 0f;
            animator?.SetBool(isMoveHash, false);
        }
    }
}
