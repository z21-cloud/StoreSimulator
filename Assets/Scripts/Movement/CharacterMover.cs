using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using StoreSimulator.GravityPhysics;

namespace StoreSimulator.Mover
{
    public class CharacterMover : MonoBehaviour
    {
        [Header("Movement Values Set-up")]
        [SerializeField] private float speed = 5f;

        // private variables
        private CharacterController _characterConrtoller;
        private IVelocityProvider _gravityProvider;

        private void Awake()
        {
            _characterConrtoller = GetComponent<CharacterController>();
            _gravityProvider = GetComponent<IVelocityProvider>();

            if(_gravityProvider == null)
            {
                Debug.LogError($"CharacterMove: Object {gameObject.name} has no gravity provider");
            }
        }

        public void Move(Vector3 direction)
        {
            // movement vector at x & z
            Vector3 moveVector = transform.right * direction.x + transform.forward * direction.z; ;
            
            // to prevert diagonal movement multiplying
            if (moveVector.magnitude > 1) moveVector.Normalize();

            float yVelocity = _gravityProvider.CurrentYVelocity;

            Vector3 finalVelocity = (moveVector * speed) + (Vector3.up * yVelocity);

            _characterConrtoller.Move(finalVelocity * Time.deltaTime);
        }
    }
}

