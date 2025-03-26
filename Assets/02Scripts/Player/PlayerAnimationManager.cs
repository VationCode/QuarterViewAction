//====================240811
//첫 시작
//Movement관련 애니메이션들 추가

//==================================================

//********** 플레이어 애니메이션 관리 클래스 **********
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DUS
{
    public enum ParametersType
    {
        Int,
        Float,
        Bool,
        Trigger
    }

    public class PlayerAnimationManager : MonoBehaviour
    {
        [SerializeField]
        private Animator anim;

        [SerializeField]
        WeaponInfo m_melee;

        private void Awake()
        {
            anim = GetComponent<Animator>();
        }


        /*public void MoveHandle(string parametersName, bool isBool)
        {
            PlayMove(parametersName, isBool);
        }*/
        private void PlayAnim(ParametersType parametersType, string parametersName = "", bool isBool = false, float value = 0)
        {
            switch (parametersType)
            {
                case ParametersType.Int:
                    anim.SetInteger(parametersName, (int)value);
                    break;
                case ParametersType.Float:
                    anim.SetFloat(parametersName, value);
                    break;
                case ParametersType.Bool:
                    anim.SetBool(parametersName, isBool);
                    break;
                case ParametersType.Trigger:
                    anim.SetTrigger(parametersName);
                    break;
            }
        }
        public void MoveAni(bool isMove, bool isWalkKey)
        {
            PlayAnim(ParametersType.Bool, "isWalk", isWalkKey);
            PlayAnim(ParametersType.Bool, "isMove", isMove);
        }
        public void MoveAniB(float moveSpeed)
        {
            PlayAnim(ParametersType.Float, "moveSpeed",false, moveSpeed);
            //PlayAnim(ParametersType.Bool, "isWalk", isWalkKey);
        }
        public void JumpAni(bool isJumping)
        {
            PlayAnim(ParametersType.Bool, "isJumping", isJumping);
            if (isJumping) PlayAnim(ParametersType.Trigger, "doJump");
        }

        public void DodgeAni(bool isDodging)
        {
            PlayAnim(ParametersType.Trigger, "doDodge");
        }

        public void SwapAni()
        {
            PlayAnim(ParametersType.Trigger, "doSwap");
        }

        public void AttackAni(string attackType)
        {
            PlayAnim(ParametersType.Trigger, attackType);
        }

        public void MeleeAttackEffectOn()
        {
            m_melee.m_meleeWeaponInfo.meleeArea.enabled = true;
            m_melee.m_meleeWeaponInfo.trailRendererEffect.enabled = true;
        }
        public void MeleeAttackEffectOff()
        {
            m_melee.m_meleeWeaponInfo.meleeArea.enabled = false;
            m_melee.m_meleeWeaponInfo.trailRendererEffect.enabled = false;
        }

        public void ReloadAni()
        {
            PlayAnim(ParametersType.Trigger, "doReload");
        }

        public void ThrowAni()
        {
            PlayAnim(ParametersType.Trigger, "doThrow");
        }

        public void Die()
        {
            PlayAnim(ParametersType.Trigger, "doDie");
        }
    }
}