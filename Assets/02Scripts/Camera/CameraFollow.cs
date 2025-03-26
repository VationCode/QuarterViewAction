//====================240811
//첫 시작
//카메라를 플레이어 따라가도록

//==================================================

//********** 타겟 따르는 기능 클래스 **********
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DUS
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField]
        private Transform target;

        [SerializeField]
        Vector3 offsetDis;
        [SerializeField]
        Vector3 offsetRot;
        private void LateUpdate()
        {
            transform.position = target.position + offsetDis;
            transform.rotation = Quaternion.Euler(offsetRot);
        }
    }
}