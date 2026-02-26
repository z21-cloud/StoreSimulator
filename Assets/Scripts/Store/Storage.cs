using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace StoreSimulator.InteractableObjects
{
    public class Storage : MonoBehaviour, IInteractable, IStorage
    {
        [SerializeField] private List<ShellfSlot> slots;
        [SerializeField] private float speed = 5f;

        private const float DISTANCE_THRESHOLD = 0.1f;

        private ShellfSlot _nextFreeSlot;

        public bool CanPlaceItem(IStoreable item)
        {
            foreach (var slot in slots)
            {
                if (!slot.IsOccupied)
                {
                    _nextFreeSlot = slot;
                    return true;
                }
            }

            return false;
        }

        public void PlaceItem(IStoreable item)
        {
            if (_nextFreeSlot.IsOccupied || _nextFreeSlot == null) return;
            
            _nextFreeSlot.Occupy();

            StartCoroutine(MoveItemToSlotPosition(_nextFreeSlot, item));
            
            item.OnStored(_nextFreeSlot.transform);
            
            _nextFreeSlot = null;
            Debug.Log($"Storage: all slots are not available");
            return;
        }

        private IEnumerator MoveItemToSlotPosition(ShellfSlot slot, IStoreable item)
        {
            while (Vector3.Distance(item.itemTransform.position, slot.transform.position) > DISTANCE_THRESHOLD)
            {
                item.itemTransform.position = Vector3.MoveTowards(
                    item.itemTransform.position,
                    slot.transform.position,
                    speed * Time.deltaTime
                );

                item.itemTransform.rotation = Quaternion.Slerp(
                    item.itemTransform.rotation,
                    slot.transform.rotation,
                    speed * Time.deltaTime
                );
                
                yield return null;
            }

            item.itemTransform.parent = slot.transform;
            item.itemTransform.position = slot.transform.position;
            item.itemTransform.rotation = slot.transform.rotation;
        }

        public string GetDescription()
        {
            throw new System.NotImplementedException();
        }
    }
}

