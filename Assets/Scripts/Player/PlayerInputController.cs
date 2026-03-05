using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StoreSimulator.Mover;
using StoreSimulator.PlayerInput;
using StoreSimulator.Jumper;
using System;
using StoreSimulator.InteractableObjects;

namespace StoreSimulator.PlayerController
{
    public class PlayerInputController : MonoBehaviour
    {
        [Header("Input set-up")]
        [SerializeField] private MonoBehaviour inputSource;

        [Header("Mover set-up")]
        [SerializeField] private CharacterMover mover;

        [Header("Jumper set-up")]
        [SerializeField] private Jumping jumper;

        [Header("Pickable picker set-up")]
        [SerializeField] private PlayerInteraction picker;

        private IInputProvider input;

        private void Awake()
        {
            if(inputSource is not IInputProvider provider)
            {
                Debug.LogError("PlayerController: Invalid inputSource");
                return;
            }

            input = provider;
        }

        private void Update()
        {
            SetMovementDirection();

            Jump();

            Interact();

            Throw();

            Pack();
        }

        private void Pack()
        {
            if (!input.IsPacking) return;

            picker.DoPack();
        }

        private void Jump()
        {
            if (!input.IsJumping) return;
            
            jumper.Jump();
        }

        private void Interact()
        {
            if (!input.IsInteracting) return;

            picker.DoInteract();
        }

        private void Throw()
        {
            if (!input.IsThrowwing) return;

            picker.DoThrow();
        }

        private void SetMovementDirection()
        {
            Vector3 direction = new Vector3(input.Horizontal, 0, input.Vertical);
            mover.Move(direction.normalized);
        }
    }
}

