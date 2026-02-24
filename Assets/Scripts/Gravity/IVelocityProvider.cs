using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace StoreSimulator.GravityPhysics
{
    public interface IVelocityProvider
    {
        public float CurrentYVelocity { get; }
        public void SetVelocity(float velocity);
    }
}
