using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "NPC Memory", menuName = "AI / NPC Memory")]
public class NPCMemoryData : ScriptableObject
{
    public int visitCount;
    public float totalSpentAllTime;
    public int lastVisitDay;
    public List<VisitRecord> history = new List<VisitRecord>();

    private const int MaxVisits = 5;

    public void AddVisit(VisitRecord record)
    {
        history.Add(record);
        if(history.Count > MaxVisits) history.RemoveAt(0);

        visitCount++;
        totalSpentAllTime += record.totalSpent;
        lastVisitDay = record.dayIndex;
    }
}
