using UnityEngine;

namespace _Game.Scripts.AI.EnemyAI
{
    public class AIStateChase : AIBaseState
    {
        public AIStateChase(BaseCharacter character, bool needsExitTime) : base(character, needsExitTime)
        {
        }
        
        public override void OnEnter()
        {
            base.OnEnter();
            
            Debug.Log("Chase");
            
            character.animator.CrossFadeInFixedTime("Run", 0.1f);
        }

        public override void OnLogic()
        {
            base.OnLogic();
            
            Chase();
        }

        public override void OnExit()
        {
            base.OnExit();
            character.GetComponent<AIMechanic>().navMeshAgent.SetDestination(character.transform.position);
        }
        
        private void Chase()
        {
            var chaseTarget = character.GetComponent<AIMechanic>().GetTargetCharacter();

            if (chaseTarget != null)
            {
                character.GetComponent<AIMechanic>().navMeshAgent.SetDestination(chaseTarget.transform.position);
            }
        }
    }
}