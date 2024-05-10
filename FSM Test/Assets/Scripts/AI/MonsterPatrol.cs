using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.AI
{
    public class MonsterPatrol : Monster
    {
        public Transform[] wayPoints;

        protected override void Start()
        {
            base.Start();

            //기본 몬스터 상태 + 새로운 상태 추가
            stateMachine.RegisterState(new PatrolState());
        }
    }
}