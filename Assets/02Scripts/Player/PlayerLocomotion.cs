//====================250316
//상점 입장 기능 추가

//====================250301
//상태 관리에 대한 정리

//====================250218
// 리팩토링 작업
// 차후 멀티 기능 넣기

//====================240928 
// 장전 계산 수정
// 수류탄 투척

//====================240824
// 무기와 아이템 분류
// 수류탄 로직

//====================240816 ~ 17
// 아이템 제작
// 필드 아이템 충돌 처리 및 획득
// 무기 아이템 스왑 (Equip 장착)

//====================240811
// 첫 시작
// Movement관련 기능 개발

//==================================================
// 연속 작업한날은 ~로 묶기


//********** 플레이어 동작 관리 클래스 **********
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace DUS
{
    public enum PlayerState
    {
        Move,
        Jump,
        Dodge,
        PickUpWeapon,
        SwapWeapon,
        Attack,
        Reload,
        Grenade,
        Die,
        Shopping
    }
    public class PlayerLocomotion : MonoBehaviour
    {
        /*#region Singleton
        private static PlayerLocomotion instance;
        public static PlayerLocomotion Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<PlayerLocomotion>();
                    if (instance == null)
                    {
                        //GameObject singletonObj = new GameObject(typeof(PlayerLocomotion).Name);
                        GameObject singletonObj = new GameObject("Player");
                        instance = singletonObj.AddComponent<PlayerLocomotion>();
                    }
                }
                return instance;
            }
        }
        #endregion*/

        //===== RefClass
        public PlayerAnimationManager m_playerAnimationManager { get; private set; }
        public PlayerHasWeapon m_playerHasWeapon { get; private set; }
        public UIManager m_UIManager;
        //public PlayerAudioManager m_PlayerAudioManager;
        public GameManager m_GameManager;

        public PlayerInputHandler m_playerInputHandler;
        

        //===== State
        [HideInInspector]
        public PlayerState m_playerState { get; private set; }

        //===== Setting
        //[FormerlySerializedAs("runSpeed")] //변수명 변경 시 해당 값이 변경되지 않도록하는 방법
        [SerializeField]
        Camera m_followCamera;

        [Header("[ Player Movement Value]")]
        [SerializeField]
        float m_runSpeed;
        [SerializeField]
        float m_walkSpeed;
        [Tooltip("위로 회피"), SerializeField]
        float m_jumpUpForce;
        [SerializeField]
        float m_jumpForwardForce;

        [Tooltip("빠른 회피"), SerializeField]
        float m_dodgeForwardForce;
        float m_moveSpeed;

        //===== Item 관련
        [Header("[Grenade]")]
        public EffectGrenadeManager m_effectGrenadeManager;
        public GameObject m_GrenadePrefab;

        [Header("[Coin]")]
        public int m_Coin;

        [Header(" [Status] ")]
        public int m_CurrentHealth;
        public int m_MaxHealth;
        public MeshRenderer[] m_MeshRenders;

        //===== Movement 관련
        Vector3 m_moveDir;
        Rigidbody m_playerRigid;

        bool m_isGrounded; //점프는 트리거함수 때문에 m_isStatusProgressing 대신에 따로 또 체크
        bool m_isStatusProgressing;
        bool m_isDamage;
        Coroutine m_dialogCoroutine;
        /*private void SingletonInitialized()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);
        }*/

        private void Awake()
        {
            //SingletonInitialized();

            //m_playerInputHandler = PlayerInputHandler.Instance;

            m_playerRigid = GetComponent<Rigidbody>();
            m_playerAnimationManager = GetComponentInChildren<PlayerAnimationManager>();
            m_playerHasWeapon = GetComponent<PlayerHasWeapon>();
            
            ChangeState(PlayerState.Move);

            m_MeshRenders = GetComponentsInChildren<MeshRenderer>();

            PlayerPrefs.SetInt("MaxScore", 1025);
        }

        private void Start()
        {
            m_CurrentHealth = m_MaxHealth;
            m_UIManager.ChangeHeart(m_CurrentHealth, m_MaxHealth);
            m_Coin = 4000;
            m_UIManager.ChangeCoinTMP(m_Coin);
        }

        void StopToWall()
        {
            //Debug.DrawRay(transform.position, transform.forward * 5);
            //m_isBoard = Physics.Raycast(transform.position, transform.forward, 5, LayerMask.GetMask("Wall"));
        }

        private void FixedUpdate()
        {
            //StopToWall();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                ChangeState(PlayerState.Die);
            }
            UpdateState();
        }
        #region ============================================================================================================================== StatusMagement
        private void UpdateState()
        {
            if (m_isStatusProgressing) return;
            //FSM(유한상태머신) 방식으로 상태 관리(상태머신 연계는 다음에 넣기)
            switch (m_playerState)
            {
                case PlayerState.Move:
                    Move();
                    Rotate();
                    break;
                case PlayerState.Jump:
                    Jump();
                    break;
                case PlayerState.Dodge:
                    Dodge();
                    break;
                case PlayerState.PickUpWeapon:
                    PickUpWeapon();
                    break;
                case PlayerState.SwapWeapon:
                    break;
                case PlayerState.Attack:
                    Attack();
                    break;
                case PlayerState.Reload:
                    Reload();
                    break;
                case PlayerState.Grenade:
                    Grenade();
                    break;
                case PlayerState.Die:
                    Die();
                    break;
                case PlayerState.Shopping:

                    break;
            }
        }
        private void ChangeState(PlayerState playerState)
        {
            m_playerState = playerState;
        }

        private void FSMManagement()
        {
            if(m_isStatusProgressing) return;

            //m_isStatusProgressing가 false 상태일 때, 즉 다른 동작이 없을 때 교체
            else if(m_playerInputHandler.m_IsJumpKey) ChangeState(PlayerState.Jump);
            else if(m_playerInputHandler.m_IsDodgeKey) ChangeState(PlayerState.Dodge);
            else if(m_playerInputHandler.m_IsPickUpWeaponKey) ChangeState(PlayerState.PickUpWeapon);
            else if(m_playerInputHandler.m_IsAttackKey) ChangeState(PlayerState.Attack);
            else if(m_playerInputHandler.m_IsSwapKey) ChangeState(PlayerState.SwapWeapon);
            else if(m_playerInputHandler.m_isReloadKey) ChangeState(PlayerState.Reload);
            else if (m_playerInputHandler.m_isGrenadeKey) ChangeState(PlayerState.Grenade);
        }
        #endregion ==============================================================================================================================/StatusMagement

        #region ============================================================================================================================== Movement
        /// <summary>
        /// 항상 모든 동작은 무브상태일 때 상태전이가 이루어지도록 관리
        /// </summary>
        private void Move()
        {
            m_moveDir = new Vector3(m_playerInputHandler.m_InputMoveVec.x, 0, m_playerInputHandler.m_InputMoveVec.y);
            if (m_moveDir.sqrMagnitude > 1) m_moveDir.Normalize(); //확인 결과 노멀라이즈되서 오는듯 (대각선을 위해서)

            m_moveSpeed = m_playerInputHandler.m_IsWalkKey ? m_walkSpeed : m_runSpeed;
            transform.Translate(m_moveDir * m_moveSpeed * Time.deltaTime, Space.World);

            m_playerAnimationManager.MoveAni(m_moveDir == Vector3.zero ? false : true, m_playerInputHandler.m_IsWalkKey);

            FSMManagement();

            //====================주요 분석====================
            //position의 이동과 Translate의 이동의 차이에 의한 선택(위치적 이동, 상대적 이동)
            //rigid 이동으로하지 않은 이유는 FixedUpdate 즉 물리 연산은 CPU 무리가기에 최적화로 간단 설계
            //다른 방식도 있으나 일단은 간략하게만
            //transform.Translate(m_moveDir * m_moveSpeed * Time.deltaTime, Space.World);
            //transform.position += moveDir * moveSpeed * Time.deltaTime; //강의 방식
            //rigid.MovePosition(transform.position + (moveDir * moveSpeed * Time.deltaTime));*/
            //=================================================
        }

        private void Rotate()
        {
            // 마우스 회전에 따른 캐릭터 회적이 아닌 키로만 회전 가능하게
            // 이유는 기획충돌로 인해
            transform.LookAt(transform.position + m_moveDir);

            #region=============================================250219 마우스 회전에 의한 캐릭터 회전 기획상 맞지 않는 듯 싶어 주석처리
            //if (m_isAttackking) return;
            // 1. 키보드에 의한 회전

            /*if (m_equipWeaponInfo.weaponType == WeaponInfo.WeaponType.Melee && m_isAttackking) return;
            transform.LookAt(transform.position + m_moveDir);
            
            // 2. 마우스에 의한 회전
            Ray ray = m_followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;

            if (m_equipWeaponInfo.weaponType == WeaponInfo.WeaponType.Melee) return;
            if (!m_isAttackKey) return;

            //out 저장함수라고 생각하면됨 Hit가 되었을 때 rayHit에 Hit정보를 담겠다
            if (Physics.Raycast(ray, out rayHit, 100))
            {
                Vector3 nextVec = rayHit.point - transform.position;
                nextVec.y = 0;
                transform.LookAt(transform.position + nextVec);
            }*/
            #endregion=============================================/250219 마우스 회전에 의한 캐릭터 회전 주석처리
        }

        private void Jump()
        {
            if (m_isGrounded) return;
            //m_PlayerAudioManager.PlayAudio(1);
            m_isGrounded = true;
            m_isStatusProgressing = true;

            m_playerRigid.AddForce(Vector3.up * m_jumpUpForce, ForceMode.Impulse);
            m_playerRigid.AddForce(transform.forward * m_jumpForwardForce, ForceMode.Impulse);

            m_playerAnimationManager.JumpAni(m_isStatusProgressing);
        }

        private void Dodge()
        {
            m_playerRigid.linearVelocity = transform.forward * m_dodgeForwardForce;
            m_playerAnimationManager.DodgeAni(m_isStatusProgressing);
            InvokeStatus("OutDodge", 1.0f);
        }

        private void OutDodge()
        {
            m_playerRigid.linearVelocity = Vector3.zero;
            OutStatus();
        }
        //적과의 인터렉션
        private void Die()
        {
            m_GameManager.GameOver();
            m_playerAnimationManager.Die();
            m_isStatusProgressing = true;
            StopAllCoroutines();
        }
        IEnumerator OnDamage(int damage, Bullet bullet)
        {
            m_isDamage = true;
            foreach (MeshRenderer mesh in m_MeshRenders)
            {
                mesh.material.color = Color.yellow;
            }
            m_CurrentHealth -= damage;

            if (m_CurrentHealth <= 0)
            {
                m_CurrentHealth = 0;
                ChangeState(PlayerState.Die);
            }
            if (bullet != null && bullet.attacker == AttacType.BossTaunt) m_playerRigid.AddForce(transform.forward * -25, ForceMode.Impulse);
            yield return new WaitForSeconds(0.3f);
            if (bullet != null && bullet.attacker == AttacType.BossTaunt) m_playerRigid.linearVelocity = Vector3.zero;
            foreach (MeshRenderer mesh in m_MeshRenders)
            {
                mesh.material.color = Color.white;
            }
            m_UIManager.ChangeHeart(m_CurrentHealth, m_MaxHealth);
            m_isDamage = false;
        }
        #endregion =========================================================================================================================== /Movement
        #region =========================== PublicStatusFunction
        private bool CheckStatusAndReturnToMoveStatus(bool isCheck)
        {
            if (isCheck == false)
            {
                ChangeState(PlayerState.Move);
                return true;
            }
            return false;
        }
        private void InvokeStatus(string invokeName, float time)
        {
            m_isStatusProgressing = true;
            Invoke(invokeName, time);
        }
        public void OutStatus()
        {
            m_isStatusProgressing = false;
            ChangeState(PlayerState.Move);
        }
        #endregion =========================== /PublicStatusFunction

        #region ============================================================================================================================== Item Interaction
        #region ============================== FieldItem
        // PlayerHasWepon에서 처리
        private void PickUpWeapon()
        {
            if(CheckStatusAndReturnToMoveStatus(m_playerHasWeapon.PickUpWeapon())) return;

            InvokeStatus("OutStatus", 0.3f);
        }

        /// <summary>
        /// 닿으면 자동으로 먹어지는 아이템들
        /// </summary>
        /// <param name="other"></param>
        private void PickUpFieldItem(GameObject other)
        {
            Item item = other.GetComponent<Item>();
            switch (item.itemInfo.itemType)
            {
                //탄창 먹었을 경우
                case ItemType.Ammo:
                    if (PickUpAmmo(item.itemInfo.itemNum) == false) return;
                    break;
                case ItemType.Coin:
                    m_Coin += item.itemInfo.itemNum;
                    m_UIManager.ChangeCoinTMP(m_Coin);
                    break;
                case ItemType.Grenade:
                    if (!m_effectGrenadeManager.OnEffectGrenade())
                    {
                        m_UIManager.HasWeaponUI(this);
                        return;
                    }
                    break;
                case ItemType.Heart:
                    m_CurrentHealth += item.itemInfo.itemNum;
                    if (m_CurrentHealth > m_MaxHealth)
                    {
                        m_CurrentHealth = m_MaxHealth;
                    }
                    m_UIManager.ChangeHeart(m_CurrentHealth, m_MaxHealth);
                    break;
            }
            Destroy(other.gameObject);
        }
        private bool PickUpAmmo(int itemNum)
        {
            if (!m_playerHasWeapon.IsSupplyAmmo(itemNum)) return false;
            return true;
        }
        #endregion =========================== /FieldItem

        #region ============================== Weapon

        /// <summary>
        /// PlayerHasWepon에서 처리
        /// </summary>
        /// <param name="swapKeyNum"></param>
        public void SwapWeapon(int swapKeyNum)
        {
            if(CheckStatusAndReturnToMoveStatus(m_playerHasWeapon.SwapWeapon(swapKeyNum))) return;

            InvokeStatus("OutStatus", 0.5f);
        }

        public void Attack()
        {
            //1. 공격할 상태인가를 체크 및 True이면 IsAttack()에서 동작 이루어짐
            if(CheckStatusAndReturnToMoveStatus(m_playerHasWeapon.IsAttack())) return;
            

            //2. 애니메이션 대기시간 및 원상복구
            InvokeStatus("OutStatus", m_playerHasWeapon.m_WeaponInfos[m_playerHasWeapon.m_currentIndexNum].m_rate);
        }
        private void Reload()
        {
            if (CheckStatusAndReturnToMoveStatus(m_playerHasWeapon.IsReload())) return;

            m_playerAnimationManager.ReloadAni();
            InvokeStatus("OutStatus", 3);
        }

        //무기 공격과 별개로 동작
        private void Grenade()
        {
            if (CheckStatusAndReturnToMoveStatus(m_effectGrenadeManager.OffEffectGrenade())) return;
            Ray _ray = m_followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit _rayHit;

            if (Physics.Raycast(_ray, out _rayHit, 100))
            {
                Vector3 _nextVec = _rayHit.point - transform.position;
                _nextVec.y = 0;
                GameObject _instantGrenade = Instantiate(m_GrenadePrefab, transform.position, transform.rotation);
                Rigidbody _rigidGrenade = _instantGrenade.GetComponent<Rigidbody>();
                _rigidGrenade.AddForce(_nextVec, ForceMode.Impulse);
                _rigidGrenade.AddTorque(Vector3.back * 10, ForceMode.Impulse);
            }
            m_playerAnimationManager.ThrowAni();
            InvokeStatus("OutStatus", 0.8f);
        }
        #endregion =========================== /Weapon
        #endregion ==============================================================================================================================/ Item Interaction

        #region ============================================================================================================================== Shop Interaction
        private void EnterShop(GameObject other)
        {
            m_isStatusProgressing = true;
            ChangeState(PlayerState.Shopping);
            Shop shop = other.GetComponentInParent<Shop>();
            shop.Enter(this);
        }
        public void ExitShop(GameObject other)
        {
            Shop shop = other.GetComponentInParent<Shop>();
            shop.Exit();
        }
        #endregion =========================================================================================================================== /Shop Interaction
        #region ============================================================================================================================== Detection
        //Collision 충돌, Trigger 계기
        //Item 콜라이더 두개 가지고 있는 상태(작은 콜라이더 : Floor에 고정, 큰 콜라이더 : 플레이어 트리거 감지)
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.CompareTag("Floor"))
            {
                if (m_isGrounded)
                {
                    m_isStatusProgressing = false;
                    m_isGrounded = false;
                    ChangeState(PlayerState.Move);
                    m_playerAnimationManager.JumpAni(m_isStatusProgressing);
                }
            }
        }
        IEnumerator PickupDialog(string talk)
        {
            
            m_UIManager.ShowDiaLogTMP(true, talk);
            yield return new WaitForSeconds(2f);
            m_UIManager.ShowDiaLogTMP(false, "");
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Item"))
            {
                PickUpFieldItem(other.gameObject);
            }
            else if (other.CompareTag("EnemyBullet"))
            {
                if (m_isDamage || m_playerState == PlayerState.Die) return;
                Bullet bullet = other.GetComponent<Bullet>();

                /*if (m_dialogCoroutine != null)
                {
                    StopCoroutine(OnDamage(bullet.m_damage, bullet));
                    m_dialogCoroutine = null;
                }
                m_dialogCoroutine = */
                    StartCoroutine(OnDamage(bullet.m_damage, bullet));
            }
            else if(other.CompareTag("EnemyMeleeAttack"))
            {
                if (m_isDamage || m_playerState == PlayerState.Die) return;
                Enemy enemy = other.GetComponentInParent<Enemy>();
                /*if (m_dialogCoroutine != null)
                {
                    StopCoroutine(OnDamage(enemy.m_damage, null));
                    m_dialogCoroutine = null;
                }
                m_dialogCoroutine = */
                    StartCoroutine(OnDamage(enemy.m_damage, null));
            }


            if (other.CompareTag("Weapon"))
            {
                if (m_dialogCoroutine != null)
                {
                    StopCoroutine(m_dialogCoroutine);
                    m_dialogCoroutine = null;
                }
                m_dialogCoroutine = StartCoroutine(PickupDialog("F 키를 누르세요"));
            }

            if (other.CompareTag("Shop"))
            {
                if (m_dialogCoroutine != null)
                {
                    StopCoroutine(m_dialogCoroutine);
                    m_dialogCoroutine = null;
                }
                m_dialogCoroutine = StartCoroutine(PickupDialog("Z 키를 누르세요"));

            }
        }
        private void OnTriggerStay(Collider other)
        {
            if(other.CompareTag("Shop"))
            {
                if (m_playerInputHandler.m_isShoppingKey)
                {
                    EnterShop(other.gameObject);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Shop"))
            {
                ExitShop(other.gameObject);
                if (m_dialogCoroutine != null)
                {
                    StopCoroutine(m_dialogCoroutine);
                    m_dialogCoroutine = null;
                }
                m_UIManager.ShowDiaLogTMP(false,"");
            }
            if (other.CompareTag("Weapon"))
            {
                if (m_dialogCoroutine != null)
                {
                    StopCoroutine(m_dialogCoroutine);
                    m_dialogCoroutine = null;
                }
                m_UIManager.ShowDiaLogTMP(false, "");
            }

        }

        #endregion =========================================================================================================================== /Detection
    }
}