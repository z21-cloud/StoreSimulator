using System;
using Unity.VisualScripting;
using UnityEngine;

namespace StoreSimulator.StoreUtility
{
    public class StoreManager : MonoBehaviour
    {
        [SerializeField] private bool isOpen;
        [SerializeField] private bool isAuto;
        private StoreState _currentState;
        public static StoreManager Instance { get; private set; }
        public bool IsOpen => _currentState == StoreState.Open;
        public bool IsAuto => isAuto;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void OnEnable()
        {
            TimeManager.Instance.OnPhaseChanged += HandlePhaseChange;
        }

        public void TryOpen()
        {
            _currentState = StoreState.Open;
            Debug.Log($"[Store Manager]: Current store state is {_currentState}; IsOpen - {IsOpen}");
        }
        public void TryClose()
        {
            _currentState = StoreState.Closed;
            Debug.Log($"[Store Manager]: Current store state is {_currentState}; IsOpen - {IsOpen}");
        }

        private void HandlePhaseChange(DayPhase phase)
        {
            if (!isAuto) return;

            if (phase == DayPhase.Morning) TryOpen();
            else if (phase == DayPhase.Night) TryClose();
        }

        private void OnDisable()
        {
            TimeManager.Instance.OnPhaseChanged -= HandlePhaseChange;
        }
    }
}

