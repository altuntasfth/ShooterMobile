using System;
using _Game.Scripts.Pool;
using _Game.Scripts.SO;
using _Game.Scripts.Utilities;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

namespace _Game.Scripts
{
    public class PlayerMechanic : BaseCharacter
    {
        public PlayerSettings playerSettings;
        public bool isAttackState;

        private void OnEnable()
        {
            InputManager.Instance.PointerDown += HandleOnPointerDown;
            InputManager.Instance.PointerDrag += HandleOnPointerDrag;
            InputManager.Instance.PointerEnd += HandleOnPointerEnd;

            gameManager.shootJoystick.PointerUp += Shoot;
            gameManager.movementJoystick.PointerDown += PlayMoveAnimation;
            gameManager.movementJoystick.PointerUp += PlayIdleAnimation;
        }

        private void OnDisable()
        {
            InputManager.Instance.PointerDown -= HandleOnPointerDown;
            InputManager.Instance.PointerDrag += HandleOnPointerDrag;
            InputManager.Instance.PointerEnd -= HandleOnPointerEnd;
            
            gameManager.shootJoystick.PointerUp -= Shoot;
            gameManager.movementJoystick.PointerDown -= PlayMoveAnimation;
            gameManager.movementJoystick.PointerUp -= PlayIdleAnimation;
        }

        protected override void Awake()
        {
            base.Awake();
            
            Initialize();
        }

        private void Initialize()
        {
            gameManager.cameraTarget.parent = this.transform;
            gameManager.cameraTarget.localPosition = Vector3.zero;
        }

        private void Update()
        {
            if (gameManager == null || !isAlive || isAttackState)
            {
                return;
            }

            Vector3 targetDirection = transform.position + SkillDirection(gameManager.movementJoystick) * 5;
            NavMeshHit navMeshHit;
            bool isWalkable = NavMesh.SamplePosition(targetDirection, out navMeshHit, 4, NavMesh.AllAreas);

            if (isWalkable)
            {
                Rotation();
                if (IsSkillActive(gameManager.movementJoystick))
                {
                    Move();
                }
            }
            else
            {
                throw new Exception(name + ": " + targetDirection + " is non-walkable point!");
            }
        }

        private void Move()
        {
            Vector2 moveVector = GetMoveVector();
            Vector2 animatorMove = moveVector;
            
            float angleBetweenLocalForwardAndMoveVector = Angle360(Vector2.up, GetAimVector());
            animatorMove = animatorMove.Rotate(-angleBetweenLocalForwardAndMoveVector);
            
            animator.SetFloat(Animator.StringToHash("MoveX"), 
                Mathf.Lerp(animator.GetFloat(Animator.StringToHash("MoveX")), 
                    animatorMove.x, Time.deltaTime * 5f));
            
            animator.SetFloat(Animator.StringToHash("MoveY"), 
                Mathf.Lerp(animator.GetFloat(Animator.StringToHash("MoveY")),
                    animatorMove.y, Time.deltaTime * 5f));

            Vector3 moveInputVector = SkillDirection(gameManager.movementJoystick);
            transform.position += moveInputVector * playerSettings.movementSpeed * Time.deltaTime;
        }

        private void Rotate(Vector3 direction)
        {
            transform.forward = Vector3.Lerp(transform.forward, direction, rotationSpeed * Time.deltaTime);
        }

        private void Rotation()
        {
            if (IsSkillActive(gameManager.shootJoystick))
            {
                Rotate(SkillDirection(gameManager.shootJoystick));
            }
            else if (IsSkillActive(gameManager.dashJoystick))
            {
                Rotate(SkillDirection(gameManager.dashJoystick));
            }
            else if (IsSkillActive(gameManager.bombJoystick))
            {
                Rotate(SkillDirection(gameManager.bombJoystick));
            }
            else
            {
                Rotate(SkillDirection(gameManager.movementJoystick));
            }
        }

        private bool IsSkillActive(VariableJoystick skillJoystick)
        {
            if (SkillDirection(skillJoystick).magnitude > 0.01f)
            {
                return true;
            }

            return false;
        }

        private Vector3 SkillDirection(VariableJoystick skillJoystick)
        {
            Vector3 skillDirection = skillJoystick.Horizontal * Vector3.right +
                                        skillJoystick.Vertical * Vector3.forward;

            return skillDirection.normalized;
        }

