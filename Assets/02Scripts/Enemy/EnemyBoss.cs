using System.Collections;
using DUS;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBoss : Enemy
{
    [SerializeField]
    GameObject missilePrefab;
    [SerializeField]
    GameObject missilePosA;
    [SerializeField]
    GameObject missilePosB;
    [SerializeField]
    GameObject RockPrefab;
    [SerializeField]
    GameObject RockPos;
    [SerializeField]
    BoxCollider m_meleeArea;

    BoxCollider boxCollider;
    Vector3 m_lookVec;
    Vector3 m_TauntVec;
    bool m_isLook;
    protected override void Awake()
    {
        base.Awake();
        boxCollider = GetComponent<BoxCollider>();
        m_navMeshAgent.isStopped = true;
        StartCoroutine(Attack());

    }
    private void Start()
    {
        m_isLook = true;
    }

    private void Update()
    {
        if (m_status == Status.Die || m_GameManager.m_player.m_playerState == PlayerState.Die)
        {
            StopAllCoroutines();
            return;
        }

        if (m_isLook)
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            m_lookVec = new Vector3(h, 0, v) * 5f;
            transform.LookAt(m_target.transform.position + m_lookVec);
        }
        else
        {
            m_navMeshAgent.SetDestination(m_TauntVec);
        }

    }
    protected void FixedUpdate()
    {
        
    }

    private void AttackType()
    {
        int ranAction = Random.Range(0, 5);
        switch (ranAction)
        {
            case 0:
            case 1:
                StartCoroutine(MissileShot());
                break;
            case 2:
            case 3:
                StartCoroutine(RockShot());
                break;
            case 4:
                StartCoroutine(Taunt());
                break;
        }
    }

    IEnumerator MissileShot()
    {
        m_animatorManager.Boss_MissileShot();
        yield return new WaitForSeconds(0.2f);
        GameObject missileA = Instantiate(missilePrefab, missilePosA.transform.position, missilePosA.transform.rotation);
        BossMissile BossMissileA = missileA.GetComponent<BossMissile>();
        BossMissileA.target = m_target.gameObject;

        yield return new WaitForSeconds(0.3f);
        GameObject missileB = Instantiate(missilePrefab, missilePosB.transform.position, missilePosB.transform.rotation);
        BossMissile BossMissileB = missileB.GetComponent<BossMissile>();
        BossMissileB.target = m_target.gameObject;
        yield return new WaitForSeconds(4f);
        StartCoroutine(Attack());
    }
    IEnumerator RockShot()
    {
        m_animatorManager.Boss_RockShot();
        m_isLook = false;
        Instantiate(RockPrefab, RockPos.transform.position, RockPos.transform.rotation);
        yield return new WaitForSeconds(4f);
        m_isLook = true;
        StartCoroutine(Attack());
    }
    IEnumerator Taunt()
    {
        m_TauntVec = m_target.transform.position + m_lookVec;

        m_isLook = false;
        m_navMeshAgent.isStopped = false;
        boxCollider.enabled = false;
        m_animatorManager.Boss_Taunt();

        yield return new WaitForSeconds(1.5f);
        m_meleeArea.enabled = true;

        yield return new WaitForSeconds(0.5f);
        m_meleeArea.enabled = false;

        yield return new WaitForSeconds(3f);
        m_isLook = true;
        boxCollider.enabled = true;
        m_navMeshAgent.isStopped = true;
        StartCoroutine(Attack());
    }

    protected override IEnumerator Attack()
    {
        yield return new WaitForSeconds(3f);
        AttackType();
        //isChase = false;
        //isAttack = false;
    }

}
