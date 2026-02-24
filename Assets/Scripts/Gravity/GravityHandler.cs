using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace StoreSimulator.GravityPhysics
{
    public class GravityHandler : MonoBehaviour, IVelocityProvider
    {
        [SerializeField] private float gravity = -9.81f;

        //  private vars
        private CharacterController _characterController;
        private float _verticalVelocity;

        // constants
        private const float RESET_GROUND_GRAVITY = 2f;

        public float CurrentYVelocity => _verticalVelocity;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
        }

        private void Update()
        {
            ApplyGravity();
        }

        private void ApplyGravity()
        {
            // if character falling or is grounded -> reset gravity
            if (_characterController.isGrounded && _verticalVelocity < 0f)
            {
                _verticalVelocity = -RESET_GROUND_GRAVITY;
            }
            else
            {
                _verticalVelocity += gravity * Time.deltaTime;
            }
        }

        public void SetVelocity(float velocity)
        {
            _verticalVelocity = velocity;
        }
    }
}

