using System;
using System.Collections.Generic;
using StoreSimulator.StoreableItems;
using UnityEngine;

public class NPCNeeds : MonoBehaviour
{
    [SerializeField] private float thirstDecrease = 1f;
    [SerializeField] private float hungerDecrease = 1f;
    [SerializeField] private float decreaseTimer = 1f;

    private float _timer = 0f;
    private float _hunger = 100f;
    private float _thirst = 100f;

    public float Hunger => _hunger;
    public float Thirst => _thirst;

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= decreaseTimer)
        {
            _hunger -= hungerDecrease;
            _hunger = Mathf.Max(_hunger, 0f);

            _thirst -= thirstDecrease;
            _thirst = Mathf.Max(_thirst, 0f);

            _timer = 0f;
        }
    }

    public void IncreaseParameters(float amountHunger, float amountThirst)
    {
        _hunger += amountHunger;
        _hunger = Mathf.Min(_hunger, 100f);

        _thirst += amountThirst;
        _thirst = Mathf.Min(_thirst, 100f);
    }
}
