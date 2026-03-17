using System.Collections.Generic;
using UnityEngine;

public class DeliveryQueue<T> where T : MonoBehaviour
{
    private readonly int _capacity = 15;
    private T[] _buffer;
    private int _head;
    private int _tail;
    private int _count;

    public bool IsFull => _count == _capacity;
    public bool IsEmpty => _count == 0;
    public int Count => _count;

    public DeliveryQueue(int capacity = 15)
    {
        _capacity = capacity;
        _buffer = new T[_capacity];
        _count = 0;
        _head = 0;
        _tail = 0;
    }

    public T Dequeue()
    {
        if(IsEmpty)
        {
            Debug.Log($"[QueueBuffer]: is Empty! return null");
            return null;
        }

        T value = _buffer[_head];
        _buffer[_head] = default;

        _head = (_head + 1) % _capacity;
        _count--;

        return value;
    }

    public void Enqueue(T obj)
    {
        if(IsFull)
        {
            Debug.Log($"[QueueBuffer]: is Full!");
            return;
        }

        _buffer[_tail] = obj;
        _tail = (_tail + 1) % _capacity;
        _count++;
    }
}
