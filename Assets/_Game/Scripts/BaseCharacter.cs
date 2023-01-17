using System;
using System.Collections.Generic;
using _Game.Scripts.AI.EnemyAI;
using Cinemachine.Utility;
using DG.Tweening;
using FSM;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _Game.Scripts
{
    public class BaseCharacter : MonoBehaviour
    {
        public enum AIType
        {
            FRIEND,
            ENEMY
        };
        public AIType aiType;
        
        public Animator animator;
        public Rigidbody rb;
        public int bulletCapacity = 5;
        public int currentBulletCount = 5;
        public bool isAlive;
        public float initialHealth = 100;
        public float currentHealth;
        public float healthRatio;

        public Image healthBarFillImage;
        public GameManager gameManager;
        public Transform shootPoint;
        [SerializeField] protected float rotationSpeed = 10f;
        
        public UnityEvent onDeathEvent;

        protected virtual void Awake()
        {
            gameManager = FindObjectOfType<GameManager>();
        }

        protected virtual void Update()
        {
            if (!isAlive)
            {
                return;
            }
            
            healthBarFillImage.fillAmount = Mathf.Lerp(healthBarFillImage.fillAmount, healthRatio, Time.smoothDeltaTime * 5f);
        }

        public void TakeDamage(float damageValue)
        {
            currentHealth = Mathf.Clamp(currentHealth - damageValue, 0f, float.MaxValue);
            healthRatio = currentHealth / initialHealth;
            
            if (currentHealth <= 0f)
            {
                DOTween.Kill(this.gameObject);

                isAlive = false;
                healthBarFillImage.fillAmount = 0f;
                gameObject.tag = "Untagged";
                gameObject.layer = LayerMask.NameToLayer("Default");
                GetComponent<Collider>().enabled = false;
                animator.Play("Dead");

                if (onDeathEvent != null)
                {
                    onDeathEvent.Invoke();
                }
                
                if (aiType == AIType.FRIEND)
                {
                    gameManager.playerTeamMembers.Remove(this);

                    if (gameManager.playerTeamMembers.Count != 0)
                    {
                        gameManager.cameraTarget.parent = gameManager.playerTeamMembers[0].transform;
                        gameManager.cameraTarget.localPosition = Vector3.zero;
                    }
                    else
                    {
                        gameManager.LevelComplete();
                    }
                }
                else
                {
                    gameManager.enemyTeamMembers.Remove(this);

                    if (gameManager.enemyTeamMembers.Count == 0)
                    {
                        gameManager.LevelComplete();
                    }
                }

                this.enabled = false;
            }
        }
    }
}