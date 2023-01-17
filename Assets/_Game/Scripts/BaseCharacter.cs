using System;
using System.Collections.Generic;
using _Game.Scripts.AI.EnemyAI;
using Cinemachine.Utility;
using FSM;
using UnityEngine;
using UnityEngine.AI;

namespace _Game.Scripts
{
    public class BaseCharacter : MonoBehaviour
    {
        public Animator animator;
        public Rigidbody rb;
        public bool isAlive;

        public GameManager gameManager;
        public Transform shootPoint;
        [SerializeField] protected float rotationSpeed = 10f;

        protected virtual void Awake()
        {
            gameManager = FindObjectOfType<GameManager>();
        }
    }
}