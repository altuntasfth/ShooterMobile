using System;
using _Game.Scripts.Pool;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace _Game.Scripts
{
    public class BulletEntity : PoolEntity
    {
        public float speed = 10f;
        public float destroyTime = 2f;
        public float damage = 10f;

        public BaseCharacter.AIType aiType;

        private void Update()
        {
            transform.position += transform.forward * speed * Time.deltaTime;
        }

        private void OnTriggerEnter(Collider other)
        {
            BaseCharacter character = other.gameObject.GetComponent<BaseCharacter>();
            if (character.isAlive && character.aiType != aiType)
            {
                character.TakeDamage(damage);
                DOTween.Kill("Destroy" + this.GetInstanceID());
                PoolManager.Instance.Pool.Release(this.gameObject);
            }
        }
    }
}