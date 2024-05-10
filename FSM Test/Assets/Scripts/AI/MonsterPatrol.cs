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

            //�⺻ ���� ���� + ���ο� ���� �߰�
            stateMachine.RegisterState(new PatrolState());
        }
    }
}