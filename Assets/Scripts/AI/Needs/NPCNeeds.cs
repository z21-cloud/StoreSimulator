using System;
using System.Collections.Generic;
using StoreSimulator.StoreableItems;
using UnityEngine;

public class NPCNeeds : MonoBehaviour
{
    [SerializeField] private NeedsSO config;
    private float _timer = 0f;

    private float _currentHunger;
    private float _currentThirst;

    public float Hunger => _currentHunger;
    public float Thirst => _currentThirst;

    void Start()
    {
        _currentHunger = config.startHunger;
        _currentThirst = config.startThirst;
    } 

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= config.decreaseTickRate)
        {
            _currentHunger -= config.hungerDecreaseRate;
            _currentHunger = Mathf.Max(_currentHunger, 0f);

            _currentThirst -= config.thirstDecreaseRate;
            _currentThirst = Mathf.Max(_currentThirst, 0f);

            _timer = 0f;
        }
    }

    public void IncreaseParameters(float amountHunger, float amountThirst)
    {
        _currentHunger += amountHunger;
        _currentHunger = Mathf.Min(_currentHunger, 100f);

        _currentThirst += amountThirst;
        _currentThirst = Mathf.Min(_currentThirst, 100f);
    }
}
