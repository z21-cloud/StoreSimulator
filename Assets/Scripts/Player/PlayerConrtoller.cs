using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StoreSimulator.Mover;
using StoreSimulator.PlayerInput;
using StoreSimulator.Jumper;
using System;

namespace StoreSimulator.PlayerController
{
    public class PlayerConrtoller : MonoBehaviour
    {
        [Header("Input set-up")]
        [SerializeField] private InputManager input;

        [Header("Mover set-up")]
        [SerializeField] private CharacterMover mover;

        [Header("Jumper set-up")]
        [SerializeField] private Jumping jumper;

        private void Update()
        {
            SetMovementDirection();

            if (input.isJumping) Jump();
        }

        private void Jump()
        {
            jumper.Jump();
        }

        private void SetMovementDirection()
        {
            Vector3 direction = new Vector3(input.Horizontal, 0, input.Vertical);
            mover.Move(direction.normalized);
        }
    }
}

