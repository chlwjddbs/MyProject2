using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame.AI
{
    public abstract class State
    {
        protected StateMachine stateMachine;
        protected Monster monster;

        public State() {}

        //State ����: monster, stateMachine
        public void SetState(StateMachine _stateMachine, Monster _monster)
        {
            this.stateMachine = _stateMachine;
            this.monster = _monster;

            //���� �ʱ�ȭ
            OnInitialize();
        }

        public virtual void OnInitialize() { }
        public virtual void OnEnter() { }
        public abstract void OnUpdate(float deltaTime);
        public virtual void OnExit() { }
    }

    public class StateMachine
    {
        private Monster monster;

        private State currentState;
        public State CurrentState { get { return currentState; } }

        private State previousState;
        public State PreviousState { get { return previousState; } }

        private float elapsedTime = 0;
        public float ElapsedTime { get { return elapsedTime; } }

        private Dictionary<System.Type, State> states = new Dictionary<System.Type, State>();

        //������
        public StateMachine(Monster _moster, State initState)
        {
            this.monster = _moster;

            //���µ��, �ʱ�ȭ
            RegisterState(initState);
            currentState = initState;

            //���� ���� ó��
            currentState.OnEnter();
            elapsedTime = 0f;
        }

        //���� ���
        public void RegisterState(State state)
        {
            //���� ���� - 
            state.SetState(this, monster);
            //���
            states[state.GetType()] = state;
        }

        public void Update(float deltaTime)
        {
            elapsedTime += deltaTime;
            currentState.OnUpdate(deltaTime);
        }

        //���� ����
        public State ChangeState(State newState)
        {
            //���� ���� üũ
            var newType = newState.GetType();
            if(newType == currentState?.GetType())
            {
                return currentState;
            }

            if (currentState != null)
            {
                currentState.OnExit();
            }

            previousState = currentState;
            currentState = states[newType];
            //���� ���� ó��
            currentState.OnEnter();
            elapsedTime = 0f;

            return currentState;
        }
    }
}
