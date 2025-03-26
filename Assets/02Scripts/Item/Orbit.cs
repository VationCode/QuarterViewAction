//====================250303
//그룹에서 처리로 변경하고 해당 계산 사용x

//====================240824
// 플레이어를 따라 회전
// 플레이어에 넣고 그냥 회전 시 플레이어의 회전에 따라 또 같이 회전을 해버리기에 이상해짐
// 회전의 퍼포먼스를 더 주기위해 플레이어의 회전에 관계없이 동작되도록

//==================================================

//********** 수류탄 회전 궤도 **********
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DUS
{
    public class Orbit : MonoBehaviour
    {
        public Transform target;
        public float orbitSpeed;
        Vector3 m_offset;

        void Start()
        {
            m_offset = transform.position - target.position; //플레이어와의 거리
        }

        // Update is called once per frame
        void Update()
        {
            transform.position = target.position + m_offset;
            transform.RotateAround(target.position, Vector3.up, orbitSpeed * Time.deltaTime);
            m_offset = transform.position - target.position;
        }
    }
}