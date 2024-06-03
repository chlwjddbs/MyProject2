using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathPState : PlayerStates
{
    private float gameover;

    public override void Initialize()
    {
        base.Initialize();
        gameover = 3f;
    }
    public override void OnUpdate()
    {
        if(stateMachine.ElapsedTime >= gameover)
        {
            GameData.instance.fader.SceneLoad("GameOverScene");
        }
    }
}
