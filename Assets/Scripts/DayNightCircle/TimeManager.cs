using System;
using StoreSimulator.StoreUtility;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [SerializeField] private float phaseDuration = 120f; // 2 mins
    private DayPhase _currentDayPhase = DayPhase.Night;
    private float _timer = 0f;
    private int _phaseCount = 0;

    public DayPhase CurrentDayPhase => _currentDayPhase;

    void Start()
    {
        _phaseCount = Enum.GetValues(typeof(DayPhase)).Length;
        Debug.Log($"[Day Phase]: {_currentDayPhase}");
    }

    private void OnPhaseChanged()
    {
        _currentDayPhase = (DayPhase)(((int)_currentDayPhase + 1) % _phaseCount);
        Debug.Log($"[Day Phase]: {_currentDayPhase}");

        if (StoreManager.Instance.IsAuto)
        {
            if (_currentDayPhase == DayPhase.Night)
            {
                StoreManager.Instance.TryClose();
            }
            else if (_currentDayPhase == DayPhase.Morning)
            {
                StoreManager.Instance.TryOpen();
            }
        }
    }

    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= phaseDuration)
        {
            _timer = 0;
            OnPhaseChanged();
        }
    }
}
