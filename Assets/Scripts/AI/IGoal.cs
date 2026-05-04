using System.Collections.Generic;
using UnityEngine;

public interface IGoal
{
    string Name { get; }
    
    // Насколько сильно NPC хочет этого прямо сейчас? (0-100)
    float GetPriority(MonoBehaviour npc);
    
    // Уже достигли цели?
    bool IsSatisfied(MonoBehaviour npc);
    
    // Можем ли мы вообще к ней приступить?
    bool CanPursue(MonoBehaviour npc);
    
    // Построить план (список действий)
    List<INPCAction> CreatePlan(MonoBehaviour npc);
}
