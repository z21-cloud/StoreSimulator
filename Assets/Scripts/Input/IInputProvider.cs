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
    }
}
