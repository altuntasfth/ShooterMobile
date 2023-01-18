using _Game.Scripts.Pool;
using DG.Tweening;
using UnityEngine;

namespace _Game.Scripts.AI.EnemyAI
{
    public class AIStateAttack : AIBaseState
    {
        private float rotationSpeed = 0.2f;
        
        public AIStateAttack(BaseCharacter character, bool needsExitTime) : base(character, needsExitTime)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            Debug.Log("Attack");
            
            DOTween.Kill("FaceToPlayer" + character.GetInstanceID());

            if (character.GetComponent<AIMechanic>().GetTargetCharacter() != null)
            {
                Vector3 lookPos = character.GetComponent<AIMechanic>().GetTargetCharacter().transform.position - character.transform.position;
                lookPos.y = 0;
                Quaternion rotation = Quaternion.LookRotation(lookPos);
                character.transform.DORotateQuaternion(rotation, rotationSpeed).SetId("FaceToPlayer" + character.GetInstanceID())
                    .SetTarget(character.gameObject).OnComplete(() =>
                    {
                        Attack();
                    });
            }
            
            character.animator.CrossFadeInFixedTime("Attack", 0.1f);
        }

        public override void OnLogic()
        {
            base.OnLogic();
            if (timer.Elapsed > character.GetComponent<AIMechanic>().delayBeforeNextAttack)
            {
                fsm.StateCanExit();
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            
            DOTween.Kill("FaceToPlayer" + character.GetInstanceID());
        }

        private void Attack()
        {
            //if (character.currentBulletCount > 0)
            {
                BulletEntity bullet = PoolManager.Instance.Pool.Get().GetComponent<BulletEntity>();
                character.shotsCount++;
                character.currentBulletCount--;
                character.currentBulletCounterTMP.text = "Bullet Capacity: " + character.currentBulletCount;
                bullet.ownerCharacter = character;
                bullet.transform.position = character.shootPoint.position;
                bullet.transform.forward = character.transform.forward;

                DOTween.Kill("Destroy" + bullet.GetInstanceID());
                DOVirtual.DelayedCall(bullet.destroyTime, () =>
                {
                    bullet.Disable.Invoke(bullet);
                }).SetId("Destroy" + bullet.GetInstanceID());

                character.currentBulletCount--;
            }
        }
    }
}