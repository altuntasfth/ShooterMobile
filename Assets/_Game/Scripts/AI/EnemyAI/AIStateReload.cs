using UnityEngine;

namespace _Game.Scripts.AI.EnemyAI
{
    public class AIStateReload : AIBaseState
    {
        public AIStateReload(BaseCharacter character, bool needsExitTime) : base(character, needsExitTime)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
            Debug.Log("Reload");
            
            character.animator.CrossFadeInFixedTime("Run", 0.1f);
        }

        public override void OnLogic()
        {
            base.OnLogic();
            
            Reload();
        }

        public override void OnExit()
        {
            base.OnExit();
            
            character.GetComponent<AIMechanic>().navMeshAgent.SetDestination(character.transform.position);
        }

        private void Reload()
        {
            var reloadTarget = character.GetComponent<AIMechanic>().GetNearestBulletHolder();

            if (reloadTarget != null)
            {
                character.GetComponent<AIMechanic>().navMeshAgent.SetDestination(reloadTarget.transform.position);
            }
            else
            {
               //character.gameManager.GetRandomPosition()
            }
        }
    }
}