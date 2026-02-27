using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace StoreSimulator.InteractableObjects
{
    [RequireComponent(typeof(HoldableObject))]
    public class StoreableItem : MonoBehaviour, IStoreable
    {
        public Transform itemTransform => transform;

        public void OnStored(Transform shelfPoint)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;

            BoxCollider collider = GetComponent<BoxCollider>();
            collider.enabled = false;
        }

        public void OnPickedFromStore()
        {
            BoxCollider collider = GetComponent<BoxCollider>();
            collider.enabled = true;
        }
    }
}

