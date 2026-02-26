using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace StoreSimulator.PhysicalObjects
{
    public class ResetObjectPhysics : MonoBehaviour
    {
        public void SetPhysics(Rigidbody rb, bool value)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.useGravity = value;
        }
    }
}

