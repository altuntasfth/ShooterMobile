using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Game.Scripts
{
    public class CharacterModelEntity : MonoBehaviour
    {
        public List<Rigidbody> rigidbodies;
        public List<Collider> colliders;

        private void Awake()
        {
            GetRagdollRigidbodiesAndColliders();
            SetActiveRagdoll(false);
        }

        private void GetRagdollRigidbodiesAndColliders()
        {
            rigidbodies = GetComponentsInChildren<Rigidbody>().ToList();
            colliders = GetComponentsInChildren<Collider>().ToList();
        }

        public void SetActiveRagdoll(bool isActive)
        {
            for (var i = 0; i < colliders.Count; i++)
            {
                colliders[i].enabled = isActive;
            }
            
            for (var i = 0; i < rigidbodies.Count; i++)
            {
                rigidbodies[i].isKinematic = !isActive;
            }
        }
    }
}