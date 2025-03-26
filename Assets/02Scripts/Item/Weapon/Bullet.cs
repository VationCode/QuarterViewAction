using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.InputSystem.HID;

namespace DUS
{
    public enum AttacType
    {
        Player,
        Melee,
        BossMissile,
        BossRock,
        BossTaunt,
        EnemyMissile
    }
    public class Bullet : MonoBehaviour
    {
        public AttacType attacker;
        public int m_damage;
        public float speed;
        public float lifeTime;

        float timer;

        private void Update()
        {
            if (attacker == AttacType.BossTaunt) return;
            ShootBullet();
        }

        void ShootBullet()
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            Timer();
        }
        protected void Timer()
        {
            timer += Time.deltaTime;

            if (timer >= lifeTime)
            {
                gameObject.SetActive(false);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (attacker != AttacType.BossRock && collision.gameObject.CompareTag("Floor"))
            {
                Destroy(gameObject,3);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Wall") && attacker != AttacType.BossMissile)
            {
                Destroy(gameObject);
            }
            if((attacker == AttacType.BossMissile || attacker == AttacType.BossRock || attacker == AttacType.EnemyMissile)  && other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                Destroy(gameObject);
            }

            if(attacker == AttacType.Player && other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                Destroy(gameObject);
            }
        }
    }
}
