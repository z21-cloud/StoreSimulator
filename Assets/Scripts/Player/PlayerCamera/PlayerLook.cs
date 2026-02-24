using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StoreSimulator.PlayerInput;

namespace StoreSimulator.PlayerCamera
{
    public class PlayerLook : MonoBehaviour
    {
        [SerializeField] private float sensitivity = 500f;
        [SerializeField] private InputManager input;
        [SerializeField] private Transform playerBody;

        private float _xRotation = 0f;

        private const float Y_ROTATION_THRESHOLD = 30f;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            playerBody.Rotate(Vector3.up, input.MouseX * sensitivity * Time.deltaTime);

            _xRotation -= input.MouseY * sensitivity * Time.deltaTime;
            _xRotation = Mathf.Clamp(_xRotation, -Y_ROTATION_THRESHOLD, Y_ROTATION_THRESHOLD);
            transform.localEulerAngles = new Vector3(_xRotation, 0f, 0f);
        }
    }
}

