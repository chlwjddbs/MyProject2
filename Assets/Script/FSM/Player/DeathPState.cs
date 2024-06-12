using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathPState : PlayerStates
{
    private float gameover;
    private bool changeScene;

    public override void Initialize()
    {
        base.Initialize();
        pState = PlayerState.Death;
        gameover = 2f;
    }
    public override void OnUpdate()
    {
        if(stateMachine.ElapsedTime >= gameover && !changeScene)
        {
            changeScene = true;
            GameData.instance.fader.SceneLoad("GameOverScene"); 
        }
    }
}
