using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace _Game.Scripts
{
    public class GameManager : MonoBehaviour
    {
        public Camera mainCamera;
        public Transform cameraTarget;
        
        public bool isGameStarted;
        public bool isGameOver;

        public LayerMask aiLayers;

        public int enemyTeamMemberCount = 3;
        public int playerTeamMemberCount = 3;
        
        public Transform enemyTeamBase;
        public Transform playerTeamBase;

        public GameObject enemyAIPrefab;
        public GameObject playerAIPrefab;

        public VariableJoystick movementJoystick;
        public VariableJoystick shootJoystick;
        public VariableJoystick bombJoystick;
        public VariableJoystick dashJoystick;

        public List<BaseCharacter> enemyTeamMembers;
        public List<BaseCharacter> playerTeamMembers;

        private void Awake()
        {
            isGameStarted = true;
            
            // GenerateEnemyTeamMembers();
            // GeneratePlayerTeamMembers();
        }

        private void GenerateEnemyTeamMembers()
        {
            for (int i = 0; i < enemyTeamMemberCount; i++)
            {
                AIMechanic ai = Instantiate(enemyAIPrefab, GetRandomPosition(enemyTeamBase.position), Quaternion.identity).GetComponent<AIMechanic>();
                enemyTeamMembers.Add(ai);
            }
        }
        
        private void GeneratePlayerTeamMembers()
        {
            for (int i = 0; i < playerTeamMemberCount; i++)
            {
                BaseCharacter ai = Instantiate(playerAIPrefab, GetRandomPosition(playerTeamBase.position), Quaternion.identity).GetComponent<AIMechanic>();
                playerTeamMembers.Add(ai);
            }
        }
        
        private Vector3 GetRandomPosition(Vector3 center)
        {
            Vector3 targetPosition = center + Random.insideUnitSphere.normalized * 5;
            
            NavMeshHit navMeshHit;
            bool isWalkable = NavMesh.SamplePosition(targetPosition, out navMeshHit,
                3, NavMesh.AllAreas);

            if (isWalkable)
            {
                if (!Physics.CheckSphere(navMeshHit.position, 2f, aiLayers))
                {
                    return navMeshHit.position;
                }
            }
            
            GetRandomPosition(center);

            return navMeshHit.position;
        }
    }
}