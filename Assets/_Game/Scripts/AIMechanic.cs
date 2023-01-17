using System;
using System.Collections.Generic;
using _Game.Scripts.AI.EnemyAI;
using Cinemachine.Utility;
using FSM;
using UnityEngine;
using UnityEngine.AI;

namespace _Game.Scripts
{
    public class AIMechanic : BaseCharacter
    {
        public enum AIType
        {
            FRIEND,
            ENEMY
        };
        public AIType aiType;
        public float attackRange = 8f;
        public NavMeshAgent navMeshAgent;
        public float delayBeforeNextAttack = 0.2f;

        public List<BaseCharacter> targets;
        private StateMachine mainStateMachine;

        protected override void Awake()
        {
            base.Awake();

            if (aiType == AIType.FRIEND)
            {
                targets = gameManager.enemyTeamMembers;
            }
            else
            {
                targets = gameManager.playerTeamMembers;
            }
            
            SetupStateMachine();
        }

        private void Update()
        {
            if (mainStateMachine != null)
            {
                mainStateMachine.OnLogic();
            }
        }
        
        protected virtual void SetupStateMachine()
        {
            mainStateMachine = new StateMachine(true);

            mainStateMachine.AddState("IdleState", new AIStateIdle(this, false));
            mainStateMachine.AddState("ChaseState", new AIStateChase(this, false));
            mainStateMachine.AddState("AttackState", new AIStateAttack(this, true));
            mainStateMachine.AddState("EscapeState", new AIStateEscape(this, false));
            mainStateMachine.AddState("Death", new AIStateDeath(this, true));

            
            mainStateMachine.AddTransition("IdleState", "ChaseState", transition => 
                targets.Count > 0 && !TargetInAttackRange(transition));
            mainStateMachine.AddTransition("IdleState", "AttackState", transition => 
                TargetInAttackRange(transition));

            mainStateMachine.AddTransition("ChaseState", "IdleState", transition => 
                targets.Count == 0);
            mainStateMachine.AddTransition("ChaseState", "AttackState", transition => 
                TargetInAttackRange(transition));

            mainStateMachine.AddTransition(new Transition("AttackState", "EscapeState"));
            mainStateMachine.AddTransition("AttackState", "ChaseState", transition => 
                targets.Count > 0 && !TargetInAttackRange(transition));
            
            mainStateMachine.AddTransition("EscapeState", "IdleState", transition => 
                targets.Count > 0 && IsCharacterEscaped(transition));
            
            mainStateMachine.AddTransitionFromAny("Death", transition => !isAlive);
            
            mainStateMachine.SetStartState("IdleState");
            mainStateMachine.Init();
        }
        
        public bool TargetInAttackRange(Transition<string> arg)
        {
            if (GetTargetCharacter() == null)
            {
                return false;
            }

            var distanceToPlayer = Vector3.Distance(this.transform.position.ProjectOntoPlane(Vector3.up), 
                GetTargetCharacter().transform.position.ProjectOntoPlane(Vector3.up));
            
            return (distanceToPlayer <= attackRange);
        }

        private bool IsCharacterEscaped(Transition<string> arg)
        {
            var distanceToEscapePoint = Vector3.Distance(this.transform.position.ProjectOntoPlane(Vector3.up), 
                navMeshAgent.destination);

            return distanceToEscapePoint < 1f;
        }

        public BaseCharacter GetTargetCharacter()
        {
            BaseCharacter target = null;

            float minDistance = 99999;
            for (var i = 0; i < targets.Count; i++)
            {
                float distance = Vector3.Distance(transform.position.ProjectOntoPlane(Vector3.up), 
                    targets[i].transform.position.ProjectOntoPlane(Vector3.up));
                if (distance < minDistance)
                {
                    minDistance = distance;
                    target = targets[i];
                }
            }
            
            return target;
        }
    }
}