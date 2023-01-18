using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace _Game.Scripts
{
    public class GameManager : MonoBehaviour
    {
        public Camera mainCamera;
        public Transform cameraTarget;
        public TextMeshProUGUI scoreTMP;
        public TextMeshProUGUI levelCompleteTMP;
        
        public bool isGameStarted;
        public bool isGameOver;

        public LayerMask aiLayers;

        [Range(1, 6)] public int enemyTeamMemberCount = 3;
        [Range(1, 6)] public int playerTeamMemberCount = 3;
        
        public Transform enemyTeamBase;
        public Transform playerTeamBase;

        public GameObject playerPrefab;
        public GameObject enemyAIPrefab;
        public GameObject playerAIPrefab;
        public GameObject bulletHolderPrefab;

        public VariableJoystick movementJoystick;
        public VariableJoystick shootJoystick;
        public VariableJoystick bombJoystick;
        public VariableJoystick dashJoystick;

        public List<BaseCharacter> enemyTeamMembers;
        public List<BaseCharacter> playerTeamMembers;
        public List<BulletHolder> bulletHolders;

        public int playerScore;
        public int enemyScore;

        private void Awake()
        {
            GenerateEnemyTeamMembers();
            GeneratePlayerTeamMembers();

            playerScore = PlayerPrefs.GetInt("PlayerScore", 0);
            enemyScore = PlayerPrefs.GetInt("EnemyScore", 0);

            scoreTMP.text = "Player: " + playerScore + " | " + enemyScore + " :Enemy";
            
            isGameStarted = true;
        }

        private void GenerateEnemyTeamMembers()
        {
            for (int i = 0; i < enemyTeamMemberCount; i++)
            {
                AIMechanic ai = Instantiate(enemyAIPrefab, enemyTeamBase.GetChild(i).position, Quaternion.identity).GetComponent<AIMechanic>();
                enemyTeamMembers.Add(ai);
            }
        }
        
        private void GeneratePlayerTeamMembers()
        {
            BaseCharacter player = Instantiate(playerPrefab, playerTeamBase.GetChild(0).position, Quaternion.identity).GetComponent<BaseCharacter>();
            playerTeamMembers.Add(player);
            
            for (int i = 1; i < playerTeamMemberCount; i++)
            {
                BaseCharacter ai = Instantiate(playerAIPrefab, playerTeamBase.GetChild(i).position, Quaternion.identity).GetComponent<AIMechanic>();
                playerTeamMembers.Add(ai);
            }
        }

        public void LevelComplete()
        {
            isGameOver = true;
            DOTween.KillAll();
            UpdateScoreBoard();

            if (enemyScore == 3)
            {
                levelCompleteTMP.text = "LEVEL FAIL";
                PlayerPrefs.DeleteAll();
            }

            if (playerScore == 3)
            {
                levelCompleteTMP.text = "LEVEL COMPLETE";
                PlayerPrefs.DeleteAll();
            }
            
            DOVirtual.DelayedCall(2f, () =>
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            });
        }

        private void UpdateScoreBoard()
        {
            if (playerTeamMembers.Count == 0)
            {
                enemyScore++;
            }
            else
            {
                playerScore++;
            }
            
            scoreTMP.text = "Player: " + playerScore + " | " + enemyScore + " :Enemy";
            
            PlayerPrefs.SetInt("PlayerScore", playerScore);
            PlayerPrefs.SetInt("EnemyScore", enemyScore);
            PlayerPrefs.Save();
        }

        private void SpawnBulletHolder()
        {
            
        }
        
        public Vector3 GetRandomPosition(Vector3 center)
        {
            Vector3 targetPosition = center + Random.insideUnitSphere.normalized * 5;
            
            NavMeshHit navMeshHit;
            bool isWalkable = NavMesh.SamplePosition(targetPosition, out navMeshHit,
                3, NavMesh.AllAreas);

            if (isWalkable)
            {
                return navMeshHit.position;
            }
            
            GetRandomPosition(center);

            return navMeshHit.position;
        }
    }
}