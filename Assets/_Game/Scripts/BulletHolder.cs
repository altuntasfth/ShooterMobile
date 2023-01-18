using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace _Game.Scripts
{
    public class BulletHolder : MonoBehaviour
    {
        public int initialBulletCount = 10;
        public int currentBulletCount = 10;
        public TextMeshPro bulletCountTMP;

        private void Awake()
        {
            currentBulletCount = initialBulletCount;
        }

        private void OnTriggerEnter(Collider other)
        {
            BaseCharacter character = other.gameObject.GetComponent<BaseCharacter>();
            if (character)
            {
                if (character.isAlive && character.currentBulletCount < character.initialBulletCount && currentBulletCount != 0)
                {
                    character.ReloadBullet(this);
                
                    bulletCountTMP.text = "Bullet Capacity: " + currentBulletCount;

                    if (currentBulletCount == 0)
                    {
                        DOTween.Kill("ReloadBullet" + this.GetInstanceID());
                        DOVirtual.DelayedCall(5f, () =>
                        {
                            currentBulletCount = initialBulletCount;
                            bulletCountTMP.text = "Bullet Capacity: " + currentBulletCount;
                        }).SetId("ReloadBullet" + this.GetInstanceID());
                    }
                }
            }
        }
    }
}