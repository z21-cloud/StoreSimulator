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

        private ShellfSlot _reservedSlot;

        public bool CanPlaceItem(IStoreable item)
        {
            foreach (var slot in slots)
            {
                if(!slot.IsOccupied)
                {
                    _reservedSlot = slot;
                    return true;
                }
            }

            Debug.Log($"Storage: all slots are not available");
            return false;
        }

        public void PlaceItem(IStoreable item)
        {
            if (_reservedSlot.IsOccupied || _reservedSlot == null) return;
            
            _reservedSlot.Occupy(item);
            Debug.Log($"{_reservedSlot.gameObject.name} : isOccupied {_reservedSlot.IsOccupied}");

            item.OnStored(_reservedSlot.transform);

            StartCoroutine(MoveItemToSlotPosition(_reservedSlot, item));
            
            _reservedSlot = null;
        }

        public IHoldable GetPlacedItem()
        {
            foreach (var slot in slots)
            {
                if (slot.IsOccupied)
                {
                    IHoldable holdable = slot.GetComponentInChildren<IHoldable>();
                    IStoreable storeable = slot.GetComponentInChildren<IStoreable>();
                    storeable.OnPickedFromStore();
                    slot.Release();
                    
                    Debug.Log($"{slot.name} : isOccupied {slot.IsOccupied}");
                    return holdable;
                }
            }

            return null;
        }

        private IEnumerator MoveItemToSlotPosition(ShellfSlot slot, IStoreable item)
        {
            item.itemTransform.parent = slot.transform;

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

            item.itemTransform.position = slot.transform.position;
            item.itemTransform.rotation = slot.transform.rotation;
        }

        public string GetDescription()
        {
            throw new System.NotImplementedException();
        }
    }
}

