using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DUS
{
    public class Grenade : MonoBehaviour
    {
        public GameObject m_MeshObj;
        public GameObject m_EffectObj;
        public Rigidbody m_Rigid;

        private void Start()
        {
            StartCoroutine(Explosion());
        }

        IEnumerator Explosion()
        {
            yield return new WaitForSeconds(1.5f);
            m_Rigid.linearVelocity = Vector3.zero;
            m_Rigid.angularVelocity = Vector3.zero;

            m_MeshObj.SetActive(false);
            m_EffectObj.SetActive(true);

            RaycastHit[] _rayHits = Physics.SphereCastAll(transform.position, 30, Vector3.up, 0f, LayerMask.GetMask("Enemy"));

            foreach (RaycastHit hitObj in _rayHits)
            {
                hitObj.transform.GetComponent<Enemy>().HitByGrenade(transform.position);
            }
            Destroy(gameObject, 3);
        }
    }
}