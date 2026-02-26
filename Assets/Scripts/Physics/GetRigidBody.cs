using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace StoreSimulator.PhysicalObjects
{
    [RequireComponent(typeof(Rigidbody))]
    public class GetRigidBody : MonoBehaviour
    {
        public Rigidbody GetRB { get; private set; }

        private void Awake()
        {
            GetRB = GetComponent<Rigidbody>();
        }
    }
}

