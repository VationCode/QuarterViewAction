using UnityEngine;

public class EnemyAnimationManager : MonoBehaviour
{
    [SerializeField]
    Animator m_animator;

    private void Awake()
    {
        m_animator = GetComponent<Animator>();
    }

    public void Move(bool isWalk)
    {
        m_animator.SetBool("isWalk", isWalk);
    }
    public void Attack()
    {
        m_animator.SetTrigger("doAttack");
    }
    public void Die()
    {
        m_animator.SetTrigger("doDie");
    }

    public void Boss_MissileShot()
    {
        m_animator.SetTrigger("doShot");
    }
    public void Boss_RockShot()
    {
        m_animator.SetTrigger("doBigShot");
    }
    public void Boss_Taunt()
    {
        m_animator.SetTrigger("doTaunt");
    }
}
