using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StoreSimulator.PlayerInput;

namespace StoreSimulator.PlayerCamera
{
    public class PlayerLook : MonoBehaviour
    {
        [Header("Camera set-up")]
        [SerializeField] private float sensitivity = 500f;
        [Header("Player's transform")]
        [SerializeField] private Transform playerBody; // take transform and rotate it with camera direction
        [Header("Input")]
        [SerializeField] private InputManager input;

        // private vars
        private float _xRotation = 0f;

        // constants
        private const float Y_ROTATION_THRESHOLD = 60f; // added to avoid magic numbers

        private void Start()
        {
            // lock cursos at start to remove it from screen
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            // left and right rotation
            playerBody.Rotate(Vector3.up, input.MouseX * sensitivity * Time.deltaTime); 

            // up and down rotation
            _xRotation -= input.MouseY * sensitivity * Time.deltaTime;
            // clamps value between in range
            _xRotation = Mathf.Clamp(_xRotation, -Y_ROTATION_THRESHOLD, Y_ROTATION_THRESHOLD);
            // apply result to camera, not player transform
            transform.localEulerAngles = new Vector3(_xRotation, 0f, 0f);
        }
    }
}

