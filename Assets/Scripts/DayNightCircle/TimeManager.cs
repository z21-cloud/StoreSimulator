using System;
using StoreSimulator.StoreUtility;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [SerializeField] private float phaseDuration = 120f; // 2 mins

    [Header("Lighting Settings")]
    [SerializeField] private Light sunLight;
    private DayPhase _currentDayPhase;
    private float _timer = 0f;
    private int _phaseCount = 0;

    public DayPhase CurrentDayPhase => _currentDayPhase;
    public Action<DayPhase> OnPhaseChanged;
    public static TimeManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        _phaseCount = Enum.GetValues(typeof(DayPhase)).Length;

        _currentDayPhase = DayPhase.Morning;
        Debug.Log($"[Day Phase]: {_currentDayPhase}");
        OnPhaseChanged?.Invoke(_currentDayPhase);
    }

    private void HandlePhaseChange()
    {
        _currentDayPhase = (DayPhase)(((int)_currentDayPhase + 1) % _phaseCount);
        Debug.Log($"[Day Phase]: {_currentDayPhase}");
        OnPhaseChanged?.Invoke(_currentDayPhase);
    }

    void Update()
    {
        _timer += Time.deltaTime;

        RotateSun();

        if (_timer >= phaseDuration)
        {
            _timer = 0;
            HandlePhaseChange();
        }
    }

    private void RotateSun()
    {
        if (sunLight == null) return;

        float fullCycleDuration = phaseDuration * _phaseCount;
        float currentTotalTime = ((int)_currentDayPhase * phaseDuration) + _timer;
        float dayProgress = currentTotalTime / fullCycleDuration;

        float angle = (dayProgress * 360f) - 90f;

        sunLight.transform.rotation = Quaternion.Euler(angle, -30f, 0f);
    }
}
