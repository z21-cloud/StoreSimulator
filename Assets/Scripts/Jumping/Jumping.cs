using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using StoreSimulator.GravityPhysics;

namespace StoreSimulator.Jumper
{
    public class Jumping : MonoBehaviour
    {
        [Header("Jump velocity set-up")]
        [SerializeField] private float jumpVelocity = 10f;

        private CharacterController _characterConrtoller;
        private IVelocityProvider _gravityProvider;

        private void Awake()
        {
            _characterConrtoller = GetComponent<CharacterController>();
            _gravityProvider = GetComponent<IVelocityProvider>();

            if (_gravityProvider == null)
            {
                Debug.LogError($"Jumping: Object {gameObject.name} has no gravity provider");
            }

            if (_characterConrtoller == null)
            {
                Debug.LogError($"Jumping: Object {gameObject.name} has no Character Conrtoller");
            }
        }

        public void Jump()
        {
            if (!_characterConrtoller.isGrounded || _gravityProvider.CurrentYVelocity > 0f) return;

            _gravityProvider.SetVelocity(jumpVelocity);
        }
    }
}

