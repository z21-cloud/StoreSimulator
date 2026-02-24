using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StoreSimulator.Mover;
using StoreSimulator.PlayerInput;

namespace StoreSimulator.PlayerController
{
    public class PlayerConrtoller : MonoBehaviour
    {
        [SerializeField] private CharacterMover mover;
        [SerializeField] private InputManager input;

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

