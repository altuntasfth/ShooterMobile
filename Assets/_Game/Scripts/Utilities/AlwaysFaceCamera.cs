using UnityEngine;

namespace _Game.Scripts.Utilities
{
    public class AlwaysFaceCamera : MonoBehaviour
    {
        public bool yAxisOnly = false;
        public int updateFrameInterval = 2;
        public Vector3 offset;
        private Transform myTransform = null;
        private Camera mainCamera;

        void Awake()
        {
            myTransform = transform;
            mainCamera = Camera.main;
        }

        void Update()
        {
            if (Time.frameCount % updateFrameInterval != 0)
            {
                return;
            }

            if (yAxisOnly)
            {
                myTransform.LookAt(new Vector3(mainCamera.transform.position.x, myTransform.position.y,
                    mainCamera.transform.position.z));
            }
            else
            {
                myTransform.LookAt(mainCamera.transform);
            }

            myTransform.transform.rotation *= Quaternion.Euler(offset);
        }
    }
}