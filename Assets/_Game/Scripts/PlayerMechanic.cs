using System;
using _Game.Scripts.Pool;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Game.Scripts
{
    public class PlayerMechanic : BaseCharacter
    {
        private void OnEnable()
        {
            InputManager.Instance.PointerDown += HandleOnPointerDown;
            InputManager.Instance.PointerDrag += HandleOnPointerDrag;
            InputManager.Instance.PointerEnd += HandleOnPointerEnd;

            gameManager.shootJoystick.PointerUp += Shoot;
        }

        private void OnDisable()
        {
            InputManager.Instance.PointerDown -= HandleOnPointerDown;
            InputManager.Instance.PointerDrag += HandleOnPointerDrag;
            InputManager.Instance.PointerEnd -= HandleOnPointerEnd;
            
            gameManager.shootJoystick.PointerUp -= Shoot;
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
            Rotation();
            
            if (IsSkillActive(gameManager.movementJoystick))
            {
                Move();
            }
        }

        private void Move()
        {
            transform.position += SkillDirection(gameManager.movementJoystick) * movementSpeed * Time.deltaTime;
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

        private void Shoot()
        {
            BulletEntity bullet = PoolManager.Instance.Pool.Get().GetComponent<BulletEntity>();
            bullet.transform.position = shootPoint.position;
            bullet.transform.forward = SkillDirection(gameManager.shootJoystick);

            DOTween.Kill("Destroy" + bullet.GetInstanceID());
            DOVirtual.DelayedCall(bullet.destroyTime, () =>
            {
                bullet.isUsed = false;
                bullet.Disable.Invoke(bullet);
            }).SetId("Destroy" + bullet.GetInstanceID());
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