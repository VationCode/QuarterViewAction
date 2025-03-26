
using System.Collections;
using DUS;
using UnityEngine;
using UnityEngine.AI;

public class EnemyCharge : Enemy
{
    [SerializeField]
    BoxCollider m_boxCollider;

    protected override void Awake()
    {
        base.Awake();
        m_boxCollider = transform.GetChild(1).GetComponent<BoxCollider>();
        m_boxCollider.enabled = false;
    }

    protected override IEnumerator Attack()
    {
        yield return StartCoroutine(base.Attack()); //부모 코루틴 호출하려면 base.Attack()가 아닌 스타트코루틴 이런 방식으로
        
        m_rigid.isKinematic = false;
        m_rigid.useGravity = true;
        this.m_rigid.AddForce(this.transform.forward * 20, ForceMode.Impulse);
        m_boxCollider.enabled = true;


        yield return new WaitForSeconds(attackDelay);
        m_rigid.isKinematic = true;
        m_rigid.useGravity = false;
        m_boxCollider.enabled = false;

        StartCoroutine(OutAttack());
    }
}
