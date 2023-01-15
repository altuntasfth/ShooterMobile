using System;
using UnityEngine;

namespace _Game.Scripts
{
    public class BaseCharacter : MonoBehaviour
    {
        [SerializeField] protected GameManager gameManager;
        [SerializeField] protected Transform shootPoint;
        [SerializeField] protected float movementSpeed = 10f;
        [SerializeField] protected float rotationSpeed = 10f;

        protected virtual void Awake()
        {
            gameManager = FindObjectOfType<GameManager>();
        }
    }
}