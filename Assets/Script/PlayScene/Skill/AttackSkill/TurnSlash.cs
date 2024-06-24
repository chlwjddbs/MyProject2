using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnSlash : SkillManager
{
    public ParticleSystem effect;
    public float skillDamage; 

    public override void UseSkill()
    {
        base.UseSkill();

        if (isUse)
        {
            player.UseMana(cunsumeMana);
            player.SetAnime(skillMotion);
            player.SetDamage(skillDamage,true);
            player.ChangeState(new ActionPState());
            player.SetActionSpeed(actionSpeed);
            //player.isAction = true;     
            
            isUse = false;
            remainingTime = coolTime;

            //Ω∫≈≥ ¿Ã∆Â∆Æ
            ParticleSystem _effect = Instantiate(effect, player.effectPos);
            Vector3 effectOffset = player.effectPos.position;
            effectOffset.y = 2.5f;
            _effect.transform.position = effectOffset;
        }
    }
}
