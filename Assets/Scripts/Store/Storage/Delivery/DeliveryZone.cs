using System.Collections.Generic;
using StoreSimulator.Boxes;
using StoreSimulator.StoreableItems;
using UnityEngine;

namespace StoreSimulator.Delivery
{
    public class DeliveryZone : MonoBehaviour, IBoxStorage, IBoxOwner
    {
        [SerializeField] private List<Transform> slots;
        [SerializeField] private DeliveryConfig config;
        [SerializeField] private BoxPooling boxPooling;
        [SerializeField] private Transform queuePosition;

        private BoxStorage[] _placedBoxes;
        private DeliveryQueue<BoxStorage> _waitingQueue;

        private void Awake()
        {
            _placedBoxes = new BoxStorage[config.MaxCapacity];
            _waitingQueue = new DeliveryQueue<BoxStorage>(config.MaxCapacity);
        }

        public bool HasFreeSlot() => !config.IsFull || !_waitingQueue.IsFull;

        public int CurrentSlotsCount() =>
            Mathf.Abs(config.currentCount - config.MaxCapacity * 2);

        public bool CanTakeItem() => _placedBoxes.Length > 0;

        private int FindFreeSlot()
        {
            for(int i = 0; i < _placedBoxes.Length; i++)
                if(_placedBoxes[i] == null) return i;

            return -1;    
        }

        public void PlaceBox(BoxStorage box)
        {
            if (config.IsFull)
            {
                if (!_waitingQueue.IsFull)
                {
                    box.transform.position = queuePosition.position;
                    _waitingQueue.Enqueue(box);

                    Debug.Log($"[DeliveryZone]: Pallet full. Box added to queue. " +
                            $"Queue: {_waitingQueue.Count}/{config.MaxCapacity}");
                }
                return;
            }

            PlaceOnPallet(box);
        }

        private void PlaceOnPallet(BoxStorage box)
        {
            int index = FindFreeSlot();
            if(index == -1) return;

            _placedBoxes[index] = box;
            config.currentCount++;
            box.SetOwner(this);
            box.transform.position = GetNextSpawnPosition(index);
        }

        private Vector3 GetNextSpawnPosition(int index)
        {
            int column = index % config.columns;
            int row = (index / config.columns) % config.rows;
            int level = index / (config.rows * config.columns);
            int slotIndex = column + row * config.columns;

            return new Vector3
            (
                slots[slotIndex].position.x,
                slots[slotIndex].position.y + level * config.boxSize,
                slots[slotIndex].position.z
            );
        }

        public BoxStorage TakeBox()
        {
            throw new System.NotImplementedException();
        }

        public void OnBoxRemoved(BoxStorage box)
        {
            for(int i = 0; i < _placedBoxes.Length; i++)
            {
                if(_placedBoxes[i] == box)
                {
                    _placedBoxes[i] = null;
                    break;
                }
            }

            config.currentCount--;

            if(!_waitingQueue.IsEmpty)
            {
                BoxStorage next = _waitingQueue.Dequeue();
                PlaceOnPallet(next);

                Debug.Log($"[DelveryZone]: Spawned from queue. " + 
                        $"Queue remaining: {_waitingQueue.Count}");
            }
        }
    }
}

