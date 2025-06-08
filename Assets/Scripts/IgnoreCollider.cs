using UnityEngine;

namespace JiggleBall.Ragdoll.Utility
{
    public class IgnoreCollider : MonoBehaviour
    {

        public Collider otherCollider;
        public bool ignore = true;

        void Awake()
        {
            Physics.IgnoreCollision(otherCollider, GetComponentInChildren<Collider>(), ignore);
        }
    }
}