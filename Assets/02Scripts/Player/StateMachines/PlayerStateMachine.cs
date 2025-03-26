//===========================250301
//플레이어의 상태머신 추상클래스

//===========================
using DUS;
using UnityEngine;

public abstract class PlayerStateMachine
{
    public abstract void EnterState(PlayerLocomotion playerLocomotion);
    public abstract void UpdateState(PlayerLocomotion playerLocomotion);
    public abstract void ExitState();
}