using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

namespace DUS
{
    public class UIManager : MonoBehaviour
    {
        /*#region Singleton
        private static UIManager instance;
        public static UIManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<UIManager>();
                    if(instance != null )
                    {
                        GameObject singletonObj = new GameObject("UIManager");
                        instance = singletonObj.AddComponent<UIManager>();
                    }
                }
                return instance;
            }
        }
        #endregion /Singleton*/
        [SerializeField]
        GameObject m_menuPanel;
        [SerializeField]
        GameObject m_gamePanel;
        [SerializeField]
        GameObject m_gameOverPanel;

        [SerializeField]
        TextMeshProUGUI m_maxScoreTMP;
        [SerializeField]
        TextMeshProUGUI m_currentScoreTMP;
        ///int m_currentScore;

        [Header("StageGroup"), SerializeField]
        TextMeshProUGUI m_stageTMP;
        [SerializeField] 
        TextMeshProUGUI m_timeTMP;

        [Header("StatusGroup"),SerializeField]
        TextMeshProUGUI m_heartTMP;
        [SerializeField]
        TextMeshProUGUI m_ammoTMP;
        [SerializeField]
        TextMeshProUGUI m_coinTMP;

        [Header("WeaponGroup"), SerializeField]
        Image m_hamer;
        [SerializeField] 
        Image m_handGun;
        [SerializeField] 
        Image m_submachineGun;
        [SerializeField] 
        Image m_grnade;

        [Header("EnemyGroup"), SerializeField]
        TextMeshProUGUI m_melee;
        [SerializeField]
        TextMeshProUGUI m_charge;
        [SerializeField]
        TextMeshProUGUI m_range;

        [Header("BossGroup"), SerializeField]
        RectTransform m_bossHealthGroup;
        [SerializeField]
        RectTransform m_bossHealthBar;

        [Header("GameOVerGroup"), SerializeField]
        TextMeshProUGUI m_gameOverScoreTMP;
        [SerializeField]
        TextMeshProUGUI m_gameOverMaxScoreTMP;

        [SerializeField]
        TextMeshProUGUI m_playerDialogTMP;


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

        int meleeCtn = 0;
        int chargeCtn = 0;
        int rangeCtn = 0;

        private void Awake()
        {
            //SingletonInitialized();
            ChangeAmmoTMP(0, 0, 0, 0);
            ChangeCoinTMP(0);
            ShowDiaLogTMP(false,"");
            SetBossGroupUI(false);
            m_gameOverPanel.SetActive(false);
        }

        public void SetCurrentScore(int score)
        {
            m_currentScoreTMP.text = string.Format("{0:n0}", score);
        }
        public void SetMaxScore(int maxScore)
        {
            PlayerPrefs.SetInt("MaxScoreTMP", maxScore);
            m_maxScoreTMP.text = string.Format("{0:n0}",PlayerPrefs.GetInt("MaxScoreTMP"));
        }

        public void SetUIStart(bool isBool)
        {
            m_gamePanel.SetActive(isBool);
            m_menuPanel.SetActive(!isBool);
        }

        public void ChangeHeart(int current, int max)
        {
            m_heartTMP.text = current + " / " + max;
        }
        public void SetPlayTime(float hour, float min, float second)
        {
            m_timeTMP.text = string.Format("{0:00}", hour) + ":" + string.Format("{0:00}", min) + ":" + string.Format("{0:00}", second);
        }

        public void SetStageUI(int index)
        {
            m_stageTMP.text = "STAGE " + index;
        }

        public void HasWeaponUI(PlayerLocomotion playerLocomotion)
        {
            if (playerLocomotion.gameObject.activeInHierarchy == false) return;
            m_hamer.color = new Color(1,1,1, playerLocomotion.m_playerHasWeapon.m_hasWeapon[0] ? 1:0);
            m_handGun.color= new Color(1, 1, 1, playerLocomotion.m_playerHasWeapon.m_hasWeapon[1] ? 1 : 0);
            m_submachineGun.color = new Color(1, 1, 1, playerLocomotion.m_playerHasWeapon.m_hasWeapon[2] ? 1 : 0);
            m_grnade.color = new Color(1, 1, 1, playerLocomotion.m_effectGrenadeManager.m_HasGrenades > 0 ? 1 : 0);
        }

        public void ChangeAmmoTMP(int currentAmmo, int maxAmmo, int magazineAmmo, int maxMagazineAmmo)
        {
            m_ammoTMP.text = currentAmmo + " / " + maxAmmo + " / " +magazineAmmo + " / " + maxMagazineAmmo;
        }
        public void ChangeCoinTMP(int coin)
        {
            m_coinTMP.text = coin.ToString();
        }

        public void ShowDiaLogTMP(bool isActivate, string tmp)
        {
            m_playerDialogTMP.enabled = isActivate;
            m_playerDialogTMP.text = tmp;
        }

        public void SetBossGroupUI(bool isActivate)
        {
            m_bossHealthGroup.gameObject.SetActive(isActivate);
        }

        public void SetBossHealthImage(EnemyBoss enemyBoss)
        {
            m_bossHealthBar.localScale = new Vector3((float)enemyBoss.m_CurHealth/enemyBoss.m_MaxHealth,1,1);
        }

        public void SetCountMonsterUI(int num, int num2, int num3)
        {
            meleeCtn = num;
            m_melee.text = " x " + (meleeCtn).ToString();
            chargeCtn = num2;
            m_charge.text = " x " + (chargeCtn).ToString();
            rangeCtn = num3;
            m_range.text = " x " + (rangeCtn).ToString();
        }

        public void SetGameOverUI()
        {
            m_gameOverPanel.gameObject.SetActive(true);
            m_gamePanel.SetActive(false);
            m_gameOverScoreTMP.text = m_currentScoreTMP.text;
            m_gameOverMaxScoreTMP.text = m_maxScoreTMP.text;
        }
    }
}
