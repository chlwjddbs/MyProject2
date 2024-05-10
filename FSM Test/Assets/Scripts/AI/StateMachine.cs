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

        //State 셋팅: monster, stateMachine
        public void SetState(StateMachine _stateMachine, Monster _monster)
        {
            this.stateMachine = _stateMachine;
            this.monster = _monster;

            //상태 초기화
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

        //생성자
        public StateMachine(Monster _moster, State initState)
        {
            this.monster = _moster;

            //상태등록, 초기화
            RegisterState(initState);
            currentState = initState;

            //상태 들어가기 처리
            currentState.OnEnter();
            elapsedTime = 0f;
        }

        //상태 등록
        public void RegisterState(State state)
        {
            //상태 셋팅 - 
            state.SetState(this, monster);
            //등록
            states[state.GetType()] = state;
        }

        public void Update(float deltaTime)
        {
            elapsedTime += deltaTime;
            currentState.OnUpdate(deltaTime);
        }

        //상태 변경
        public State ChangeState(State newState)
        {
            //현재 상태 체크
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
            //상태 들어가기 처리
            currentState.OnEnter();
            elapsedTime = 0f;

            return currentState;
        }
    }
}
