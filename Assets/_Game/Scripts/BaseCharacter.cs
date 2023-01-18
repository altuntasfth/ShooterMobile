using System;
using System.Collections.Generic;
using _Game.Scripts.AI.EnemyAI;
using Cinemachine.Utility;
using DG.Tweening;
using FSM;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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

        public CharacterModelEntity modelEntity;
        public GameObject criticTMPPrefab;
        public Animator animator;
        public Rigidbody rb;
        public TextMeshPro currentBulletCounterTMP;
        public int initialBulletCount = 5;
        public int currentBulletCount;
        public bool isAlive;
        public float initialHealth = 100;
        public float initialArmor = 100;
        public float currentHealth;
        public float currentArmor;
        public float healthRatio;
        public float armorRatio;

        public int shotsCount;
        public int hitShotsCount;

        public Image healthBarFillImage;
        public Image armorBarFillImage;
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
            armorBarFillImage.fillAmount = Mathf.Lerp(armorBarFillImage.fillAmount, armorRatio, Time.smoothDeltaTime * 5f);
        }

        public void ReloadBullet(BulletHolder bulletHolder)
        {
            int missingBulletCount = initialBulletCount - currentBulletCount;
            int reloadableBulletCount = 0;

            if (bulletHolder.currentBulletCount >= missingBulletCount)
            {
                bulletHolder.currentBulletCount -= missingBulletCount;
                reloadableBulletCount = missingBulletCount;
            }
            else
            {
                reloadableBulletCount = bulletHolder.currentBulletCount;
                bulletHolder.currentBulletCount = 0;
            }
            
            currentBulletCount = reloadableBulletCount;
            currentBulletCounterTMP.text = "Bullet Capacity: " + currentBulletCount;
        }

        public float GetCriticDamage(float damageValue)
        {
            if (hitShotsCount != shotsCount)
            {
                shotsCount = 0;
                hitShotsCount = 0;
            }
            else
            {
                if (hitShotsCount % 3 == 0)
                {
                    damageValue *= 2;
                    TextMeshPro criticTMP = Instantiate(criticTMPPrefab, transform.position + Vector3.up * 3f, Quaternion.identity).
                        GetComponent<TextMeshPro>();
                    criticTMP.text = "CRITIC!";
                    Destroy(criticTMP.gameObject, 2f);
                    
                    shotsCount = 0;
                    hitShotsCount = 0;
                }
            }
            
            //Debug.Log(hitShotsCount + " " + shotsCount + " " + damageValue, this);

            return damageValue;
        }

        public void TakeDamage(float damageValue)
        {
            float armorPenetrationValue = Random.Range(10f, 50f);
            float damageToHealth = 0f;
            float damageToArmor = 0f;
            
            if (currentArmor > 0f)
            {
                damageToHealth = damageValue * armorPenetrationValue / 100f;
                damageToArmor = damageValue * (100f - armorPenetrationValue) / 100f;
            }
            else
            {
                damageToHealth = damageValue;
            }
            
            currentHealth = Mathf.Clamp(currentHealth - damageToHealth, 0f, float.MaxValue);
            currentArmor = Mathf.Clamp(currentArmor - damageToArmor, 0f, float.MaxValue);
            healthRatio = currentHealth / initialHealth;
            armorRatio = currentArmor / initialArmor;
            
            if (currentHealth <= 0f)
            {
                DOTween.Kill(this.gameObject);

                isAlive = false;
                healthBarFillImage.fillAmount = 0f;
                armorBarFillImage.fillAmount = 0f;
                gameObject.tag = "Untagged";
                gameObject.layer = LayerMask.NameToLayer("Default");
                GetComponent<Collider>().enabled = false;

                animator.enabled = false;
                modelEntity.SetActiveRagdoll(true);

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