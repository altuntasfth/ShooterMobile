using UnityEngine;

namespace _Game.Scripts.SO
{
    [CreateAssetMenu(fileName = "PlayerSettings", menuName = "Player/Create Player Settings")]
    public class PlayerSettings : ScriptableObject
    {
        public float movementSpeed = 10f;
        public float initialHealth = 100f;
        public float initialArmor = 100f;
        public int initialBulletCapacity = 10;
        
        public float shootCancelTime = 0.5f;
        public float dashCancelTime = 0.5f;
    }
}