using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace StoreSimulator.InteractableObjects
{
    [RequireComponent(typeof(HoldableObject))]
    public class ThrowableObject : MonoBehaviour, IThrowable
    {
        private IHoldable _holdable;

        private void Awake()
        {
            _holdable = GetComponent<IHoldable>();    
            
            if(_holdable == null)
            {
                Debug.LogError($"ThrowableObject: {gameObject.name} has no IHoldable component");
                return;
            }
        }

        public void Throw(Vector3 direction, float force)
        {
            Debug.Log("Throwing");
            _holdable.Release(direction * force);
        }
    }
}

