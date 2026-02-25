using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace StoreSimulator.PlayerInput
{
    public interface IInputProvider
    {
        public float Horizontal { get; }
        public float Vertical { get; }
        public bool MouseLMB { get; }
        public float MouseX { get; }
        public float MouseY { get; }
        public bool isJumping { get; }
        public bool isInteracting { get; }
    }
}
