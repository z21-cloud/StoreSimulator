using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace StoreSimulator.Mover
{
    public class CharacterMover : MonoBehaviour
    {
        [Header("Movement Values Set-up")]
        [SerializeField] private float speed = 5f;
        [SerializeField] private float gravity = -9.81f;

        // private variables
        private CharacterController _characterConrtoller;
        private Vector3 _velocity;

        // constants
        private const float RESET_GROUND_GRAVITY = 2f;

        private void Awake()
        {
            _characterConrtoller = GetComponent<CharacterController>();
        }

        public void Move(Vector3 direction)
        {
            // makes character 
            if (_characterConrtoller.isGrounded && _velocity.y < 0)
                _velocity.y = -RESET_GROUND_GRAVITY;

            Vector3 moveVector = transform.right * direction.x + transform.forward * direction.z; ;
            
            // to prevert diagonal movement multiplying
            if (moveVector.magnitude > 1) moveVector.Normalize();

            Vector3 horizontalMove = moveVector * speed;

            // Velocity movement
            _velocity.y += gravity * Time.deltaTime;

            Vector3 finalVelocity = new Vector3(horizontalMove.x, _velocity.y, horizontalMove.z);

            _characterConrtoller.Move(finalVelocity * Time.deltaTime);
        }
    }
}

