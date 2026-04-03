using System.Collections.Generic;
using UnityEngine;

public class PriorityQueue<T>
{
    private List<(T item, float priority)> heap = new();
    public int Count => heap.Count;

    public void Enqueue(T item, float priority)
    {
        heap.Add((item, priority));

        HeapUp(heap.Count - 1);
    }

    public T Dequeue()
    {
        T rootItem = heap[0].item;
        heap[0] = heap[Count - 1];
        heap.RemoveAt(heap.Count - 1);

        if(heap.Count > 0) HeapDown(0);

        return rootItem;
    }

    public (T, float) Peek()
    {
        return heap[0];
    }

    private void HeapUp(int currentIndex)
    {
        while(currentIndex > 0)
        {
            int parentIndex = (currentIndex - 1) / 2;

            if(heap[currentIndex].priority >= heap[parentIndex].priority) break;
            Swap(parentIndex, currentIndex);
            currentIndex = parentIndex;
        }
    }

    private void HeapDown(int currentIndex)
    {
        while(currentIndex < heap.Count)
        {
            int left = 2 * currentIndex + 1;
            int right = 2 * currentIndex + 2;
            int smallest = currentIndex;

            if(left < heap.Count && heap[left].priority < heap[smallest].priority) smallest = left;
            if(right < heap.Count && heap[right].priority < heap[smallest].priority) smallest = right;
            if(smallest == currentIndex) break;

            Swap(smallest, currentIndex);
            currentIndex = smallest;
        }
    }

    private void Swap(int parentIndex, int currentIndex)
    {
        (heap[parentIndex], heap[currentIndex]) = (heap[currentIndex], heap[parentIndex]);
    }

    public void Clear()
    {
        
    }
}
