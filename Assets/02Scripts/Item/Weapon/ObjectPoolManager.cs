using UnityEngine;
using UnityEngine.Pool;


namespace DUS
{
    public class ObjectPoolManager : MonoBehaviour
    {
        ObjectPool<GameObject> pool;
        const int maxSize = 100;
        const int initSize = 20;

        //CreateFunc: 오브젝트 생성 함수(Func), actionOnGet: 풀에서 오브젝트를 가져오는 함수(Action)
        //actionOnRelease: 오브젝트를 비활성화할 때 호출(Action), actionOnDestory: 오브젝트 파괴함수(Action)
        //collectionCheck: 중복 반환 체크(bool), defaultCapacity: 처음에 미리 생성하는 오브젝트 갯수(int)
        //maxSize: 저장할 오브젝트의 최대 갯수(int)
        /*private void Awake()
        {
            pool = new ObjectPool<GameObject>(createFunc, actionOnGet, actionOnRelease, actionOnDestroy, collectionCheck, defaultCapacity, maxSize); ;
        }
        private GameObject CreateObject() // 오브젝트 생성
        {
            return Instantiate(prefab);
        }

        private void ActivatePoolObject(GameObject obj) // 오브젝트 활성화
        {
            obj.SetActive(true);
        }

        private void DisablePoolObject(GameObject obj) // 오브젝트 비활성화
        {
            obj.SetActive(false);
        }

        private void DestroyPoolObject(GameObject obj) // 오브젝트 삭제
        {
            Destroy(obj);
        }

        public GameObject GetObject()
        {
            GameObject sel = null;

            if (pool.CountActive >= maxSize) // maxSize를 넘는다면 임시 객체 생성 및 반환
            {
                sel = CreateObject();
                sel.tag = "PoolOverObj";
            }
            else
            {
                sel = pool.Get();
            }

            return sel;
        }

        public void ReleaseObject(GameObject obj)
        {
            if (obj.CompareTag("PoolOverObj"))
            {
                Destroy(obj);
            }
            else
            {
                pool.Release(obj);
            }
        }*/
    }
}