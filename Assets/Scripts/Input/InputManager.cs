using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace StoreSimulator.PlayerInput
{
    public class InputManager : MonoBehaviour, IInputProvider
    {
        // properties
        public float Horizontal { get; private set; }
        public float Vertical { get; private set; }
        public bool MouseLMB { get; private set; }
        public float MouseX { get; private set; }
        public float MouseY { get; private set; }
        public bool IsJumping { get; private set; }
        public bool IsInteracting { get; private set; }
        public bool IsThrowwing { get; private set; }
        public bool IsPacking { get; private set; }
    
        // constants
        private const int LMB = 0; // left mouse button

        void Update()
        {
            GetHorizontalInput();
            GetVerticalInput();
            GetMouseInput();
            GetMouseRotation();
            Jumping();
            Picking();
            Throwing();
            Packing();
        }

        private void Packing()
        {
            IsPacking = Input.GetButtonDown("Pack");
        }

        private void Throwing()
        {
            IsThrowwing = Input.GetButtonDown("Throw");
        }

        private void GetHorizontalInput()
        {
            Horizontal = Input.GetAxisRaw("Horizontal");
        }
        private void GetVerticalInput()
        {
            Vertical = Input.GetAxisRaw("Vertical");
        }

        private void GetMouseInput()
        {
            MouseLMB = Input.GetMouseButton(LMB);
        }

        private void GetMouseRotation()
        {
            MouseX = Input.GetAxis("MouseX");
            MouseY = Input.GetAxis("MouseY");
        }

        private void Jumping()
        {
            IsJumping = Input.GetButtonDown("Jump");
        }

        private void Picking()
        {
            IsInteracting = Input.GetButtonDown("Interact");
        }
    }
}

