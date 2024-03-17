using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NomalHeal : SkillManager
{
    public float recoveryPoint;
    public ParticleSystem effect;

    public override void UseSkill(PlayerController _player)
    {
        if (isUse)
        {
            //�����Ҹ�
            _player.GetComponent<PlayerStatus>().UseMana(cunsumeMana);
            //��ų ��� ���
            _player.SetAnime(skillMotion);
            //��ų ��� ����
            _player.SetState(PlayerState.Action);
            _player.SetActionSpeed(actionSpeed);
            PlayerController.isAction = true;
                      
            //recoveryPoint = Mathf.Round(player.maxHealth * (15 / 100));
            //HPȸ��
            _player.playerStatus.RecoveryHP(recoveryPoint);

            isUse = false;
            remainingTime = coolTime;

            //��ų ����Ʈ
            ParticleSystem _effect = Instantiate(effect,_player.effectPos);
            Vector3 effectOffset = _player.effectPos.position;
            effectOffset.y = 2f;
            _effect.transform.position = effectOffset;
            
            Destroy(_effect.gameObject, 2f);
        }
    }
}
