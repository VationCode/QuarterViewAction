//====================250302
//기존 개인에서의 연산을 그룹에서 회전 및 Active 이펙트 관리
//

//==========================

using UnityEngine;

namespace DUS
{
    public class EffectGrenadeManager : MonoBehaviour
    {
        public GameManager GameManager;

        [SerializeField]
        GameObject target;

        [SerializeField]
        GameObject[] effectGrenades;

        public int m_HasGrenades;
        public int m_MaxHasGrenades;

        private int currentNum;
        float disY;
        private void Awake()
        {
            GameManager = FindObjectOfType<GameManager>();
            effectGrenades = new GameObject[transform.childCount];
            for (int i = 0; i < effectGrenades.Length; i++)
            {
                effectGrenades[i] = transform.GetChild(i).gameObject;
                effectGrenades[i].SetActive(false);
            }

            m_MaxHasGrenades = effectGrenades.Length;
            currentNum = -1;
        }

        private void Start()
        {
            transform.position = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
            disY = transform.position.y - target.transform.position.y;
        }
        // Update is called once per frame
        void Update()
        {
            transform.position = new Vector3(target.transform.position.x, target.transform.position.y + disY, target.transform.position.z);
            transform.Rotate(0, Time.deltaTime * 50, 0);
        }

        public bool OnEffectGrenade()
        {
            if (currentNum >= m_MaxHasGrenades-1) return false;
            ++currentNum;
            effectGrenades[currentNum].SetActive(true);
            m_HasGrenades = currentNum + 1; //인덱스0부터 활성화 하기에 카운트는 +1함
            GameManager.m_player.m_UIManager.HasWeaponUI(GameManager.m_player);
            return true;
        }
        public bool OffEffectGrenade()
        {
            if (currentNum < 0) return false;
            effectGrenades[currentNum].SetActive(false);
            --currentNum;
            m_HasGrenades = currentNum + 1; //인덱스0부터 활성화 하기에 카운트는 +1함
            GameManager.m_player.m_UIManager.HasWeaponUI(GameManager.m_player);
            return true;
        }
    }
}