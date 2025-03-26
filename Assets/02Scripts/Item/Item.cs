//====================240811

//==================================================
//********** 아이템 기본 정보 담는 클래스 **********
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Serialization;

namespace DUS
{
    public enum ItemType { Weapon, Ammo, Coin, Grenade, Heart };

    [Serializable]
    public struct ItemInfo
    {
        public ItemType itemType;
        public int itemNum;
    }

    public class Item : MonoBehaviour
    {
        public ItemInfo itemInfo;

        [Header("[자동 삽입]"), SerializeField]
        float rotateSpeed = 30;

        Rigidbody rigid;
        SphereCollider sphereCollider;

        private void Awake()
        {
            rigid = GetComponent<Rigidbody>();
            sphereCollider = GetComponent<SphereCollider>();
        }
        private void Start()
        {
            if (rotateSpeed == 0) rotateSpeed = 30;
        }
        private void Update()
        {
            transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
        }
    }
}