//====================250301
//키값의 받아드리는 코드 리팩토링

//===============================================
//InputAction시스템을 통해 크로스플랫폼 구현하기
//인풋액션에서 액션부분에 만든 이름들 앞에 (On + 액션이름)이런 식으로 함수를 만들면 콜백함수로 동작 된다.
//※ 콜백함수 - 특정 이벤트가 발생되었을 때 시스템에서 인자로 호출해 실행되는 함수(델리게이트에서 함수 실행하듯)

//===============================================
//입력이 들어왔는지 안들어왔느지만 체크

using System.Threading;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace DUS
{
    public class PlayerInputHandler : MonoBehaviour
    {
        /*#region Singleton
        private static PlayerInputHandler instance;
        public static PlayerInputHandler Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<PlayerInputHandler>();
                    if(instance == null)
                    {
                        //GameObject singletonObj = new GameObject(typeof(PlayerInputHandler).Name);
                        GameObject singletonObj = new GameObject("Player");
                        instance = singletonObj.AddComponent<PlayerInputHandler>();
                    }
                }
                return instance;
            }
        }
        #endregion*/

        PlayerInputAciton m_playerInputAction;
        PlayerLocomotion m_playerlocomotion;

        [Header("Movement")]
        public Vector2 m_InputMoveVec { get; private set; }
        public bool m_IsWalkKey { get; private set; }//달리는게 기본으로 되어있고 걷는게 기능 
        public bool m_IsJumpKey { get; private set; }
        public bool m_IsDodgeKey { get; private set; }
        public bool m_IsPickUpWeaponKey { get; private set; }

        //===== Weapon Action 관련
        [Header("Weapon")]
        public bool m_IsSwapKey { get; private set; }
        public bool m_IsAttackKey { get; private set; }
        public int m_CurrentSwapKeyNum { get; private set; }

        public bool m_isReloadKey { get; private set; }

        public bool m_isGrenadeKey { get; private set; }

        public bool m_isShoppingKey {  get; private set; }

        bool m_isState(PlayerState playerState) => m_playerlocomotion.m_playerState == playerState;

        /*private void SingleTonInitialized()
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
            //SingleTonInitialized();

            m_playerlocomotion = FindObjectOfType<PlayerLocomotion>();

            m_playerInputAction = new PlayerInputAciton();
            m_playerInputAction.Enable();
            KeyManagement();
        }

        private void KeyManagement()
        {
            m_playerInputAction.Player.Move.performed += moveVec => m_InputMoveVec = moveVec.ReadValue<Vector2>();
            m_playerInputAction.Player.Move.canceled += moveVec => m_InputMoveVec = Vector2.zero;

            m_playerInputAction.Player.Sprint.performed += walk => m_IsWalkKey = true;
            m_playerInputAction.Player.Sprint.canceled += walk => m_IsWalkKey = false;

            m_playerInputAction.Player.Jump.performed += jump => m_IsJumpKey = true;
            m_playerInputAction.Player.Jump.canceled += jump => m_IsJumpKey = false;

            m_playerInputAction.Player.Dodge.performed += dodge => m_IsDodgeKey = true;
            m_playerInputAction.Player.Dodge.canceled += dodge => m_IsDodgeKey = false;

            m_playerInputAction.Player.PickUpWeapon.performed += pickUpWeaponKey => m_IsPickUpWeaponKey = true;
            m_playerInputAction.Player.PickUpWeapon.canceled += pickUpWeaponKey => m_IsPickUpWeaponKey = false;

            m_playerInputAction.Player.WeaponSwap.performed += InputWeaponSwapKey;
            
            m_playerInputAction.Player.WeaponAttack.performed += attack => m_IsAttackKey = true;
            m_playerInputAction.Player.WeaponAttack.canceled += attack => m_IsAttackKey = false;

            m_playerInputAction.Player.Reload.performed += Reload => m_isReloadKey = true;
            m_playerInputAction.Player.Reload.canceled += Reload => m_isReloadKey = false;

            m_playerInputAction.Player.Grenade.performed += Grenade => m_isGrenadeKey = true;
            m_playerInputAction.Player.Grenade.canceled += Grenade => m_isGrenadeKey = false;

            m_playerInputAction.Player.Shopping.performed += Shopping => m_isShoppingKey = true;
            m_playerInputAction.Player.Shopping.canceled += Shopping => m_isShoppingKey = false;
        }

        public void InputWeaponSwapKey(InputAction.CallbackContext callbackContext)
        {
            m_CurrentSwapKeyNum = int.Parse(callbackContext.control.name);
            m_playerlocomotion.SwapWeapon(m_CurrentSwapKeyNum);
        }
    }
}