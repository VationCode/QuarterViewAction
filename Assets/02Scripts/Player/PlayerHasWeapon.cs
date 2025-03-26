using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Progress;

namespace DUS
{
    public class PlayerHasWeapon : MonoBehaviour
    {
        public GameManager m_gameManager;
        PlayerLocomotion m_playerlocomotion;
        PlayerAnimationManager m_playerAnimationManager;

        //1헤머, 2권총, 3라이플
        [Header("무기")]
        public GameObject[] m_AttachWeapons;

        public WeaponInfo[] m_WeaponInfos { get; private set; }
        public bool[] m_hasWeapon { get; private set; }
        public int m_currentIndexNum { get; private set; }

        GameObject nearWeapon;

        private void Awake()
        {
            m_playerlocomotion = m_gameManager.m_player;
            m_playerAnimationManager = m_gameManager.m_player.m_playerAnimationManager;
            m_hasWeapon = new bool[3];
            m_WeaponInfos = new WeaponInfo[m_AttachWeapons.Length];

            m_playerlocomotion = GetComponent<PlayerLocomotion>();
            m_playerAnimationManager = GetComponentInChildren<PlayerAnimationManager>();
            m_currentIndexNum = -1;

            //m_playerlocomotion.m_UIManager.HasWeaponUI(m_playerlocomotion);
        }
        private void Start()
        {
            m_playerlocomotion.m_UIManager.HasWeaponUI(m_playerlocomotion);
        }

        #region =====================================================================GetWeapon 
        public void GetWeaponInfo(int itemNum)
        {
            m_WeaponInfos[itemNum] = m_AttachWeapons[itemNum].GetComponent<WeaponInfo>();
        }

        /// <summary>
        /// 근처 필드에 무기 아이템 있을 시 
        /// F키를 눌러 가지고 있지 않은 아이템만 PickUp
        /// </summary>
        public bool PickUpWeapon()
        {
            if (nearWeapon == null) return false;
            
            Item item = nearWeapon.GetComponent<Item>();
            //1. 가지고 있느냐 없느냐 => 가지고있다 리턴
            if (m_hasWeapon[item.itemInfo.itemNum]) return false;

            //2. 먹었을 때 들고 있는 무기를 교체, 이전 무기 Off
            if (m_currentIndexNum != -1) m_AttachWeapons[m_currentIndexNum].SetActive(false);

            m_currentIndexNum = item.itemInfo.itemNum;

            m_hasWeapon[m_currentIndexNum] = true;
            m_AttachWeapons[m_currentIndexNum].SetActive(true);

            //3. 무기 정보 가져오기
            GetWeaponInfo(item.itemInfo.itemNum);

            //4. 필드 무기 삭제
            Destroy(nearWeapon);
            if(item.itemInfo.itemNum == 0) m_playerlocomotion.m_UIManager.ChangeAmmoTMP(0, 0, 0, 0);
            m_playerlocomotion.m_UIManager.ChangeAmmoTMP(m_WeaponInfos[m_currentIndexNum].m_rangeWeaponInfo.currentAmmo, m_WeaponInfos[m_currentIndexNum].m_rangeWeaponInfo.maxLoadAmmo, m_WeaponInfos[m_currentIndexNum].m_rangeWeaponInfo.magazineAmmo, m_WeaponInfos[m_currentIndexNum].m_rangeWeaponInfo.maxMagazineAmmo);

            m_playerAnimationManager.MoveAni(false, m_playerlocomotion.m_playerInputHandler.m_IsWalkKey);
            m_playerlocomotion.m_UIManager.ShowDiaLogTMP(false, "");
            m_playerlocomotion.m_UIManager.HasWeaponUI(m_playerlocomotion);
            return true;
        }
        #endregion ===================================================================== /GetWeapon

        #region =====================================================================Swap
        /// <summary>
        /// 키값 1,2,3 | 인덱스 0,1,2 
        /// </summary>
        /// <param name="swapKeyNum"></param>
        public bool SwapWeapon(int swapKeyNum)
        {
            --swapKeyNum; 
            //1. 스왑 키 눌렀을 때 무기 없느냐, 첫 무기인가 리턴
            if (!m_hasWeapon[swapKeyNum]) return false;
            if (m_playerlocomotion.m_playerInputHandler.m_CurrentSwapKeyNum == -1) return false;
            if (m_currentIndexNum == swapKeyNum) return false;

            m_AttachWeapons[m_currentIndexNum].SetActive(false);
            m_currentIndexNum = swapKeyNum;
            m_AttachWeapons[m_currentIndexNum].SetActive(true);

            m_playerlocomotion.m_playerAnimationManager.SwapAni();
            m_playerlocomotion.m_UIManager.ChangeAmmoTMP(m_WeaponInfos[m_currentIndexNum].m_rangeWeaponInfo.currentAmmo, m_WeaponInfos[m_currentIndexNum].m_rangeWeaponInfo.maxLoadAmmo, m_WeaponInfos[m_currentIndexNum].m_rangeWeaponInfo.magazineAmmo, m_WeaponInfos[m_currentIndexNum].m_rangeWeaponInfo.maxMagazineAmmo);
            return true;
        }

        #endregion =====================================================================/Swap

        public bool IsSupplyAmmo(int itemNum)
        {
            if (m_currentIndexNum == -1) return false;

            switch (itemNum)
            {
                case 300:
                    if (!m_hasWeapon[1] || !m_WeaponInfos[1].IsCalculatePickUpAmmo(15)) return false;
                    break;
                case 400:
                    if (!m_hasWeapon[2] || !m_WeaponInfos[2].IsCalculatePickUpAmmo(30)) return false;
                    break;
            }
            return true;
        }

        public bool IsAttack()
        {
            if (m_currentIndexNum == -1) return false;
            if(m_currentIndexNum > 0)
            {
                if (m_WeaponInfos[m_currentIndexNum].m_rangeWeaponInfo.currentAmmo == 0) return false;
            }

            switch (m_WeaponInfos[m_currentIndexNum].m_weaponType)
            {
                case WeaponInfo.WeaponType.Melee:
                    m_WeaponInfos[m_currentIndexNum].MeleeAttack();
                    break;
                case WeaponInfo.WeaponType.Range:
                    m_WeaponInfos[m_currentIndexNum].Shoot();
                    break;
            }
            m_playerAnimationManager.AttackAni(m_WeaponInfos[m_currentIndexNum].m_weaponType == WeaponInfo.WeaponType.Melee ? "doSwing" : "doShot");

            return true;
        }
        public bool IsReload()
        {
            if (m_currentIndexNum == -1) return false;
            if (m_WeaponInfos[m_currentIndexNum].m_rangeWeaponInfo.magazineAmmo == 0) return false;
            if (m_WeaponInfos[m_currentIndexNum].ReloadAmmo() == false) return false;
            m_playerlocomotion.m_UIManager.ChangeAmmoTMP(m_WeaponInfos[m_currentIndexNum].m_rangeWeaponInfo.currentAmmo, m_WeaponInfos[m_currentIndexNum].m_rangeWeaponInfo.maxLoadAmmo, m_WeaponInfos[m_currentIndexNum].m_rangeWeaponInfo.magazineAmmo, m_WeaponInfos[m_currentIndexNum].m_rangeWeaponInfo.maxMagazineAmmo);
            return true;
        }
        

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Weapon") && nearWeapon == null) nearWeapon = other.gameObject;
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Weapon") && nearWeapon != null) nearWeapon = null;
        }
    }
}