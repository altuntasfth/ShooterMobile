using UnityEngine;

namespace _Game.Scripts.Pool
{
    public class PoolEntity : MonoBehaviour
    {
        public delegate void OnDisableCallback(PoolEntity poolEntity);
        public OnDisableCallback Disable;
    }
}