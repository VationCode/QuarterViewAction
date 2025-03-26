// 총알 발사를 했을 때 한발만x 수십 수백발
// 총알이 어딘가에 부딪히거나 시간이지나서 삭제를 해야할 상황이 왔을 때
// C#에서는 필요없는 메모리 알아서 가비지 컬렉터가 지워줌
// 가비지 컬렉터가 비용이 비싸다
// Destory 이 함수 비용이 발생
// 오브젝트 풀링은 이미 쏜 총알을 재활용하는 방식

using UnityEngine;
using System.Collections.Generic;

namespace DUS
{
    public class BulletPoolManager : MonoBehaviour
    {
        public GameObject bulletPrefab;
        public Transform bullePoolParent;
        public int poolSize;

        private Queue<GameObject> bulletPool = new Queue<GameObject>();

        private void Awake()
        {
            // 미리 총알 생성
            for (int i = 0; i < poolSize; i++)
            {
                GameObject bullet = Instantiate(this.bulletPrefab, bullePoolParent);
                bullet.SetActive(false);
                bulletPool.Enqueue(bullet);
            }
        }

        public GameObject GetBullet()
        {
            if (bulletPool.Count > 0)
            {
                GameObject bullet = bulletPool.Dequeue(); //출력 데이터 저장 및 큐에서는 삭제
                bullet.SetActive(true);
                return bullet;
            }
            else
            {
                // 풀에 남은 총알이 없으면 추가 생성 (필요 시)
                GameObject bullet = Instantiate(this.bulletPrefab, bullePoolParent);
                bullet.SetActive(false);
                bulletPool.Enqueue(bullet); // 삽입 큐
                return GetBullet(); //오잉?재귀?
            }
        }

        public void ReturnBullet(GameObject bullet)
        {
            bullet.SetActive(false);
            bulletPool.Enqueue(bullet);
        }
    }
}