        private Vector2 GetAimVector()
        {
            Vector2 aimVector = Vector2.zero;

            if (IsSkillActive(gameManager.shootJoystick))
            {
                aimVector = gameManager.shootJoystick.Horizontal * Vector2.right + 
                            gameManager.shootJoystick.Vertical * Vector2.up;
            }
            else if (IsSkillActive(gameManager.dashJoystick))
            {
                aimVector = gameManager.dashJoystick.Horizontal * Vector2.right + 
                            gameManager.dashJoystick.Vertical * Vector2.up;
            }
            else if (IsSkillActive(gameManager.bombJoystick))
            {
                aimVector = gameManager.bombJoystick.Horizontal * Vector2.right + 
                            gameManager.bombJoystick.Vertical * Vector2.up;
            }
            else
            {
                aimVector = gameManager.movementJoystick.Horizontal * Vector2.right + 
                            gameManager.movementJoystick.Vertical * Vector2.up;
            }
            
            aimVector = aimVector.sqrMagnitude < 0.01f ? Vector2.up * 0.01f : aimVector;
            return aimVector;
        }
        
        private Vector2 GetMoveVector()
        {
            return gameManager.movementJoystick.Horizontal * Vector2.right +
                   gameManager.movementJoystick.Vertical * Vector2.up;
        }

        private void PlayMoveAnimation()
        {
            if (!isAttackState && IsSkillActive(gameManager.movementJoystick))
            {
                animator.CrossFadeInFixedTime("Move", 0.1f);
            }
        }
        
        private void PlayIdleAnimation()
        {
            rb.velocity = Vector3.zero;
            if (!isAttackState && !IsSkillActive(gameManager.movementJoystick))
            {
                animator.CrossFadeInFixedTime("Idle", 0.1f);
            }
        }

        private void Shoot()
        {
            isAttackState = true;
            animator.CrossFadeInFixedTime("Attack", 0.1f);
            
            BulletEntity bullet = PoolManager.Instance.Pool.Get().GetComponent<BulletEntity>();
            bullet.transform.position = shootPoint.position;
            bullet.transform.forward = transform.forward;

            DOTween.Kill("Destroy" + bullet.GetInstanceID());
            DOVirtual.DelayedCall(bullet.destroyTime, () =>
            {
                bullet.isUsed = false;
                bullet.Disable.Invoke(bullet);
            }).SetId("Destroy" + bullet.GetInstanceID());

            DOVirtual.DelayedCall(1f, () =>
            {
                isAttackState = false;
            });
        }
        
        private float Angle360(Vector2 p1, Vector2 p2, Vector2 o = default(Vector2))
        {
            Vector2 v1, v2;
            if (o == default(Vector2))
            {
                v1 = p1.normalized;
                v2 = p2.normalized;
            }
            else
            {
                v1 = (p1 - o).normalized;
                v2 = (p2 - o).normalized;
            }

            float angle = Vector2.Angle(v1, v2);
            return Mathf.Sign(Vector3.Cross(v1, v2).z) < 0 ? (360 - angle) % 360 : angle;
        }

        #region INPUT

        private void HandleOnPointerDown(PointerEventData eventData)
        {
            if (!gameManager.isGameStarted || gameManager.isGameOver)
            {
                return;
            }
            
            Debug.Log(true);
            
            RaycastHit hit;
            Vector3 screenToWorldPoint = gameManager.mainCamera.
                ScreenToWorldPoint(eventData.position.x * Vector3.right + eventData.position.y * Vector3.up - 10 *Vector3.forward);
            if (Physics.Raycast(screenToWorldPoint, Vector3.forward, out hit, 100f))
            {
                if (hit.collider != null)
                {
                    
                }
            }
        }

        private void HandleOnPointerDrag(PointerEventData eventData)
        {
            if (!gameManager.isGameStarted || gameManager.isGameOver)
            {
                return;
            }
            
            Vector3 movementDirection = gameManager.movementJoystick.Horizontal * Vector3.right +
                                        gameManager.movementJoystick.Vertical * Vector3.forward;
        }

        private void HandleOnPointerEnd(PointerEventData eventData)
        {
            if (!gameManager.isGameStarted || gameManager.isGameOver)
            {
                return;
            }
        }

        #endregion
    }
}