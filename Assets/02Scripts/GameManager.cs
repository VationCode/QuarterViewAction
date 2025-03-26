//====================250318
//전체적인 관리 시작

//====================240811

//********** 게임 진행 관리 매니저 **********
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DUS
{
    public class GameManager : MonoBehaviour
    {
        /*#region SingleTon
        private static GameManager instance;
        public static GameManager Instance
        {
            get
            {
                if (instance == null) instance = FindObjectOfType<GameManager>();
                return instance;
            }
        }
        #endregion*/

        public EnemyRespawn m_enemyRespawn;
        public PlayerLocomotion m_player;
        public EnemyBoss m_boss;

        [SerializeField]
        Camera m_mainCamera;
        [SerializeField]
        Camera m_menuCamera;
        [SerializeField]
        UIManager m_UIManager;
        [SerializeField]
        GameObject m_mainGameObj;

        [Header("Stage"),SerializeField]
        GameObject m_shopStageGroup;
        [SerializeField]
        GameObject m_stage1Group;

        public int m_stageNum;
        public bool m_isBattle;
        public int m_CurrentScore;
        public int m_MaxScore;

        float m_playTime;
        Coroutine m_InBattleCorutine;
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
            m_enemyRespawn = GetComponent<EnemyRespawn>();
            Application.targetFrameRate = 60;

            m_MaxScore = 2025;
            m_UIManager.SetMaxScore(m_MaxScore);
        }
        private void Start()
        {
            m_UIManager.SetUIStart(false);
            m_player.gameObject.SetActive(false);
            m_mainGameObj.SetActive(false);
            SetChangeCamera(false);
            InitialzeStage();
        }

        private void InitialzeStage()
        {
            m_shopStageGroup.SetActive(true);
            m_stage1Group.SetActive(false);
        }

        private void SetChangeCamera(bool isActivate)
        {
            m_menuCamera.gameObject.SetActive(!isActivate);
            m_mainCamera.gameObject.SetActive(isActivate);
        }

        /// <summary>
        /// UI버튼에서 실행
        /// </summary>
        public void GameStart()
        {
            m_UIManager.SetUIStart(true);
            m_mainGameObj.SetActive(true);
            m_player.gameObject.SetActive(true);
            SetChangeCamera(true);
        }
        public void GameOver()
        {
            m_UIManager.SetGameOverUI();

        }
        public void Restart()
        {
            SceneManager.LoadScene(0);
        }

        private void Update()
        {
            if(m_isBattle) m_playTime += Time.time;
        }

        private void LateUpdate()
        {
            int hour = (int)(m_playTime / 3600);
            int min = (int)((m_playTime - hour * 3600) / 60);
            int second = (int)(m_playTime % 60);
            m_UIManager.SetPlayTime(hour, min, second);
            m_UIManager.SetStageUI(m_stageNum);
            m_UIManager.SetCountMonsterUI(m_enemyRespawn.meleeEnemyCnt, m_enemyRespawn.chargeEnemyCnt, m_enemyRespawn.rangeEnemyCnt);

            m_UIManager.SetCurrentScore(m_CurrentScore);
            if (m_CurrentScore >= m_MaxScore)
            {
                m_MaxScore = m_CurrentScore;
                m_UIManager.SetMaxScore(m_MaxScore);
            }

            if(m_boss != null)
            m_UIManager.SetBossHealthImage(m_boss);
        }

        public void StageStart()
        {
            if(m_InBattleCorutine != null)
            {
                StopCoroutine(m_InBattleCorutine);
                m_InBattleCorutine = null;
            }
            m_shopStageGroup.SetActive(false);
            m_stage1Group.SetActive(true);
            m_isBattle = true;
            m_InBattleCorutine = StartCoroutine(m_enemyRespawn.Respawn(m_stageNum, m_player));
        }

        public void StageEnd()
        {
            m_shopStageGroup.SetActive(true);
            m_stage1Group.SetActive(false);
            m_isBattle = false;
            m_stageNum++;
        }

        
    }
}