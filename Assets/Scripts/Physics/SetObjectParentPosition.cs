using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace StoreSimulator.PhysicalObjects
{
    public class SetObjectParentPosition : MonoBehaviour
    {
        public void SetParentPosition(Transform holdPoint)
        {
            transform.position = holdPoint.position;
            transform.rotation = holdPoint.rotation;
            transform.parent = holdPoint;
        }
    }
}
