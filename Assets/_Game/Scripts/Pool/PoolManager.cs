using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace _Game.Scripts.Pool
{
    public class PoolManager : MonoBehaviour
    {
        public enum PoolType
        {
            Stack,
            LinkedList
        }

        public PoolType poolType;

        public bool collectionChecks = true;
        public int maxPoolSize = 10;

        public PoolData poolData;

        IObjectPool<GameObject> m_Pool;

        public List<GameObject> poolItems;
        
        
        public static PoolManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                Instance = this;
            }
            
            for (var i = 0; i < maxPoolSize; i++)
            {
                GameObject item = CreatePooledItem();
                Pool.Release(item);
            }
        }

        private void Update()
        {
            DebugPool();
        }

        private void DebugPool()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                poolItems.Add(Pool.Get());
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                if (poolItems.Count != 0)
                {
                    Pool.Release(poolItems[poolItems.Count-1]);
                    poolItems.Remove(poolItems[poolItems.Count - 1]);
                }
            }
        }

        public IObjectPool<GameObject> Pool
        {
            get
            {
                if (m_Pool == null)
                {
                    if (poolType == PoolType.Stack)
                        m_Pool = new ObjectPool<GameObject>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool,
                            OnDestroyPoolObject, collectionChecks, 10, maxPoolSize);
                    else
                        m_Pool = new LinkedPool<GameObject>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool,
                            OnDestroyPoolObject, collectionChecks, maxPoolSize);
                }

                return m_Pool;
            }
        }

        private GameObject CreatePooledItem()
        {
            var go = Instantiate(poolData.poolInfo.prefab);
            var poolEntity = go.GetComponent<PoolEntity>();
            poolEntity.Disable += ReturnObjectToPool;
            return go;
        }

        private void ReturnObjectToPool(PoolEntity poolEntity)
        {
            Pool.Release(poolEntity.gameObject);
        }

        private void OnReturnedToPool(GameObject gObject)
        {
            gObject.SetActive(false);
        }

        private void OnTakeFromPool(GameObject gObject)
        {
            gObject.SetActive(true);
        }

        private void OnDestroyPoolObject(GameObject gObject)
        {
            Destroy(gObject);
        }
    }
}