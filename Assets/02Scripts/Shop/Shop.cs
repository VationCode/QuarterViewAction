
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace DUS
{
    public enum SHOPTYPE
    {
        Item,
        Weapon
    }
    public class Shop : MonoBehaviour
    {
        public RectTransform m_ShopGroupUI;
        private PlayerLocomotion m_player;
        [SerializeField]
        SHOPTYPE m_type;

        [SerializeField]
        NPCAnimationManager NPCAnimationManager;

        public GameObject m_AmmoPlus;
        public GameObject[] itemObj;
        public Transform[] itemPos;
        public int[] itemPrice;
        public string[] talkData;
        public TextMeshProUGUI dialogTMP;

        private void Awake()
        {
            NPCAnimationManager = GetComponentInChildren<NPCAnimationManager>();
        }

        public void Enter(PlayerLocomotion player)
        {
            m_player = player;
            m_ShopGroupUI.anchoredPosition = Vector3.zero;
        }

        public void Exit()
        {
            NPCAnimationManager.Hello();
            m_ShopGroupUI.anchoredPosition = Vector3.down * 1000;
        }

        public void Buy(int index)
        {
            int price = itemPrice[index];
            if (price > m_player.m_Coin)
            {
                StopCoroutine(Talk());
                StartCoroutine(Talk());
                return;
            }
            m_player.m_Coin -= price;
            Vector3 ranVec = Vector3.right * Random.Range(-3, 3)
                + Vector3.forward * Random.Range(-3, 3);
            Instantiate(itemObj[index], itemPos[index].position + ranVec, itemPos[index].rotation);
            if(m_type == SHOPTYPE.Item && index == 1)
            {
                Instantiate(m_AmmoPlus, itemPos[index].position + ranVec, itemPos[index].rotation);
            }

            m_player.m_UIManager.ChangeCoinTMP(m_player.m_Coin);
        }
        IEnumerator Talk()
        {
            dialogTMP.text = talkData[1];
            yield return new WaitForSeconds(2f);
            dialogTMP.text = talkData[0];
        }
      
    }
}