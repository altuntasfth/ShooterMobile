using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.Scripts.Pool
{
    [CreateAssetMenu(fileName = "PoolData", menuName = "Pool/Create Pool Data")]
    public class PoolData : ScriptableObject
    {
        public PoolInfo poolInfo;
    }

    [Serializable]
    public struct PoolInfo
    {
        public string name;
        public GameObject prefab;
        public int poolSize;
    }
}