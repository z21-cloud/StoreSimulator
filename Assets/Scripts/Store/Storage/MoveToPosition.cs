using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace StoreSimulator.InteractableObjects
{
    public class MoveToPosition : MonoBehaviour
    {
        [SerializeField] private float speed = 5f;

        private const float DISTANCE_THRESHOLD = 0.1f;

        public void MoveToSlotPosition(Transform slot)
        {
            StartCoroutine(MoveItemToSlotPosition(slot));
        }

        private IEnumerator MoveItemToSlotPosition(Transform slot)
        {
            transform.parent = slot;

            while (Vector3.Distance(transform.position, slot.position) > DISTANCE_THRESHOLD)
            {
                transform.SetPositionAndRotation(
                    Vector3.MoveTowards(
                        transform.position,
                        slot.position,
                        speed * Time.deltaTime
                    ),
                    Quaternion.Slerp(
                        transform.rotation,
                        slot.rotation,
                        speed * Time.deltaTime
                    )
                );
                yield return null;
            }

            transform.SetPositionAndRotation(slot.position, slot.rotation);
        }
    }
}

