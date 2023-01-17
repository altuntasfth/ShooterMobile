using UnityEngine;

namespace _Game.Scripts.AI.EnemyAI
{
    public class AIStateIdle : AIBaseState
    {
        public AIStateIdle(BaseCharacter character, bool needsExitTime) : base(character, needsExitTime)
        {
        }
        
        public override void OnEnter()
        {
            base.OnEnter();
            
            Debug.Log("Idle");
            
            character.GetComponent<AIMechanic>().navMeshAgent.SetDestination(character.transform.position);
            character.animator.CrossFadeInFixedTime("Idle", 0.1f);
        }
        
        public override void OnLogic()
        {
            base.OnLogic();

            fsm.StateCanExit();
        }
        
        public override void OnExit()
        {
            base.OnExit();
        }
    }
}