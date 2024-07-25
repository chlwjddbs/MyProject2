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
            //�����Ҹ�
            player.UseMana(cunsumeMana);
            //��ų ��� ���
            player.SetAnime(skillMotion);
            //��ų ��� ����
            player.ChangeState(new ActionPState());
            player.SetActionSpeed(actionSpeed);

            //recoveryPoint = Mathf.Round(player.maxHealth * (15 / 100));
            //HPȸ��
            player.RecoveryHP(recoveryPoint);

            isUse = false;
            remainingTime = coolTime;

            //��ų ����Ʈ
            ParticleSystem _effect = Instantiate(effect, player.effectPos);
            Vector3 effectOffset = player.effectPos.position;
            effectOffset.y = 2f;
            _effect.transform.position = effectOffset;
            
            Destroy(_effect.gameObject, 2f);
        }
    }
}
