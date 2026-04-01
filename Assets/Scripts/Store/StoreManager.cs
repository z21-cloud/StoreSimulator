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

        void Update()
        {
            //if(isOpen) TryOpen();
            //else TryClose();
        }

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
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
    }
}

