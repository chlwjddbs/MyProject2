using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.AI
{
    public class AttackState : State
    {
        private Animator animator;

        protected int attackTriggerHash = Animator.StringToHash("AttackTrigger");

        public override void OnInitialize()
        {
            animator = monster.GetComponent<Animator>();
        }

        public override void OnEnter()
        {
            animator.SetTrigger(attackTriggerHash);
        }

        public override void OnUpdate(float deltaTime)
        {
            
        }

        public override void OnExit()
        {
            monster.attackDelay = 5f;
        }
    }
}
