using System;
using _Game.Scripts.Pool;
using DG.Tweening;
using UnityEngine;

namespace _Game.Scripts
{
    public class BulletEntity : PoolEntity
    {
        public float speed = 10f;
        public float destroyTime = 2f;
        public float damage = 10f;
        public bool isUsed;

        private void Update()
        {
            transform.position += transform.forward * speed * Time.deltaTime;
        }
    }
}