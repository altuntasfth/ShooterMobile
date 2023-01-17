using _Game.Scripts.Pool;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

namespace _Game.Scripts.AI.EnemyAI
{
    public class AIStateEscape : AIBaseState
    {
        private Vector3 targetPoint;
        
        public AIStateEscape(BaseCharacter character, bool needsExitTime) : base(character, needsExitTime)
        {
        }
        
        public override void OnEnter()
        {
            base.OnEnter();
            
            Debug.Log("Escape");
            
            character.animator.CrossFadeInFixedTime("Run", 0.1f);

            targetPoint = GetRandomPosition();
            character.GetComponent<AIMechanic>().navMeshAgent.SetDestination(targetPoint);
        }
        
        public override void OnLogic()
        {
            base.OnLogic();
        }

        public override void OnExit()
        {
            base.OnExit();
            character.GetComponent<AIMechanic>().navMeshAgent.SetDestination(character.transform.position);
        }

        private Vector3 GetRandomPosition()
        {
            BaseCharacter target = character.GetComponent<AIMechanic>().GetTargetCharacter();
            if (target == null)
            {
                return character.transform.position;
            }
            
            Vector3 targetPosition = target.transform.position + 
                                     Random.insideUnitSphere.normalized * character.GetComponent<AIMechanic>().attackRange;
            
            NavMeshHit navMeshHit;
            bool isWalkable = NavMesh.SamplePosition(targetPosition, out navMeshHit,
                5, NavMesh.AllAreas);

            if (isWalkable)
            {
                return navMeshHit.position;
            }
            
            GetRandomPosition();

            return navMeshHit.position;
        }
    }
}