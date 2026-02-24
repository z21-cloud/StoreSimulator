using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StoreSimulator.Mover;
using StoreSimulator.PlayerInput;

namespace StoreSimulator.PlayerController
{
    public class PlayerConrtoller : MonoBehaviour
    {
        [Header("Input set-up")]
        [SerializeField] private InputManager input;

        [Header("Mover set-up")]
        [SerializeField] private CharacterMover mover;

        private void Update()
        {
            SetMovementDirection();
        }

        private void SetMovementDirection()
        {
            Vector3 direction = new Vector3(input.Horizontal, 0, input.Vertical);
            mover.Move(direction.normalized);
        }
    }
}

