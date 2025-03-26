//====================250312
//
//====================250306
//상속으로 적들 공격에 대한 패턴들 각 타입별 스크립트에서 관리

//====================250304
// 인터페이스로 데미지 받아오는거 변경하려고 했으나 로직 및 시간상 적용X

//====================240928
// 적 데미지 리액션 및 무기 데미지에 따른 피 관리

//********** 적 관리 클래스 **********
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Internal.Commands;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.UI.Image;

namespace DUS
{
    public enum EnemyType
    {
        Melee,
        Charge,
        Range,
        Boss
    }
    public enum Status
    {
        Idle,
        Walk,
        Attack,
        Die,
    }
    public class Enemy : MonoBehaviour
    {
        public EnemyType m_EnemyType;
        public GameManager m_GameManager;

        public int m_MaxHealth;
        public int m_CurHealth;

        public int m_damage;
        public int m_score;
        public GameObject[] coins;

        [SerializeField]
        float m_sphereRadius;
        [SerializeField]
        float m_maxDistance;

        [SerializeField]
        protected float attackDelay;

        protected EnemyAnimationManager m_animatorManager;
        protected Status m_status;
        protected NavMeshAgent m_navMeshAgent;
        protected bool isChase;
        protected bool isAttack;
        protected Rigidbody m_rigid;

        [HideInInspector]
        public PlayerLocomotion m_target;
        Material m_mat;
        bool m_isDamage;
        
        protected virtual void Awake()
        {
            m_rigid = this.GetComponent<Rigidbody>();
            m_mat = this.GetComponentInChildren<MeshRenderer>().material;
            m_navMeshAgent = this.GetComponent<NavMeshAgent>();
            m_GameManager = FindObjectOfType<GameManager>();
            m_target = m_GameManager.m_player;
            m_animatorManager = this.GetComponentInChildren<EnemyAnimationManager>();
            //m_GameManager = GameManager.Instance;
            m_status = Status.Idle;
        }

        private void Start()
        {
            m_CurHealth = m_MaxHealth;
            if(m_EnemyType == EnemyType.Boss) isChase = true;
        }

        private void Update()
        {
            if (m_status == Status.Die || m_target.m_playerState == PlayerState.Die)
            {
                StopAllCoroutines();
                return;
            }
            else if (m_navMeshAgent.enabled)
            {
                m_navMeshAgent.SetDestination(m_target.transform.position);
                m_navMeshAgent.isStopped = isChase;
            }
        }

        protected void ChangeStatus(Status status)
        {
            m_status = status;
        }

        private void Targeting()
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, m_sphereRadius, transform.forward,m_maxDistance, LayerMask.GetMask("Player"));
            if(m_status == Status.Idle && hits.Length <= 0)
            {
                ChangeStatus(Status.Walk);
                m_animatorManager.Move(true);
            }

            if (hits.Length > 0 && !isAttack)
            {
                StartCoroutine(Attack());
                m_animatorManager.Move(false);
            }
        }

        protected virtual IEnumerator Attack()
        {
            isChase = true;
            isAttack = true;

            yield return new WaitForSeconds(0.5f);
            m_animatorManager.Attack();
        }
        protected IEnumerator OutAttack()
        {
            isChase = false;
            isAttack = false;
            m_status = Status.Idle;
            m_animatorManager.Move(false);
            yield return new WaitForSeconds(0.2f);
        }

        protected void FixedUpdate()
        {
            if (m_status == Status.Die || m_target.m_playerState == PlayerState.Die)
            {
                StopAllCoroutines();
                return;
            }
            Targeting();
        }

        public void HitByGrenade(Vector3 explosionPos)
        {
            m_CurHealth -= 20;
            Vector3 _reactVec = transform.position - explosionPos;

            StartCoroutine(OnDamage(CalculateHealth(_reactVec, null, null, null, 1),3));
        }

        void OutDamage()
        {
            m_isDamage = false;
        }

        void Die()
        {
            m_CurHealth = 0;
            m_navMeshAgent.enabled = false;
            m_status = Status.Die;
            m_animatorManager.Die();

            int ranCoin = Random.Range(0,3);
            GameObject coin = Instantiate(coins[ranCoin], this.transform.position, Quaternion.identity);
            
            switch(m_EnemyType)
            {
                case EnemyType.Melee:
                    --m_GameManager.m_enemyRespawn.meleeEnemyCnt;
                    break;
                case EnemyType.Charge:
                    --m_GameManager.m_enemyRespawn.chargeEnemyCnt;
                    break;
                case EnemyType.Range:
                    --m_GameManager.m_enemyRespawn.rangeEnemyCnt;
                    break;
                case EnemyType.Boss:
                    --m_GameManager.m_enemyRespawn.bossEnemyCnt;
                    break;
            }

            Destroy(gameObject,4);
        }

        private Vector3 CalculateHealth(Vector3 _reactVec, WeaponInfo weaponInfo = null, Bullet bullet = null, Collider other = null, float time = 0)
        {
            if (m_status == Status.Die) return Vector3.zero;
            
            m_isDamage = true;
            
            if (weaponInfo != null) m_CurHealth -= weaponInfo.m_damage;
            else if (bullet != null) m_CurHealth -= bullet.m_damage;

            if (m_CurHealth <= 0)
            {
                //Enemy 상태 관련
                Die();

                //UI 관련
                //UIManager.Instance.SetKillMonsterUI(this.enemyType);

                m_GameManager.m_CurrentScore += m_score;

            }
            Vector3 reactVec = Vector3.zero;
            if (_reactVec != Vector3.zero)
                reactVec = _reactVec;
            else if(other != null)
                reactVec = transform.position - other.transform.position; //죽음 리액션(뒤로 밀리는 효과)을 위한

            Invoke("OutDamage", time);
            return reactVec;
        }
        IEnumerator OnDamage(Vector3 reactVec, int weaponType)
        {
            m_mat.color = Color.red;
            reactVec = reactVec.normalized;
            
            switch (weaponType)
            {
                case 0:
                    m_rigid.AddForce((reactVec * 3), ForceMode.Impulse);
                    break;
                case 1:
                    m_rigid.AddForce((reactVec * 2f), ForceMode.Impulse);
                    break;
                default:
                    break;
            }

            yield return new WaitForSeconds(0.1f);

            if (m_CurHealth > 0) m_mat.color = Color.white;
            else
            {
                m_mat.color = Color.gray;
                this.gameObject.layer = 12;

                reactVec += Vector3.up;
                if (weaponType == 2)
                {
                    m_rigid.freezeRotation = false;
                    m_rigid.AddForce(reactVec * 8, ForceMode.Impulse);
                    m_rigid.AddTorque(reactVec * 15, ForceMode.Impulse);
                }
                else m_rigid.AddForce(reactVec * 5, ForceMode.Impulse);

                //Destroy(gameObject, 4);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (m_isDamage) return; //데미지 딜레이를 위해

            if (other.CompareTag("Melee"))
            {
                WeaponInfo _weapon = other.GetComponent<WeaponInfo>();
                StartCoroutine(OnDamage(CalculateHealth(Vector3.zero,_weapon, null, other, 0.2f), 0));
            }
            else if (other.CompareTag("Bullet"))
            {
                Bullet bullet = other.GetComponent<Bullet>();
                Debug.Log(bullet.name + " : " + bullet.m_damage);
                StartCoroutine(OnDamage(CalculateHealth(Vector3.zero, null,bullet, other,0.1f), 1));
            }
        }

        
    }
}
