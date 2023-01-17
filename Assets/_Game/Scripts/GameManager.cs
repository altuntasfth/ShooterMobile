using System;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Scripts
{
    public class GameManager : MonoBehaviour
    {
        public Camera mainCamera;
        public Transform cameraTarget;
        
        public bool isGameStarted;
        public bool isGameOver;

        public VariableJoystick movementJoystick;
        public VariableJoystick shootJoystick;
        public VariableJoystick bombJoystick;
        public VariableJoystick dashJoystick;
        
        private void Awake()
        {
            isGameStarted = true;
        }
    }
}