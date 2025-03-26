using System.Collections;
using DUS;
using UnityEngine;

public class EnemyMelee : Enemy
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
        yield return StartCoroutine(base.Attack());
        m_boxCollider.enabled = true;

        yield return new WaitForSeconds(attackDelay);
        m_boxCollider.enabled = false;

        StartCoroutine(OutAttack());
    }
}

