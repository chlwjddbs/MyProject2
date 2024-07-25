using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NomalHeal : SkillManager
{
    public float recoveryPoint;
    public ParticleSystem effect;

    public override void UseSkill()
    {
        base.UseSkill();

        if (isUse)
        {
            //마나소모
            player.UseMana(cunsumeMana);
            //스킬 모션 등록
            player.SetAnime(skillMotion);
            //스킬 모션 실행
            player.ChangeState(new ActionPState());
            player.SetActionSpeed(actionSpeed);

            //recoveryPoint = Mathf.Round(player.maxHealth * (15 / 100));
            //HP회복
            player.RecoveryHP(recoveryPoint);

            isUse = false;
            remainingTime = coolTime;

            //스킬 이펙트
            ParticleSystem _effect = Instantiate(effect, player.effectPos);
            Vector3 effectOffset = player.effectPos.position;
            effectOffset.y = 2f;
            _effect.transform.position = effectOffset;
            
            Destroy(_effect.gameObject, 2f);
        }
    }
}
