using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace StoreSimulator.Mover
{
    public class CharacterMover : MonoBehaviour
    {
        [SerializeField] private float speed = 5f;
        [SerializeField] private float gravity = -9.81f;

        private CharacterController _characterConrtoller;
        private Vector3 _velocity;
        
        private void Awake()
        {
            _characterConrtoller = GetComponent<CharacterController>();
        }
        public void Move(Vector3 direction)
        {
            Vector3 moveVector = transform.right * direction.x + transform.forward * direction.z;
            _characterConrtoller.Move(moveVector * speed * Time.deltaTime);

            if (_characterConrtoller.isGrounded && _velocity.y < 0)
                _velocity.y = -2f;

            _velocity.y += gravity * Time.deltaTime;
            _characterConrtoller.Move(_velocity * Time.deltaTime);
        }
    }
}

