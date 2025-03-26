using DUS;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

namespace DUS
{
    public class EnemyRespawn : MonoBehaviour
    {
        [SerializeField]
        Transform[] m_respawnZones;

        [SerializeField]
        GameObject[] m_enemies;

        [SerializeField]
        List<int> m_enemyList;

        public int meleeEnemyCnt;
        public int chargeEnemyCnt;
        public int rangeEnemyCnt;
        public int bossEnemyCnt;

        GameManager m_gameManager;
        private void Awake()
        {
            m_enemyList = new List<int>();
            m_gameManager = FindObjectOfType<GameManager>();
        }

        public IEnumerator Respawn(int stageNum, PlayerLocomotion player)
        {
            //Boss 리스폰
            if (stageNum % 5 == 0)
            {
                GameObject instantEnemy = Instantiate(m_enemies[3], m_respawnZones[0].transform.position, m_respawnZones[0].transform.rotation);
                EnemyBoss boss = instantEnemy.GetComponent<EnemyBoss>();
                m_gameManager.m_boss = boss;
                ++bossEnemyCnt;
                m_gameManager.m_player.m_UIManager.SetBossGroupUI(true);
            }
            else
            {
                m_gameManager.m_player.m_UIManager.SetBossGroupUI(false);
                //스테이지 단위로 생성 수 늘어나게
                for (int i = 0; i < stageNum; i++)
                {
                    int ran = Random.Range(0, 3);
                    m_enemyList.Add(ran);
                }

                while (m_enemyList.Count > 0)
                {
                    int ranZonNum = Random.Range(0, 4);
                    int enemyNum = 0;

                    GameObject instantEnemy = Instantiate(m_enemies[m_enemyList[enemyNum]], m_respawnZones[ranZonNum].transform.position, m_respawnZones[ranZonNum].transform.rotation);
                    Enemy enemy = instantEnemy.GetComponent<Enemy>();

                    switch (enemy.m_EnemyType)
                    {
                        case EnemyType.Melee:
                            ++meleeEnemyCnt;
                            break;
                        case EnemyType.Charge:
                            ++chargeEnemyCnt;
                            break;
                        case EnemyType.Range:
                            ++rangeEnemyCnt;
                            break;
                    }

                    if (enemyNum == 3)
                    {
                        EnemyBoss boss = GetComponent<EnemyBoss>();
                    }
                    enemy.m_target = player;
                    m_enemyList.RemoveAt(0);

                    yield return new WaitForSeconds(4f);
                }
            }

            while (meleeEnemyCnt + chargeEnemyCnt + rangeEnemyCnt + bossEnemyCnt > 0)
            {
                yield return null;
            }
            yield return new WaitForSeconds(4f);
            m_gameManager.StageEnd();
        }
    }
}